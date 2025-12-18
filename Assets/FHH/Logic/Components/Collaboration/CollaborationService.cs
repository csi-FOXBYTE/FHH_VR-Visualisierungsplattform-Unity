using Cysharp.Threading.Tasks;
using FHH.Logic.Components.Networking;
using FHH.Logic.Components.Sunlight;
using FHH.UI;
using FHH.UI.Sun;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Core.Services.Permission;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FHH.Input;
using FHH.Logic.Components.Laserpointer;
using Unity.Mathematics;
using Unity.Services.Vivox;
using UnityEngine;

namespace FHH.Logic.Components.Collaboration
{
    public sealed class CollaborationService : IAppServiceBase
    {
        // Events for UI
        //public event Action ClientDisconnected;
        //public event Action MissingHost;
        //public event Action ClientSessionActive;
        //public event Action SessionEnded;
        //public event Action<string /*status*/, string /*joinCode*/> ClientStatusUpdate;
        public event Action VivoxConnected;

        //public event Action<MeetingEvent> HostSessionStarting;
        //public event Action<string /*joinCode*/> HostSessionReady;

        public string CommandChannelName { get; private set; }

        // Cached at first use inside CollaborateAsync()
        private AppConfig _cfg;
        private IHttpClient _http;
        private ITokenStorageProvider _tokens;
        private PermissionService _permission;
        private UGSService _ugs;
        private VivoxHandler _vivox;
        private VivoxCommandHandler _vivoxCommand;
        //private IPersistenceService _persistence; 
        private CommandBusService _commandBus;
        private SunCalculator _sunCalculator;
        private PlayerController _pc;
        private LaserPointSelector _laserPointSelector;
        private CancellationTokenSource _sessionCts;
        private string _currentJoinCode;
        public bool TestOwnerPath = false; // for force testing as owner

        public bool IsGuidedModeEnabled;
        
        public void InitService()
        {
        }

        public void DisposeService()
        {
            // Delegate cleanup to StopSession for best-effort shutdown
            try
            {
                StopSession();
            }
            catch
            {
            }
        }

