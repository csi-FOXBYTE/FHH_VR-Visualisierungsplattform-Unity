/*using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;
using Newtonsoft.Json;

namespace FHH.Logic.Components.Collaboration
{
    // [TEST-ONLY] Host service with strict TEST MODE behaviour:
    // - Never touches persistence
    // - Never uses Relay/UGS
    // - Always uses backend /host, /rehost, /heartbeat endpoints
    public sealed class HostCollaborationService : IAppServiceBase
    {
        private AppConfig _cfg;
        private IHttpClient _http;

        private CancellationTokenSource _heartbeatCts;
        private string _currentJoinCode;

        // Reference to the client-side service just to observe "JoinSucceeded"
        private CollaborationService _client; 

        public void InitService() { }

        public void DisposeService()
        {
            try { _heartbeatCts?.Cancel(); _heartbeatCts?.Dispose(); } catch { }
        }

        private void EnsureResolved()
        {
            _cfg ??= ServiceLocator.GetService<ConfigurationService>().AppSettings;
            _http ??= new HttpClientWithRetry();
            _client ??= ServiceLocator.GetService<CollaborationService>();
        }

        private string RandomJoinCode5()
        {
            var r = UnityEngine.Random.Range(0, 100000);
            return r.ToString("D5");
        }

        public async UniTask HostWithWatchdogAsync(MeetingEvent ev, string initialJoinCode, TimeSpan joinWatchdog, CancellationToken ct)
        {
            EnsureResolved();

            // Subscribe to client success
            var joined = false;
            void OnClientJoin() { joined = true; ULog.Info("[HostTest] Client reported JoinSucceeded"); }

            _client.ClientSessionActive += OnClientJoin; // subscribe
            try
            {
                var code = string.IsNullOrEmpty(initialJoinCode) ? RandomJoinCode5() : initialJoinCode;

                // Attempt HOST; if fails, immediate rehost with NEW code
                var ok = await NotifyHostAsync(ev.Id, code, ct);
                if (!ok)
                {
                    ULog.Warning("[HostTest] initial HOST failed → rehosting with NEW code");
                    code = RandomJoinCode5();
                    ok = await NotifyRehostAsync(ev.Id, code, ct);
                    if (!ok)
                    {
                        ULog.Error("[HostTest] host+rehost both failed; abort");
                        return;
                    }
                }

                _currentJoinCode = code;
                StartHeartbeat(ev.Id, code, ct);

                // Watchdog: wait joinWatchdog for client to join; otherwise rehost with NEW code (same event)
                var start = DateTime.UtcNow;
                while (!joined && DateTime.UtcNow - start < joinWatchdog && !ct.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: ct);
                }

                if (!joined && !ct.IsCancellationRequested)
                {
                    ULog.Warning("[HostTest] client did not join in time → stop HB and rehost with NEW code");
                    StopHeartbeat();

                    var newCode = RandomJoinCode5();
                    var rehostOk = await NotifyRehostAsync(ev.Id, newCode, ct);
                    if (!rehostOk)
                    {
                        ULog.Error("[HostTest] rehost failed; abort");
                        return;
                    }
                    _currentJoinCode = newCode;
                    StartHeartbeat(ev.Id, newCode, ct);
                }
            }
            finally
            {
                _client.ClientSessionActive -= OnClientJoin;
            }
        }

        public void StopHeartbeat()
        {
            try
            {
                _heartbeatCts?.Cancel();
                _heartbeatCts?.Dispose();
                _heartbeatCts = null;
                ULog.Info("[HostTest] Heartbeat stopped");
            }
            catch { }
        }

        private void StartHeartbeat(string eventId, string joinCode, CancellationToken outerCt)
        {
            _heartbeatCts?.Cancel();
            _heartbeatCts?.Dispose();
            _heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(outerCt);

            ULog.Info($"[HostTest] Heartbeat start eventId={eventId} joinCode={joinCode}");
            _ = SendHeartbeatLoopAsync(eventId, joinCode, _heartbeatCts.Token);
        }

        // backend calls

        private async UniTask<bool> NotifyHostAsync(string eventId, string joinCode, CancellationToken ct)
        {
            var endpoint = _cfg.ServerConfig.EndpointPath("EventHost", new { id = eventId });
            var baseUrl = _cfg.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
            var headers = BuildHeaders();
            var body = JsonConvert.SerializeObject(new JoinCodeBody { joinCode = joinCode });

            var t0 = DateTime.UtcNow;
            ULog.Info($"[HostTest] → POST {url} bodyLen={body?.Length ?? 0}");
            var resp = await _http.PostAsync(url, jsonData: body, headers: headers, cancellationToken: ct);
            var dt = (int)(DateTime.UtcNow - t0).TotalMilliseconds;
            ULog.Info($"[HostTest] ← {(resp.IsSuccess ? "OK" : "ERR")} in {dt}ms for NotifyHost");
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
            ULog.Info($"[HostTest] → POST {url} bodyLen={body?.Length ?? 0}");
            var resp = await _http.PostAsync(url, jsonData: body, headers: headers, cancellationToken: ct);
            var dt = (int)(DateTime.UtcNow - t0).TotalMilliseconds;
            ULog.Info($"[HostTest] ← {(resp.IsSuccess ? "OK" : "ERR")} in {dt}ms for NotifyRehost");
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
                    ULog.Info($"[HostTest] Heartbeat → POST {url} seq={seq} joinCode={joinCode}");
                    var resp = await _http.PostAsync(url, jsonData: body, headers: headers, cancellationToken: ct);
                    var dt = (int)(DateTime.UtcNow - t0).TotalMilliseconds;
                    if (!resp.IsSuccess)
                    {
                        ULog.Warning($"[HostTest] Heartbeat ← FAIL seq={seq} {resp.StatusCode} in {dt}ms err={resp.ErrorMessage}");
                    }
                    else
                    {
                        ULog.Info($"[HostTest] Heartbeat ← OK seq={seq} in {dt}ms");
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    ULog.Warning($"[HostTest] Heartbeat exception: {ex.Message}");
                }

                seq++;
                try { await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: ct); }
                catch (OperationCanceledException) { break; }
            }

            ULog.Info("[HostTest] Heartbeat stop");
        }

        private sealed class JoinCodeBody
        {
            [JsonProperty("joinCode")] public string joinCode { get; set; }
        }

        private System.Collections.Generic.Dictionary<string, string> BuildHeaders()
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var cfgHeaders = _cfg.ServerConfig.Headers;
            if (cfgHeaders != null)
            {
                for (int i = 0; i < cfgHeaders.Length; i++)
                {
                    var h = cfgHeaders[i];
                    if (!string.IsNullOrWhiteSpace(h?.Key))
                        dict[h.Key] = h.Value ?? string.Empty;
                }
            }
            // Authorization header
            var tokens = ServiceLocator.GetService<OAuth2AuthenticationService>()?.GetTokenStorageProvider();
            var token = tokens?.GetAccessToken();
            if (!string.IsNullOrWhiteSpace(token))
                dict["Authorization"] = $"Bearer {token}";
            return dict;
        }
    }
}
*/