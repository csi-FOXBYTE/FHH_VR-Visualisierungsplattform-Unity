//using Cysharp.Threading.Tasks;
//using FHH.Logic.Components.MeetingEvent;
//using Foxbyte.Core;
//using Foxbyte.Core.Services.AuthService;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using Newtonsoft.Json;

//namespace FHH.Logic.Components.Networking
//{
//    //[Serializable]
//    //public struct EventEnvelope
//    //{
//    //    public string Id;
//    //    public string JoinCode;
//    //    public string Name;
//    //    public string State;
//    //    public DateTime PlannedAt;
//    //}
    
//    public class HostSessionManager
//    {
//        private readonly IHttpClient _httpClient;
//        private readonly UGSService _ugsService;
//        private readonly MeetingEventManager _meetingEventManager;
//        private RelayHandler _relayHandler;
//        private VivoxHandler _vivoxHandler;
//        private CancellationTokenSource _heartbeatCts;
//        private const int MaxPlayers = 100;
//        private string _channelName;
        

//        public HostSessionManager(IHttpClient httpClient, UGSService ugsService, MeetingEventManager meetingEventManager)
//        {
//            _httpClient = httpClient;
//            _ugsService = ugsService;
//            _meetingEventManager = meetingEventManager ?? throw new ArgumentNullException(nameof(meetingEventManager));
//        }

//        public HostSessionManager(IHttpClient httpClient, MeetingEventManager meetingEventManager) 
//            : this(httpClient, ServiceLocator.GetService<UGSService>(), meetingEventManager)
//        {
//            if (_ugsService == null)
//            {
//                throw new InvalidOperationException("UGSService is not registered in ServiceLocator.");
//            }
//            InitializeAsync().Forget();
//        }

//        private async UniTaskVoid InitializeAsync()
//        {
//            await _ugsService.EnsureReadyAsync();
//            await UniTask.Yield();
//            _relayHandler = _ugsService.GetModule<RelayHandler>();
//            _vivoxHandler = _ugsService.GetModule<VivoxHandler>();

//            //events, callbacks, etc.

//            // get the user info from the auth service so i can set the display name for Vivox
//            // make sure the auth service exists before acccessing it

//            //var authService = ServiceLocator.GetService<IAuthService>();
//            //if (authService != null && authService.IsAuthenticated)
//            //{
//            //    _vivoxHandler.SetDisplayName(authService.UserName);
//            //}
//            //else
//            //{
//            //    _vivoxHandler.SetDisplayName(Environment.UserName);
//            //}


//            await UniTask.CompletedTask;
//        }


//        /// <summary>
//        /// Hosts a new session and joins the Vivox channel.
//        /// If reHost is true, it will notify the backend to rehost the session.
//        /// </summary>
//        private async UniTaskVoid HostSessionAsync(string eventId, bool reHost = false, CancellationToken token = default)
//        {
//            if (string.IsNullOrEmpty(eventId))
//            {
//                ULog.Error("Event ID cannot be null or empty.");
//                return;
//            }
//            try
//            {
//                SessionDetails sessionDetails = await _relayHandler.HostAsync(maxPlayers: MaxPlayers);
//                if (sessionDetails != null && !string.IsNullOrEmpty(sessionDetails.Id))
//                {
//                    _channelName = GetChannelNameFHH(sessionDetails.Code);
//                    await _vivoxHandler.JoinChannelAsync(_channelName);
//                    ULog.Info($"Joined channel: {_channelName}");
                    
//                    await _meetingEventManager.NotifySessionHostedAsync(eventId, sessionDetails.Code, reHost, token);

