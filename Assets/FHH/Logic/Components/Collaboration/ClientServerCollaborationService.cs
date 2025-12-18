/*using Cysharp.Threading.Tasks;
using FHH.Logic.Components.Networking;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Core.Services.DataTransfer;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Core.Services.PersistenceService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Unity.Services.Vivox;
using UnityEngine;

namespace FHH.Logic.Components.Collaboration
{
    public sealed class ClientServerCollaborationService : IAppServiceBase
    {
        // Events for UI
        public event Action ClientDisconnected;
        public event Action MissingHost;
        public event Action ClientSessionActive;
        public event Action SessionEnded;
        public event Action<string *//*status*//*, string *//*joinCode*//*> ClientStatusUpdate;
        public event Action VivoxConnected;

        public event Action<MeetingEvent> HostSessionStarting;
        public event Action<string *//*joinCode*//*> HostSessionReady;

        public string CommandChannelName { get; private set; }

        private bool _testMode;

        // Cached at first use inside Collaborate()
        private AppConfig _cfg;
        private IHttpClient _http;
        private ITokenStorageProvider _tokens;
        private ServerEventService _serverEvents;
        private PermissionService _permission;
        private UGSService _ugs;
        private RelayHandler _relay;
        private VivoxHandler _vivox;
        private IPersistenceService _persistence;
        private VivoxCommandHandler _vivoxCommand;
        private CommandBusService _commandBus;

        // Owner flow state
        private CancellationTokenSource _heartbeatCts;
        private string _currentJoinCode;
        private IDisposable _statusSubscription;
        public bool TestOwnerPath = true; // for force testing as owner

        private CancellationTokenSource _sessionCts;
        private CancellationTokenSource _clientJoinCts;

        public void InitService()
        {
        }

        public void DisposeService()
        {
            // Best-effort cleanup (no exceptions)
            try { _statusSubscription?.Dispose(); } catch { }
            try { _heartbeatCts?.Cancel(); _heartbeatCts?.Dispose(); } catch { }
        }

        public void EnableTestMode(bool enabled)
        {
            _testMode = enabled; 
        }

        public async UniTask<List<MeetingEvent>> GetMeetingEventsAsync(CancellationToken ct = default)
        {
            await EnsureResolvedAsync(); // resolve services on demand

            var endpoint = _cfg.ServerConfig.EndpointPath("Events");
            var headers = BuildHeaders();
            var baseUrl = _cfg.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
            
            var t0 = DateTime.UtcNow;
            ULog.Info($"[Collab] → GET {url}");

            var resp = await _http.GetAsync(url, headers, ct);

            var dt = (int)(DateTime.UtcNow - t0).TotalMilliseconds;
            var bodySnippet = string.IsNullOrEmpty(resp.Data) ? "" : (resp.Data.Length > 300 ? resp.Data.Substring(0, 300) : resp.Data);
            ULog.Info($"[Collab] ← {(resp.IsSuccess ? "OK" : "ERR")} in {dt}ms body=\"{bodySnippet}\"");

            if (!resp.IsSuccess || string.IsNullOrWhiteSpace(resp.Data))
            {
                ULog.Error($"[Collab] /events failed: {resp.ErrorMessage}");
                return new List<MeetingEvent>();
            }

            try
            {
                var raw = JsonConvert.DeserializeObject<MeetingEventListItem[]>(resp.Data) ?? Array.Empty<MeetingEventListItem>();
                var result = new List<MeetingEvent>(raw.Length);
                foreach (var e in raw)
                {
                    result.Add(ToMeetingEventFromListItem(e));
                }
                return result;
            }
            catch (Exception ex)
            {
                ULog.Error($"[Collab] Failed to parse events: {ex.Message}");
                return new List<MeetingEvent>();
            }
        }

        private void ForceMute(bool isMuted)
        {
            _vivox?.ForceMuteLocalPlayer(isMuted);
        }
        
        public async UniTask CollaborateAsync(MeetingEvent ev, CancellationToken ct, string testJoinCode = null)
        {
            await EnsureResolvedAsync(); // resolve services on demand
            // ensure UGS modules
            _ugs ??= ServiceLocator.GetService<UGSService>();
            if (_relay == null)
            {
                if (_ugs?.GetModule<RelayHandler>() == null)
                    await ServiceLocator.GetService<UGSService>().RegisterModuleAsync(new RelayHandler());
                ULog.Info("[Collab] RelayHandler registered");
            }
            _relay ??= _ugs?.GetModule<RelayHandler>();
            if (_vivox == null)
            {
                if (_ugs?.GetModule<VivoxHandler>() == null)
                {
                    var displayName = ServiceLocator.GetService<PermissionService>().GetUser().DisplayName;
                    await ServiceLocator.GetService<UGSService>().RegisterModuleAsync(new VivoxHandler(displayName));
                    ULog.Info("[Collab] VivoxHandler registered for user: " + displayName);
                }
            }
            _vivox ??= _ugs?.GetModule<VivoxHandler>();

            // register command bus
            if (_commandBus == null)
            {
                await ServiceLocator.RegisterServiceAsync<CommandBusService>(new CommandBusService());
            }
            _commandBus = ServiceLocator.GetService<CommandBusService>();
            if (_commandBus == null)
            {
                ULog.Error("[Collab] CommandBusService not found!");
                return;
            }
            _commandBus.MuteAllChanged += ForceMute;
           
            _sessionCts?.Cancel();
            _sessionCts?.Dispose();
            _sessionCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var sct = _sessionCts.Token;

            // Prepare current user + roles
            var user = _permission.GetUser();
            var userEmail = user?.Email ?? string.Empty;

            var persistKey = ev.Id; // use eventId directly
            ULog.Info($"[Collab] Collaborate start eventId={ev?.Id} userEmail={userEmail} ownerEmail={ev?.Owner?.Email}");

            if (TestOwnerPath)
            {
                Debug.LogWarning("TESTING OWNER PATH");
                if (string.IsNullOrWhiteSpace(ev?.Owner?.Email)) Debug.LogError("Owner Email empty during Test.");
                userEmail = ev.Owner.Email;
            }

            // owner path
            if (!string.IsNullOrWhiteSpace(userEmail) &&
                !string.IsNullOrWhiteSpace(ev?.Owner?.Email) &&
                string.Equals(userEmail, ev.Owner.Email, StringComparison.OrdinalIgnoreCase))
            {
                HostSessionStarting?.Invoke(ev);

                // Ensure owner+moderator roles
                if (!_permission.IsOwner()) { _permission.AddRole("owner"); ULog.Info("[Collab] Role add=owner"); }
                if (!_permission.IsModerator()) { _permission.AddRole("moderator"); ULog.Info("[Collab] Role add=moderator"); }

                // Host or rehost depending on persisted state
                string joinCode;
                if (!string.IsNullOrEmpty(testJoinCode))
                {
                    ULog.Info($"[Collab] TEST MODE: using provided joinCode={testJoinCode}; attempting backend HOST (no persistence)");
                    var hosted = await NotifyHostAsync(ev.Id, testJoinCode, sct);
                    if (!hosted)
                    {
                        ULog.Warning("[Collab] TEST MODE host failed; not persisting; returning owner path early");
                        return;
                    }
                    joinCode = testJoinCode;
                    HostSessionReady?.Invoke(joinCode);
                }
                else
                {
                    if (_testMode)
                    {
                        ULog.Warning("[Collab] TEST MODE owner path without testJoinCode is not supported (no Relay).");
                        return;
                    }
                    var persisted = await _persistence.LoadDataAsync<HostedSessionState>(persistKey);
                    ULog.Info($"[Collab] Persist.Load key={persistKey} found={(persisted != null)}");
                    persisted ??= new HostedSessionState();
                    joinCode = await HostOrRehostIfNeededAsync(ev.Id, persisted, sct);
                    if (string.IsNullOrWhiteSpace(joinCode))
                    {
                        ULog.Error("[Collab] Unable to obtain joinCode for hosting.");
                        return;
                    }
                }
                // always stop any prior heartbeat, then start a fresh one for this joinCode
                _heartbeatCts?.Cancel();
                _heartbeatCts?.Dispose();
                _heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(sct);

                _currentJoinCode = joinCode;
                HostSessionReady?.Invoke(joinCode);

                // register vivox command handler
                RegisterVivoxCommandHandlerAsync().Forget();

                // Start heartbeat loop
                ULog.Info($"[Collab] Heartbeat start eventId={ev.Id} joinCode={_currentJoinCode}");
                SendHeartbeatLoopAsync(ev.Id, _currentJoinCode, _heartbeatCts.Token).Forget();

                await SetHostProjectFromEventAsync(ev, _heartbeatCts.Token);

                // Clean-up on cancellation -> revoke roles except guest + LayerManager.Reset
                AwaitCancellationCleanupOwnerAsync(ev.Id, persistKey, _heartbeatCts, sct)
                    .Forget(ex => ULog.Warning($"[Collab] Owner cleanup task faulted: {ex.Message}"));
                return;
            }

            // CLIENT
            await SubscribeStatusAndJoinAsync(ev.Id, sct);
            AwaitCancellationCleanupClientAsync(sct)
                .Forget(ex => ULog.Warning($"[Collab] Client cleanup task faulted: {ex.Message}"));
        }

        private async UniTask SetHostProjectFromEventAsync(MeetingEvent ev, CancellationToken ct)
        {
            try
            {
                var projectId = ev?.ProjectId;

                if (!string.IsNullOrWhiteSpace(projectId))
                {
                    var project = await LayerManager.Instance.GetProjectAsync(projectId);
                    await LayerManager.Instance.SetProjectAsync(project);
                    ULog.Info($"[Collab] Host set project id={projectId}");
                }
                else
                {
                    ULog.Info("[Collab] Host: no projectId on event; unloading project");
                    LayerManager.Instance.UnloadProjectAsync().Forget();
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                ULog.Warning($"[Collab] Host project set failed: {ex.Message}");
            }
        }

        public async UniTask EndSessionAsync(string eventId, CancellationToken ct = default)
        {
            await EnsureResolvedAsync();

            // Only the owner should be calling this, but we won't hard-fail; the backend will enforce.
            var endpoint = _cfg.ServerConfig.EndpointPath("EventEnd", new { id = eventId });
            var baseUrl = _cfg.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
            var headers = BuildHeaders();

            var t0 = DateTime.UtcNow;
            ULog.Info($"[Collab] → POST {url} (EndSession)");

            var resp = await _http.PostAsync(url, headers:headers, cancellationToken: ct);

            var dt = (int)(DateTime.UtcNow - t0).TotalMilliseconds;
            ULog.Info($"[Collab] ← {(resp.IsSuccess ? "OK" : "ERR")} in {dt}ms for EndSession");

            if (!resp.IsSuccess)
            {
                ULog.Warning($"[Collab] /events/{eventId}/end failed: {resp.ErrorMessage}");
            }

            // clear persisted joinCode
            await _persistence.DeleteDataAsync(eventId);
            ULog.Info($"[Collab] Persist.Delete key={eventId}");
            
            SessionEnded?.Invoke();
        }

        // Owner helpers
        
        private async UniTask<string> HostOrRehostIfNeededAsync(string eventId, HostedSessionState persisted, CancellationToken ct)
        {
            var isRehost = persisted != null && persisted.Hosting;

            // Fresh host via Relay
            var session = await _relay.HostAsync(maxPlayers: 100); 
            if (session == null || string.IsNullOrWhiteSpace(session.Code))
            {
                ULog.Error("[Collab] Relay host returned no join code.");
                return null;
            }

            var joinCode = session.Code;
            ULog.Info($"[Collab] Host {(isRehost ? "rehost" : "fresh")} eventId={eventId} joinCode={joinCode}");

            var notified = isRehost
                ? await NotifyRehostAsync(eventId, joinCode, ct)
                : await NotifyHostAsync(eventId, joinCode, ct);

            if (!notified)
            {
                ULog.Error("[Collab] Backend host/rehost notification failed.");
                return null;
            }
            
            var newState = new HostedSessionState
            {
                Hosting = true
            };

            CommandChannelName = "cmd_" + joinCode;
            await _vivox.JoinCommandChannelAsync(CommandChannelName);
            await _vivox.JoinChannelAsync(joinCode, ChatCapability.TextAndAudio);

            await _persistence.SaveDataAsync(eventId, newState);
            ULog.Info($"[Collab] Persist.Save key={eventId} Hosting=true joinCode={joinCode}");

            return joinCode;
        }

        private async UniTask<bool> NotifyHostAsync(string eventId, string joinCode, CancellationToken ct)
        {
            var endpoint = _cfg.ServerConfig.EndpointPath("EventHost", new { id = eventId });
            var headers = BuildHeaders();
            var baseUrl = _cfg.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
            var body = JsonConvert.SerializeObject(new JoinCodeBody { joinCode = joinCode });

            var t0 = DateTime.UtcNow;
            ULog.Info($"[Collab] → POST {url} bodyLen={body?.Length ?? 0}");

            var resp = await _http.PostAsync(url, jsonData: body, headers: headers, cancellationToken: ct);

            var dt = (int)(DateTime.UtcNow - t0).TotalMilliseconds;
            ULog.Info($"[Collab] ← {(resp.IsSuccess ? "OK" : "ERR")} in {dt}ms for NotifyHost");

            return resp.IsSuccess;
        }

        private async UniTask<bool> NotifyRehostAsync(string eventId, string joinCode, CancellationToken ct)
        {
            var endpoint = _cfg.ServerConfig.EndpointPath("EventRehost", new { id = eventId });
            var baseUrl = _cfg.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
            var headers = BuildHeaders();
            var body = JsonConvert.SerializeObject(new JoinCodeBody { joinCode = joinCode });

            var t0 = DateTime.UtcNow;
            ULog.Info($"[Collab] → POST {url} bodyLen={body?.Length ?? 0}");

            var resp = await _http.PostAsync(url, jsonData: body, headers: headers, cancellationToken: ct);

            var dt = (int)(DateTime.UtcNow - t0).TotalMilliseconds;
            ULog.Info($"[Collab] ← {(resp.IsSuccess ? "OK" : "ERR")} in {dt}ms for NotifyRehost");

            return resp.IsSuccess;
        }

        private async UniTask SendHeartbeatLoopAsync(string eventId, string joinCode, CancellationToken ct)
        {
            var endpoint = _cfg.ServerConfig.EndpointPath("EventHeartbeat", new { id = eventId });
            var baseUrl = _cfg.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
            var headers = BuildHeaders();
            var body = JsonConvert.SerializeObject(new JoinCodeBody { joinCode = joinCode });

            var seq = 0;
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var t0 = DateTime.UtcNow;
                    if (seq == 0) // only log first heartbeat to reduce spam
                        ULog.Info($"[Collab] Heartbeat → POST {url} seq={seq} joinCode={joinCode}; only logging 1st heartbeat");
                    var resp = await _http.PostAsync(url, jsonData: body, headers: headers, cancellationToken: ct);
                    var dt = (int)(DateTime.UtcNow - t0).TotalMilliseconds;
                    var snippet = string.IsNullOrEmpty(resp.Data) ? "" : (resp.Data.Length > 300 ? resp.Data.Substring(0, 300) : resp.Data);
                    if (!resp.IsSuccess)
                    {
                        ULog.Warning($"[Collab] Heartbeat ← FAIL seq={seq} {resp.StatusCode} in {dt}ms err={resp.ErrorMessage}");
                    }
                    else
                    {
                        if (seq == 0) // only log first heartbeat to reduce spam
                            ULog.Info($"[Collab] Heartbeat ← OK seq={seq} in {dt}ms body=\"{snippet}\"");
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ULog.Warning($"[Collab] Heartbeat exception: {ex.Message}");
                }

                seq++;
                try { await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: ct); }
                catch (OperationCanceledException) { break; }
            }
            ULog.Info("[Collab] Heartbeat stop");
        }

        private async UniTask AwaitCancellationCleanupOwnerAsync(string eventId, string persistKey, CancellationTokenSource heartbeatCts, CancellationToken lct)
        {
            ULog.Info("[Collab] Owner cleanup wait start");
            try
            {
                // Wait until canceled
                await UniTask.WaitUntilCanceled(lct);
            }
            catch (OperationCanceledException)
            {
                // expected
            }

            try { heartbeatCts?.Cancel(); heartbeatCts?.Dispose(); } catch { }

            await _relay.EndSessionAsync();
            await _vivox.LeaveChannelAsync(CommandChannelName);
            await _vivox.LeaveChannelAsync(_currentJoinCode);
            UnregisterVivoxCommandHandlerAsync().Forget();

            //ULog.Info("[Collab] Owner cleanup: heartbeat canceled & disposed");
            ResetRole();
            LayerManager.Instance.UnloadProjectAsync().Forget();
            ULog.Info("[Collab] Owner cleanup done (roles reset, project unloaded)");
        }

        // Client helpers

        private async UniTask SubscribeStatusAndJoinAsync(string eventId, CancellationToken ct)
        {
            // Ensure relay module
            if (_relay == null) _relay = _ugs.GetModule<RelayHandler>();

            var endpoint = _cfg.ServerConfig.EndpointPath("EventStatus", new { id = eventId });
            var baseUrl = _cfg.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
            //var headers = BuildHeaders();
            //headers["Accept"] = "text/event-stream";
            ULog.Info($"[Collab] SSE subscribe start url={url}");

            // cancel any prior sub
            try { _statusSubscription?.Dispose(); } catch { }
            _statusSubscription = null;

            _statusSubscription = await _serverEvents.SubscribeUrlAsync<string>(
                url,
                s => s,
                data =>
                {
                    // Robustly process each chunk; log raw and handle JSON-only payloads
                    if (string.IsNullOrEmpty(data)) return;

                    //  debug visibility of every chunk
                    var raw200 = data.Length > 200 ? data.Substring(0, 200) : data;
                    ULog.Info($"[Collab] SSE chunk[200]=\"{raw200}\"");

                    // If the server sends pure JSON (no 'data:' prefix), handle directly
                    var firstNonWs = data.TrimStart();
                    if (firstNonWs.Length > 0 && (firstNonWs[0] == '{' || firstNonWs[0] == '['))
                    {
                        // normalize to data: path for reuse
                        OnStatusData(eventId, "data:" + firstNonWs);
                        return;
                    }

                    //  Process per-line for 'data :' and 'error :' variants
                    var lines = data.Split('\n');
                    foreach (var line in lines)
                    {
                        var trimmed = line?.Trim();
                        if (string.IsNullOrEmpty(trimmed)) continue;

                        if (trimmed == ":ok")
                        {
                            ULog.Info("[Collab] SSE keep-alive");
                            continue;
                        }

                        // accept optional spaces before/after colon: data : { ... }
                        if (trimmed.StartsWith("data", StringComparison.OrdinalIgnoreCase))
                        {
                            var idx = trimmed.IndexOf(':');
                            if (idx >= 0)
                            {
                                var json = trimmed.Substring(idx + 1).Trim();
                                OnStatusData(eventId, "data:" + json); // normalize
                            }
                            continue;
                        }

                        if (trimmed.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                        {
                            var idx = trimmed.IndexOf(':');
                            var err = idx >= 0 ? trimmed.Substring(idx + 1).Trim() : trimmed;
                            OnStatusData(eventId, "error:" + err);
                            continue;
                        }
                    }
                },
                ct);

            ULog.Info("[Collab] SSE subscribe success");
        }

        private void OnStatusData(string eventId, string raw)
        {
            try
            {
                var trimmed = raw?.Trim();
                if (string.IsNullOrEmpty(trimmed)) return;

                if (trimmed.Length > 0 && (trimmed[0] == '{' || trimmed[0] == '['))
                {
                    var update = JsonConvert.DeserializeObject<MeetingStatusUpdate>(trimmed);
                    if (update == null) return;
                    HandleStatusUpdate(update);
                    return;
                }

                if (trimmed.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    var json = trimmed.Substring("data:".Length).Trim();
                    var update = JsonConvert.DeserializeObject<MeetingStatusUpdate>(json);
                    if (update == null) return;
                    HandleStatusUpdate(update);
                    return;
                }
                else if (trimmed.StartsWith("error:", StringComparison.OrdinalIgnoreCase))
                {
                    ULog.Warning($"[CollaborationService] SSE error: {trimmed}");
                }
            }
            catch (Exception ex)
            {
                var raw200 = string.IsNullOrEmpty(raw) ? "" : (raw.Length > 200 ? raw.Substring(0, 200) : raw);
                ULog.Warning($"[Collab] SSE parse error: {ex.Message} raw[200]=\"{raw200}\"");
            }
        }

        private void JoinClientIfReady(MeetingStatusUpdate update)
        {
            if (!string.Equals(_currentJoinCode, update.joinCode, StringComparison.Ordinal))
            {
                ULog.Info($"[Collab] Client join: joinCode change {_currentJoinCode} → {update.joinCode}");
                _currentJoinCode = update.joinCode;

                // wait until join completes successfully
                _clientJoinCts?.Cancel();
                _clientJoinCts?.Dispose();
                _clientJoinCts = new CancellationTokenSource();
                RegisterVivoxCommandHandlerAsync().Forget();
                JoinAsClientAsync(update, _clientJoinCts.Token).Forget();
            }
        }

        private async UniTask RegisterVivoxCommandHandlerAsync()
        {
            // register vivox command handler
            if (_vivoxCommand == null)
            {
                if (_ugs?.GetModule<VivoxCommandHandler>() == null)
                {
                    CommandChannelName = "cmd_" + _currentJoinCode;
                    await ServiceLocator.GetService<UGSService>().RegisterModuleAsync(new VivoxCommandHandler(_commandBus, CommandChannelName));
                    ULog.Info("[Collab] VivoxCommandHandler registered");
                }
            }
            _vivoxCommand = _ugs?.GetModule<VivoxCommandHandler>();
        }

        private async UniTask UnregisterVivoxCommandHandlerAsync()
        {
            try
            {
                if (_vivoxCommand != null)
                {
                    await _vivoxCommand.DisposeAsync();
                    _vivoxCommand = null;
                    CommandChannelName = string.Empty;
                    ULog.Info("[Collab] VivoxCommandHandler disposed");
                }
            }
            catch (Exception ex)
            {
                ULog.Warning($"[Collab] VivoxCommandHandler dispose failed: {ex.Message}");
            }
        }

        private void HandleStatusUpdate(MeetingStatusUpdate update)
        {
            // Roles sync (moderator vs attendee)
            if (string.Equals(update.role, "moderator", StringComparison.OrdinalIgnoreCase))
            {
                if (!_permission.IsModerator())
                {
                    _permission.AddRole("moderator");
                    ULog.Info("[Collab] Role add=moderator (from SSE)");
                }
            }

            var status = update.status?.ToUpperInvariant();
            ClientStatusUpdate?.Invoke(status, update.joinCode);

            if (status == "END")
            {
                // instead of directly ending on backend call
                // we react to UGS instead and keep the option to wait for ACTIVE here
                //SafeLeaveClient();
                //SessionEnded?.Invoke();
                return;
            }

            if (status == "CANCELED")
            {
                //SafeLeaveClient();
                //ClientDisconnected?.Invoke();
                return;
            }

            if (status == "MISSING_HOST")
            {
                MissingHost?.Invoke();
                // Rejoin only when different code appears (intended behavior)
                if (!string.IsNullOrWhiteSpace(update.joinCode) &&
                    !string.Equals(update.joinCode, _currentJoinCode, StringComparison.Ordinal))
                {
                    JoinClientIfReady(update); // ACTIVE gating happens inside
                }
                return;
            }

            // PLANNED: never join, even if joinCode present
            if (status == "PLANNED")
            {
                return; // wait for ACTIVE
            }

            // ACTIVE: require joinCode; join only if code changed
            if (status == "ACTIVE" && !string.IsNullOrWhiteSpace(update.joinCode))
            {
                JoinClientIfReady(update);
                return;
            }

            // Unknown statuses ignored
        }

        private async UniTaskVoid JoinAsClientAsync(MeetingStatusUpdate update, CancellationToken ct)
        {
            try
            {
                // in TEST MODE: no UGS/Relay calls; treat as successful "join" once ACTIVE+code arrives
                if (_testMode)
                {
                    ULog.Info($"[Collab] TEST MODE: Simulated client join with code={_currentJoinCode} (no Relay)");
                }
                else
                {
                    ULog.Info($"[Collab] Relay.Join begin code={_currentJoinCode}");
                    await _relay.JoinAsync(null, _currentJoinCode, ct);
                    await _vivox.JoinCommandChannelAsync(CommandChannelName);
                    await _vivox.JoinChannelAsync(_currentJoinCode, ChatCapability.TextAndAudio);
                }

                if (!string.IsNullOrWhiteSpace(update.projectId))
                {
                    var project = await LayerManager.Instance.GetProjectAsync(update.projectId);
                    await LayerManager.Instance.SetProjectAsync(project);
                    ULog.Info($"[Collab] Client set project id={update.projectId}");
                }
                else
                {
                    await LayerManager.Instance.UnloadProjectAsync();
                    ULog.Info("[Collab] Client unloaded project (no projectId)");
                }

                // Success signal AFTER join finishes (or simulated in TEST MODE)
                ClientSessionActive?.Invoke();
            }
            catch (OperationCanceledException)
            {
                ULog.Warning("[Collab] Relay.Join canceled");
            }
            catch (Exception ex)
            {
                ULog.Error($"[Collab] Join client failed: {ex.Message}");
            }
        }

        private void SafeLeaveClient()
        {
            _clientJoinCts?.Cancel();
            _clientJoinCts?.Dispose();
            _clientJoinCts = null;

            _currentJoinCode = null;
            ClientDisconnected?.Invoke();
            ULog.Info("[Collab] Client leave");
        }

        private async UniTask AwaitCancellationCleanupClientAsync(CancellationToken lct)
        {
            ULog.Info("[Collab] Client cleanup wait start");
            try { await UniTask.WaitUntilCanceled(lct); } catch (OperationCanceledException) { }

            try { _statusSubscription?.Dispose(); } catch { }

            await _relay.LeaveSessionAsync();
            await _vivox.LeaveChannelAsync(CommandChannelName);
            await _vivox.LeaveChannelAsync(_currentJoinCode);
            UnregisterVivoxCommandHandlerAsync().Forget();
            SafeLeaveClient();
            ResetRole();
            await LayerManager.Instance.UnloadProjectAsync();
            ULog.Info("[Collab] Client cleanup done");
        }

        // Utilities

        private MeetingEvent ToMeetingEventFromListItem(MeetingEventListItem src)
        {
            // Convert ISO-8601 (UTC) to local time for model
            DateTime? startLocal = ParseUtcToLocal(src.startTime);
            DateTime? endLocal = ParseUtcToLocal(src.endTime);

            return new MeetingEvent
            {
                Id = src.id,
                Status = src.status,
                Title = src.title,
                StartTime = startLocal,
                EndTime = endLocal,
                Owner = new MeetingOwner
                {
                    Name = src.owner?.name,
                    Email = src.owner?.email,
                    Id = src.owner?.id
                },
                ProjectId = src.projectId
            };
        }

        private static DateTime? ParseUtcToLocal(string isoUtc)
        {
            if (string.IsNullOrWhiteSpace(isoUtc)) return null;
            if (DateTimeOffset.TryParse(
                    isoUtc, 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, 
                    out var dto))
            {
                var local = dto.ToLocalTime();
                return new DateTime(local.Ticks, DateTimeKind.Local);
            }
            return null;
        }

        private Dictionary<string, string> BuildHeaders()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var cfgHeaders = _cfg.ServerConfig.Headers;
            if (cfgHeaders != null)
            {
                for (int i = 0; i < cfgHeaders.Length; i++)
                {
                    var h = cfgHeaders[i];
                    if (!string.IsNullOrWhiteSpace(h?.Key))
                    {
                        dict[h.Key] = h.Value ?? string.Empty;
                    }
                }
            }

            var token = _tokens.GetAccessToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                dict["Authorization"] = $"Bearer {token}";
            }
            return dict;
        }

        private async UniTask EnsureResolvedAsync()
        {
            _cfg ??= ServiceLocator.GetService<ConfigurationService>().AppSettings;
            _http ??= new HttpClientWithRetry();
            _tokens ??= ServiceLocator.GetService<OAuth2AuthenticationService>().GetTokenStorageProvider();
            _serverEvents ??= ServiceLocator.GetService<ServerEventService>();
            _permission ??= ServiceLocator.GetService<PermissionService>();
            _persistence ??= ServiceLocator.GetService<PersistenceService>();
           
            await UniTask.Yield();
        }

        private void ResetRole()
        {
            // Remove moderator and owner roles if present
            if (_permission.IsModerator()) { _permission.RemoveRole("moderator"); ULog.Info("[Collab] Role remove=moderator"); }
            if (_permission.IsOwner()) { _permission.RemoveRole("owner"); ULog.Info("[Collab] Role remove=owner"); }
        }

        public void StopSession()
        {
            try { _statusSubscription?.Dispose(); _statusSubscription = null; } catch { }
            try { _heartbeatCts?.Cancel(); _heartbeatCts?.Dispose(); _heartbeatCts = null; } catch { }
            try { _sessionCts?.Cancel(); _sessionCts?.Dispose(); _sessionCts = null; } catch { }
            try { _clientJoinCts?.Cancel(); _clientJoinCts?.Dispose(); _clientJoinCts = null; } catch { }
            // cleanup command bus and vivox command handler
            
            try
            {
                _vivox.ForceMuteLocalPlayer(false);
                _commandBus.MuteAllChanged -= ForceMute;
            } catch { }
            try { _vivoxCommand?.DisposeAsync().Forget(); _vivoxCommand = null; } catch { }
            _currentJoinCode = null;
            //ResetRole();
            //LayerManager.Instance.UnloadProjectAsync().Forget();
            ULog.Info("[Collab] Session stopped (SSE/heartbeat canceled)");
        }

        
        private sealed class JoinCodeBody
        {
            [JsonProperty("joinCode")] public string joinCode { get; set; }
        }

        public void StopHeartbeat()
        {
            try
            {
                _heartbeatCts?.Cancel();
                _heartbeatCts?.Dispose();
                _heartbeatCts = null;
                ULog.Info("[Collab] Heartbeat manually stopped");
            }
            catch {}
        }
    }
}*/