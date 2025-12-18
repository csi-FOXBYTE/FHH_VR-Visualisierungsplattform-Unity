/*using Cysharp.Threading.Tasks;
using FHH.Logic;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace FHH.Logic.Components.Collaboration
{
    public sealed class TestCollaborationFlow : MonoBehaviour
    {
        private CollaborationService _clientService;
        private HostCollaborationService _hostService;
        private CancellationTokenSource _clientCts;
        private CancellationTokenSource _ownerCts;

        private MeetingEvent _pickedEvent;
        private string _eventId;

#pragma warning disable CS0414 // Field is assigned but its value is never used
        private volatile bool _eventsFetched;
#pragma warning restore CS0414 // Field is assigned but its value is never used
        private volatile bool _clientSessionActive;
        private volatile bool _missingHostSeen;
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private volatile bool _clientDisconnectedSeen;
#pragma warning restore CS0414 // Field is assigned but its value is never used

        private void Start()
        {
            if (!gameObject.activeInHierarchy) return;
            InitAsync().Forget();
        }
        private async UniTask InitAsync()
        {
            await ServiceLocator.RegisterServiceAsync<HostCollaborationService>(new HostCollaborationService());
            _hostService = ServiceLocator.GetService<HostCollaborationService>();

            _clientService = ServiceLocator.GetService<CollaborationService>();
            //_hostService = ServiceLocator.GetService<HostCollaborationService>();
            _clientCts = new CancellationTokenSource();
            _ownerCts = new CancellationTokenSource();

            // Ensure client runs in TEST MODE (no Relay/persistence)
            _clientService.EnableTestMode(true);

            _clientService.ClientDisconnected += () =>
            {
                Debug.Log("[TEST] Event: ClientDisconnected");
                _clientDisconnectedSeen = true;
            };
            _clientService.MissingHost += () =>
            {
                Debug.Log("[TEST] Event: MissingHost");
                _missingHostSeen = true;
            };
            _clientService.ClientSessionActive += () =>
            {
                Debug.Log("[TEST] Event: ClientSessionActive (join succeeded)");
                _clientSessionActive = true;
            };
            _clientService.SessionEnded += () => Debug.Log("[TEST] Event: SessionEnded");
            _clientService.ClientStatusUpdate += (status, code) =>
            {
                Debug.Log(
                    $"[TEST] ClientStatusUpdate: status={status}, joinCode={(string.IsNullOrEmpty(code) ? "(null)" : code)}");
            };

            RunFlowAsync().Forget();
            await UniTask.CompletedTask;
        }

        private static string RandomJoinCode5()
        {
            var r = UnityEngine.Random.Range(0, 100000);
            return r.ToString("D5");
        }

        private async UniTaskVoid RunFlowAsync()
        {
            try
            {
                Debug.Log("[TEST] Waiting 20s for User to log in…");
                await UniTask.Delay(TimeSpan.FromSeconds(20));

                var events = await _clientService.GetMeetingEventsAsync();
                _eventsFetched = true;
                Debug.Log($"[TEST] /events returned {events.Count} item(s)");
                foreach (var e in events)
                {
                    Debug.Log($"[TEST] - Id={e.Id}, Title={e.Title}, Status={e.Status}, " +
                              $"Start(local)={e.StartTime?.ToString("O") ?? "null"}, End(local)={e.EndTime?.ToString("O") ?? "null"}, " +
                              $"OwnerEmail={e.Owner?.Email}");
                }

                _pickedEvent = events.FirstOrDefault();
                if (_pickedEvent == null)
                {
                    Debug.LogWarning("[TEST] No events available; aborting test.");
                    return;
                }

                _eventId = _pickedEvent.Id;

                // CLIENT: subscribe and wait for ACTIVE+code then join (no Relay)
                Debug.Log("[TEST] Acting as CLIENT: calling Collaborate(ev) to listen for SSE…");
                _clientSessionActive = false;
                _missingHostSeen = false;
                _clientDisconnectedSeen = false;
                _clientService.CollaborateAsync(_pickedEvent, _clientCts.Token).Forget();

                // OWNER (TEST-ONLY): host with mock code, 15s watchdog; rehost with NEW code if client doesn’t join
                var firstCode = RandomJoinCode5();
                Debug.Log(
                    $"[TEST] Acting as OWNER (TEST-ONLY): HOST with testJoinCode={firstCode} (no Relay/persistence)...");
                _hostService.HostWithWatchdogAsync(_pickedEvent, firstCode, TimeSpan.FromSeconds(15), _ownerCts.Token)
                    .Forget();

                // Wait for client join success after first host (up to 40s as before)
                await WaitUntilOrTimeout(() => _clientSessionActive, TimeSpan.FromSeconds(40),
                    "[TEST] Waiting ClientSessionActive after first HOST…");

                // Give a bit of time for HB ticks and potential updates
                await UniTask.Delay(TimeSpan.FromSeconds(5));

                _missingHostSeen = false;
                // Simulate OWNER crash: stop HB (no Relay)
                Debug.Log("[TEST] Simulating OWNER disconnect: stopping HB.");
                _hostService.StopHeartbeat();

                // Wait for client to observe MISSING_HOST from SSE
                await WaitUntilOrTimeout(() => _missingHostSeen, TimeSpan.FromSeconds(20),
                    "[TEST] Waiting for client to see MISSING_HOST…");

                // REHOST later with a NEW random code (still test-only host)
                var rehostCode = RandomJoinCode5();
                Debug.Log($"[TEST] Rehosting OWNER with testJoinCode={rehostCode} …");
                _clientSessionActive = false;
                _hostService.HostWithWatchdogAsync(_pickedEvent, rehostCode, TimeSpan.FromSeconds(10), _ownerCts.Token)
                    .Forget();

                // Wait for client to become active again on new joinCode
                await WaitUntilOrTimeout(() => _clientSessionActive, TimeSpan.FromSeconds(40),
                    "[TEST] Waiting ClientSessionActive after rehost…");

                // Stop-after-20s total soak time (no EndSession)
                Debug.Log("[TEST] Soaking for 20s more, then stopping (EndSession intentionally skipped).");
                await UniTask.Delay(TimeSpan.FromSeconds(20));
                try
                {
                    _hostService.StopHeartbeat();
                }
                catch
                {
                }

                try
                {
                    _clientCts?.Cancel();
                }
                catch
                {
                }

                Debug.Log("[TEST] Test flow complete.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TEST] Exception in test flow: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static async UniTask WaitUntilOrTimeout(Func<bool> predicate, TimeSpan timeout, string waitingLog)
        {
            Debug.Log(waitingLog);
            var start = DateTime.UtcNow;
            while (DateTime.UtcNow - start < timeout)
            {
                if (predicate()) return;
                await UniTask.Delay(TimeSpan.FromMilliseconds(200));
            }

            Debug.LogWarning($"{waitingLog} timed out after {timeout.TotalSeconds}s");
        }

        private void OnDestroy()
        {
            try
            {
                _clientCts?.Cancel();
                _clientCts?.Dispose();
                Debug.Log("[TEST] Client CTS canceled & disposed");
            }
            catch
            {
            }

            try
            {
                _ownerCts?.Cancel();
                _ownerCts?.Dispose();
                Debug.Log("[TEST] Owner CTS canceled & disposed");
            }
            catch
            {
            }

            try
            {
                _hostService?.StopHeartbeat();
            }
            catch
            {
            } 
        }
    }
}*/