//                    // Begin new heartbeat using MeetingEventManager
//                    if (_heartbeatCts != null && !_heartbeatCts.IsCancellationRequested)
//                    {
//                        _heartbeatCts.Cancel();
//                    }
//                    _heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(token);
//                    _ = _meetingEventManager.StartHeartbeatLoopAsync(eventId, TimeSpan.FromSeconds(15), _heartbeatCts.Token);
//                }
//                else
//                {
//                    ULog.Error("Failed to host session. Please check the settings.");
//                }
//            }
//            catch (Exception ex)
//            {
//                ULog.Error($"Relay Error: {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// Generates a channel name for the FHH project based on the join code.
//        /// </summary>
//        private string GetChannelNameFHH(string joinCode)
//        {
//            return $"FHH_{joinCode.Trim().ToLowerInvariant()}";
//        }
        
//        public async UniTask EndSessionAsync(string eventId)
//        {
//            if (string.IsNullOrEmpty(eventId))
//            {
//                ULog.Warning("Cannot end session: Event ID is null or empty");
//                return;
//            }

//            _heartbeatCts?.Cancel();
//            await _meetingEventManager.NotifySessionEndedAsync(eventId);
//        }
//    }

//    public class ParticipantSessionListener
//    {
//        private readonly IHttpClient _httpClient;
//        private readonly RelayHandler _relay;
//        private readonly VivoxHandler _vivox;
//        private IDisposable _subscription;
//        private string _currentCode;
//        private CancellationTokenSource _cancellationTokenSource;


//        public ParticipantSessionListener(IHttpClient httpClient, RelayHandler relay, VivoxHandler vivox)
//        {
//            _httpClient = httpClient;
//            _relay = relay;
//            _vivox = vivox;
//        }

//        public async UniTask ListenAsync(CancellationToken token = default)
//        {
//            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
//            try
//            {
//                _subscription = await _httpClient.SubscribeAsync(
//                    "/events",
//                    headers: new Dictionary<string, string> { { "Accept", "text/event-stream" } },
//                    onData: OnEvent,
//                    cancellationToken: _cancellationTokenSource.Token
//                );
                
//                ULog.Info("SSE connection established successfully");
//            }
//            catch (Exception ex)
//            {
//                ULog.Error($"Failed to establish SSE connection: {ex.Message}");
//                throw;
//            }
//        }

//        private void OnEvent(string json)
//        {
//            try
//            {
//                // Handle heartbeat responses
//                if (json.Trim() == ":ok")
//                {
//                    ULog.Info("Received heartbeat confirmation");
//                    return;
//                }

//                // Try to parse as array first (for bulk updates)
//                Meeting[] events = null;
//                try
//                {
//                    events = JsonConvert.DeserializeObject<Meeting[]>(json);
//                }
//                catch
//                {
//                    // If array parsing fails, try single event
//                    var singleEvent = JsonConvert.DeserializeObject<Meeting>(json);
//                    if (singleEvent != null)
//                    {
//                        events = new Meeting[] { singleEvent };
//                    }
//                }

//                if (events != null)
//                {
//                    foreach (var ev in events)
//                    {
//                        if (!string.IsNullOrEmpty(ev.JoinCode) && ev.JoinCode != _currentCode)
//                        {
//                            _currentCode = ev.JoinCode;
//                            HandleMeetingJoinAsync(ev).Forget();
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ULog.Error($"Error processing event: {ex.Message}. JSON: {json}");
//            }
//        }

//        private async UniTaskVoid HandleMeetingJoinAsync(Meeting meeting)
//        {
//            try
//            {
//                await _relay.JoinAsync(_currentCode);
//                await _vivox.JoinChannelAsync($"ev-{meeting.Id}");
//                ULog.Info($"Successfully joined meeting {meeting.Id} with code {_currentCode}");
//            }
//            catch (Exception ex)
//            {
//                ULog.Error($"Failed to join meeting {meeting.Id}: {ex.Message}");
//            }
//        }

//        public void Stop()
//        {
//            _cancellationTokenSource?.Cancel();
//            _subscription?.Dispose();
//            _cancellationTokenSource?.Dispose();
//        }
//    }
//}