        public void EnableTestMode(bool enabled)
        {
            // no test mode in Vivox-only needed
            ULog.Info("[Collab] EnableTestMode called but test mode is no longer used in Vivox-only mode.");
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
                var today = DateTime.Now.Date;
                var cutoffDate = today.AddDays(-1); // anything <= this date will be ignored
                foreach (var e in raw)
                {
                    var meetingEvent = ToMeetingEventFromListItem(e);
                    if (meetingEvent.StartTime.HasValue)
                    {
                        var eventDate = meetingEvent.StartTime.Value.Date;
                        if (eventDate <= cutoffDate)
                        {
                            continue; // skip events from yesterday and older
                        }
                    }

                    result.Add(meetingEvent);
                    //result.Add(ToMeetingEventFromListItem(e));
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

        private void SetGuidedMode(string projectId, string variant, bool isGuided)
        {
            IsGuidedModeEnabled = isGuided;
            ServiceLocator.GetService<UIManager>().ShowNotification(isGuided ? "GuidedModeOn" : "GuidedModeOff", 4f);
            if (isGuided)
            {
                if (string.IsNullOrEmpty(projectId)) return;
                SetProjectAsync(projectId).Forget();
            }
            else
            {
                //LayerManager.Instance.UnloadProjectAsync().Forget();
            }
        }
        
        private async UniTask SetProjectAsync(string projectId)
        {
            var p = await LayerManager.Instance.GetProjectAsync(projectId);
            await LayerManager.Instance.SetProjectAsync(p);
        }

        private void SetSunTime(int year, int month, int day, int hour, int minute)
        {
            if (_sunCalculator == null) return;
            _sunCalculator.SetTime(year, month, day, hour, minute);
        }

        private void OnIndicatorChanged(Vector3 position)
        {
            _laserPointSelector.ShowIndicatorAt(position);
        }

        private void OnTeleportOccurred(double3 position, Quaternion rotation)
        {
            _pc?.TeleportTo(position, rotation, true);
        }

        private void OnVariantChanged(string variant)
        {
            LayerManager.Instance.SetVariantByIdAsync(variant).Forget();
        }

        public async UniTask CollaborateAsync(MeetingEvent ev, CancellationToken ct, string testJoinCode = null)
        {
            // Vivox-only collaboration flow, no Relay, no SSE
            await EnsureResolvedAsync(); // resolve services on demand

            // ensure UGS + Vivox
            _ugs ??= ServiceLocator.GetService<UGSService>();
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
            _commandBus.GuidedModeChanged += SetGuidedMode;
            _commandBus.SunTimeChanged += SetSunTime;
            _commandBus.IndicatorPositionChanged += OnIndicatorChanged;
            _commandBus.TeleportOccurred += OnTeleportOccurred;
            _commandBus.VariantChanged += OnVariantChanged;
            
            // reset and create session CTS
            _sessionCts?.Cancel();
            _sessionCts?.Dispose();
            _sessionCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var sct = _sessionCts.Token;

            // Prepare current user + roles
            var user = _permission.GetUser();
            var userEmail = user?.Email ?? string.Empty;

            if (TestOwnerPath)
            {
                Debug.LogWarning("TESTING OWNER PATH");
                if (string.IsNullOrWhiteSpace(ev?.Owner?.Email)) Debug.LogError("Owner Email empty during Test.");
                userEmail = ev.Owner.Email;
            }

            var isOwner =
                !string.IsNullOrWhiteSpace(userEmail) &&
                !string.IsNullOrWhiteSpace(ev?.Owner?.Email) &&
                string.Equals(userEmail, ev.Owner.Email, StringComparison.OrdinalIgnoreCase);

            var isModeratorByList =
                ev?.Moderators != null &&
                ev.Moderators.Exists(m =>
                    !string.IsNullOrWhiteSpace(m.Email) &&
                    string.Equals(m.Email, userEmail, StringComparison.OrdinalIgnoreCase));

            ULog.Info($"[Collab] Collaborate start eventId={ev?.Id} userEmail={userEmail} ownerEmail={ev?.Owner?.Email}");

            // Assign roles using owner + moderators
            if (isOwner)
            {
                if (!_permission.IsOwner())
                {
                    _permission.AddRole("owner");
                    ULog.Info("[Collab] Role add=owner");
                }
                if (!_permission.IsModerator())
                {
                    _permission.AddRole("moderator");
                    ULog.Info("[Collab] Role add=moderator (owner)");
                }
            }
            else if (isModeratorByList && !_permission.IsModerator())
            {
                _permission.AddRole("moderator");
                ULog.Info("[Collab] Role add=moderator (from moderators list)");
            }

            // Join Vivox session channels based on event id
            if (!await JoinVivoxSessionAsync(ev, sct))
            {
                return;
            }

            var sessionId = _currentJoinCode;
            var commandChannelName = CommandChannelName; 

            // Load project for all participants (host + clients) from ev.ProjectId
            await SetProjectFromEventAsync(ev, sct);

            // Unified cleanup on cancellation
            AwaitCancellationCleanupAsync(sessionId, commandChannelName, sct)
                .Forget(ex => ULog.Info($"[Collab] Cleanup task faulted: {ex.ToString()}"));
            
        }

        // Common project loading for all participants
        private async UniTask SetProjectFromEventAsync(MeetingEvent ev, CancellationToken ct)
        {
            try
            {
                var projectId = ev?.ProjectId;

                if (!string.IsNullOrWhiteSpace(projectId))
                {
                    var project = await LayerManager.Instance.GetProjectAsync(projectId);
                    await LayerManager.Instance.SetProjectAsync(project);
                    ULog.Info($"[Collab] Set project id={projectId}");
                }
                else
                {
                    //ULog.Info("[Collab] No projectId on event; unloading project");
                    //LayerManager.Instance.UnloadProjectAsync().Forget();
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ULog.Warning($"[Collab] Project set failed: {ex.Message}");
            }
        }

        public async UniTask EndSessionAsync(string eventId, CancellationToken ct = default)
        {
            //await EnsureResolvedAsync();
            ULog.Info($"[Collab] EndSessionAsync called for eventId={eventId} (no-op in Vivox-only mode)");
            // Intentionally not ending channels here; session life cycle is driven by cancellation / StopSession
            await UniTask.CompletedTask;
        }

        // Vivox-only session id (channel name) based on event.Id
        private string GetSessionId(MeetingEvent ev)
        {
            return ev?.Id;
        }

        // Join Vivox main + command channels for this event, fire VivoxConnected
        private async UniTask<bool> JoinVivoxSessionAsync(MeetingEvent ev, CancellationToken ct)
        {
            _currentJoinCode = GetSessionId(ev);
            if (string.IsNullOrWhiteSpace(_currentJoinCode))
            {
                ULog.Error("[Collab] Cannot start Vivox session: event.Id is null/empty.");
                return false;
            }

            CommandChannelName = "cmd_" + _currentJoinCode;

            await RegisterVivoxCommandHandlerAsync(); // ensures VivoxCommandHandler for command channel

            await _vivox.JoinCommandChannelAsync(CommandChannelName);
            await _vivox.JoinChannelAsync(_currentJoinCode, ChatCapability.TextAndAudio);

            VivoxConnected?.Invoke();

            return true;
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

        // Unified cleanup for host + clients
        private async UniTask AwaitCancellationCleanupAsync(
            string sessionId,
            string commandChannelName,
            CancellationToken lct)
        {
            ULog.Info("[Collab] Cleanup wait start");
            try { await UniTask.WaitUntilCanceled(lct); } catch (OperationCanceledException) { }

            try
            {
                if (_vivox != null && !string.IsNullOrEmpty(commandChannelName)) 
                {
                    await _vivox.LeaveChannelAsync(commandChannelName);
                }
            }
            catch (Exception ex)
            {
                ULog.Warning($"[Collab] Vivox leave command channel failed: {ex.Message}");
            }

            try
            {
                if (_vivox != null && !string.IsNullOrEmpty(sessionId))
                {
                    await _vivox.LeaveChannelAsync(sessionId);
                }
            }
            catch (Exception ex)
            {
                ULog.Warning($"[Collab] Vivox leave main channel failed: {ex.Message}");
            }

            var isCurrentSession = string.Equals(_currentJoinCode, sessionId, StringComparison.Ordinal);

            if (isCurrentSession)
            {
                SafeLeaveClient();
                if (_commandBus != null)
                {
                    try
                    {
                        _commandBus.MuteAllChanged -= ForceMute;
                        _commandBus.GuidedModeChanged -= SetGuidedMode;
                        _commandBus.SunTimeChanged -= SetSunTime;
                        _commandBus.IndicatorPositionChanged -= OnIndicatorChanged;
                        _commandBus.TeleportOccurred -= OnTeleportOccurred;
                        _commandBus.VariantChanged -= OnVariantChanged;
                    }
                    catch { }
                }
                try
                {
                    if (_permission != null)
                    {
                        ResetRole();
                    }
                }
                catch
                { }
                try
                {
                    if (LayerManager.Instance != null)
                    {
                        await LayerManager.Instance.UnloadProjectAsync();
                    }
                }
                catch (Exception ex)
                {
                    ULog.Warning($"[Collab] Project unload failed during cleanup: {ex.Message}");
                }
            }

            ULog.Info("[Collab] Cleanup done");
        }

        private void SafeLeaveClient()
        {
            _currentJoinCode = null;
            ULog.Info("[Collab] Client leave");
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
                ProjectId = src.projectId,
                Moderators = src.moderators != null
                    ? src.moderators.ConvertAll(m => new MeetingModerator
                    {
                        Id = m.id,
                        Name = m.name,
                        Email = m.email
                    })
                    : new List<MeetingModerator>()
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
            _permission ??= ServiceLocator.GetService<PermissionService>();

            _sunCalculator = GameObject.FindGameObjectWithTag("Sun").GetComponent<SunCalculator>();
            if (_sunCalculator == null)
            {
                Debug.LogError($"{nameof(SunPresenter)}: No {nameof(SunCalculator)} found.");
            }

            _pc ??= GameObject.FindFirstObjectByType<PlayerController>();
            if (_pc == null)
            {
                Debug.LogError($"{nameof(CollaborationService)}: No PlayerController found.");
            }

            _laserPointSelector ??= GameObject.FindFirstObjectByType<LaserPointSelector>();
            if (_laserPointSelector == null)
            {
                Debug.LogError($"{nameof(CollaborationService)}: No LaserPointSelector found.");
            }

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
            // Vivox-only stop: cancel session CTS, let cleanup handle the rest
            try
            {
                _sessionCts?.Cancel();
                _sessionCts?.Dispose();
                _sessionCts = null;
            }
            catch
            {
            }

            try
            {
                _vivox?.ForceMuteLocalPlayer(false);
                _commandBus.MuteAllChanged -= ForceMute;
                _commandBus.GuidedModeChanged -= SetGuidedMode;
            }
            catch
            {
            }

            ULog.Info("[Collab] Session stopped");
        }

        public void StopHeartbeat()
        {
            // No heartbeat in Vivox-only mode
            ULog.Info("[Collab] StopHeartbeat called (no-op in Vivox-only mode)");
        }
    }
}