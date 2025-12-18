using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Components.Collaboration;
using FHH.Logic.Components.Networking;
using FHH.UI.Chat;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System;
using System.Threading;
using Foxbyte.Core.Localization.Utilities;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace FHH.UI.MeetingBar
{
    public sealed class MeetingBarPresenter 
        : PresenterBase<MeetingBarPresenter, MeetingBarView, MeetingBarModel>
    {
        private Texture2D _adaptiveOn;
        private Texture2D _adaptiveOff;
        private Texture2D _micOn;
        private Texture2D _micOff;

        // service + roles + collab lifetime
        private CollaborationService _collab;
        private PermissionService _perm;
        private CancellationTokenSource _collabCts; // cancels when leaving as client
        private readonly SemaphoreSlim _cleanupGate = new(1, 1);
        private bool _leaving;
        private int _cleanupStarted;

        //loading animation
        private const int LoadingBarMaxChars = 14;
        private const char LoadingBarChar = 'I'; // looks better than '|', needs Arial
        private int _loadingStep;
        private bool _isTimerLoopRunning;
        

        public MeetingBarPresenter(
            GameObject targetGameobjectForView,
            UIDocument uiDoc,
            StyleSheet styleSheet,
            VisualElement target,
            MeetingBarModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameobjectForView, uiDoc, styleSheet, target, model ?? new MeetingBarModel(), perms)
        { }

        private MeetingEvent _currentMeetingEvent;

        public override async UniTask InitializeBeforeUiAsync()
        {
            _collab ??= ServiceLocator.GetService<CollaborationService>();
            _perm ??= ServiceLocator.GetService<PermissionService>();

            _adaptiveOff = Resources.Load<Texture2D>("Icons/MeetingBar/adaptive_audio_mic");
            _adaptiveOn = Resources.Load<Texture2D>("Icons/MeetingBar/adaptive_audio_mic_off"); // enabled/on = all muted
            _micOff = Resources.Load<Texture2D>("Icons/MeetingBar/mic"); 
            _micOn = Resources.Load<Texture2D>("Icons/MeetingBar/mic_off"); // enabled/on = muted

            _perm.UserBecameAnonymous += OnUserBecameAnonymous;

            await Model.InitializeAsync();
            //if (NetworkManager.Singleton == null)
            //{
            //    if (SceneManager.GetSceneByName("Multiplayer").isLoaded) return;
            //    await SceneManager.LoadSceneAsync("Multiplayer", LoadSceneMode.Additive);
            //}
        }

        public override async UniTask InitializeWithDataBeforeUiAsync<T>(T data)
        {
            await UniTask.Yield();
            if (data is MeetingEvent meetingEvent)
            {
                _currentMeetingEvent = meetingEvent;
                Model.SetEvent(meetingEvent.Id);
            }
        }

        private void OnUserBecameAnonymous(object sender, EventArgs e)
        {
            LeaveAndHideAsync().Forget(ex => Debug.LogError("Could not leave when User changed to Anonymous."));
        }
        
        public override async UniTask InitializeAfterUiAsync()
        {
            DisableActionButtonsExceptLeave();
            _cleanupStarted = 0;
            _leaving = false;
            _isTimerLoopRunning = false;
            // initial UI state (empty until service drives it)
            var isGerman = ServiceLocator.GetService<LocaleSwitcher>().IsTwoLetterLanguage("de");
            View.TitleLabel.text = isGerman ? "Verbinde" : "Connecting";
            //View.SessionTimerLabel.text = "00:00:00";
            View.SessionTimerLabel.text = "";
            //SetStatusChip("PLANNED");

            // callbacks
            View.GuidedAccessButton.clicked += OnClickGuidedAccess;
            View.ChatButton.clicked += OnClickChat;
            View.AdaptiveMicButton.clicked += OnClickAdaptiveMic;
            View.MicButton.clicked += OnClickMic;
            View.LeaveButton.clicked += OnClickLeave;

            // subscribe to service events; currently unused as we don't use Netcode/Relay
            //_collab.ClientDisconnected += OnClientDisconnected;
            //_collab.MissingHost += OnMissingHost;
            //_collab.ClientSessionActive += OnClientSessionActive;
            //_collab.SessionEnded += OnSessionEnded;
            //_collab.ClientStatusUpdate += OnClientStatusUpdate;
            //_collab.HostSessionStarting += OnHostSessionStarting;
            //_collab.HostSessionReady += OnHostSessionReady;

            _collab.VivoxConnected += OnVivoxConnected;

            StartTimerLoopIfNeeded();

            await UniTask.Yield();
            StartCollabAsync().Forget();
        }

        private void StartTimerLoopIfNeeded()
        {
            if (_isTimerLoopRunning) return;
            _isTimerLoopRunning = true;
            RunWithUiLifetimeAsync(UpdateClockLoopAsync).Forget();
        }

        private async UniTask StartCollabAsync()
        {
            await UniTask.WaitForEndOfFrame();
            await RunWithUiLifetimeAsync(StartCollaborationLoopAsync);
            
        }

        //  Ensure buttons reflect moderator role (service will also adjust roles via SSE)
        private void EnforceRoleGates()
        {
            bool isMod = _perm != null && _perm.IsModerator();
            View.GuidedAccessButton.SetEnabled(isMod);
            View.AdaptiveMicButton.SetEnabled(isMod);
        }

        // Collaboration bootstrap: fetch events & call CollaborateAsync; stays alive until view unmounts
        private async UniTask StartCollaborationLoopAsync(CancellationToken ct)
        {
            //var events = await _collab.GetMeetingEventsAsync(ct);
            //var ev = events?.Find(e => !string.Equals(e?.Status, "END", StringComparison.OrdinalIgnoreCase));
            var ev = _currentMeetingEvent;
            if (ev == null) return;

            Model.SetEvent(ev.Id);
            Model.SetProjectInfo(ev.ProjectId, ev.Title);
            //View.TitleLabel.text = ev.Title;

            // Start/Join session; lifetime = view mount (MountToken is included via UiScopeCts/CreateLinkedCts)
            _collabCts?.Cancel();
            _collabCts?.Dispose();
            _collabCts = CreateLinkedCts(ct); // linked with View.MountToken via base helpers

            try
            {
                await _collab.CollaborateAsync(ev, _collabCts.Token);
            }
            catch (OperationCanceledException) { /* expected on view close or leave */ }
        }

        private async UniTask UpdateClockLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                UpdateTimerLabel();
                var delay = Model.HasSessionStarted
                    ? TimeSpan.FromSeconds(1)
                    : TimeSpan.FromSeconds(0.2); // faster while loading

                await UniTask.Delay(delay, cancellationToken: ct);
            }
        }

        private void UpdateTimerLabel()
        {
            if (View == null) return;
            if (!Model.HasSessionStarted)
            {
                // simple 1..N bar that loops
                _loadingStep = (_loadingStep % LoadingBarMaxChars) + 1;
                View.SessionTimerLabel.text = new string(LoadingBarChar, _loadingStep);
                return;
            }
            var t = Model.GetSessionDuration();
            if (View != null)
                View.SessionTimerLabel.text = $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
        }

        
        private void OnVivoxConnected()
        {
            Model.StartSessionClock();
            View.TitleLabel.text = _currentMeetingEvent.Title;
            // tick while mounted
            //RunWithUiLifetimeAsync(UpdateClockLoopAsync).Forget();
            StartTimerLoopIfNeeded();
            EnableActionButtonsByRole();
            RefreshAdaptiveMicIcon();
            RefreshMicIcon();
        }

        // Service Event Handlers

        private void OnClientDisconnected()
        {
            //SetStatusChip("DISCONNECTED");
            Model.StopSessionClock();
            View.SessionTimerLabel.style.opacity = 0f;
            UpdateTimerLabel();
            DisableActionButtonsExceptLeave();
        }

        private void OnMissingHost()
        {
            //SetStatusChip("MISSING_HOST");
            DisableActionButtonsExceptLeave();
        }

        private void OnClientSessionActive()
        {
            //SetStatusChip("ACTIVE");
            Model.StartSessionClock();
            View.TitleLabel.text = _currentMeetingEvent.Title;
            // tick while mounted
            //RunWithUiLifetimeAsync(UpdateClockLoopAsync).Forget();
            StartTimerLoopIfNeeded();
            EnableActionButtonsByRole();
            RefreshAdaptiveMicIcon();
            RefreshMicIcon();
        }

        private void OnSessionEnded()
        {
            //if (_leaving) return;
            //SetStatusChip("END");
            Model.StopSessionClock();
            View.SessionTimerLabel.style.opacity = 0f;
            UpdateTimerLabel();
            DisableActionButtonsExceptLeave();
            //CloseViewAndCleanupAsync().Forget(ex => Debug.LogError($"MeetingBar Close on SessionEnded failed: {ex.Message}"));
        }

        private void OnClientStatusUpdate(string status, string joinCode)
        {
            Model.SetStatus(status);
            // chip mirrors status but we already handle via specific events above; keep it in sync:
            //SetStatusChip(status);
        }

        private void OnHostSessionStarting(MeetingEvent e)
        {
            DisableActionButtonsExceptLeave();
        }

        private void OnHostSessionReady(string joinCode)
        {
            Model.StartSessionClock();
            View.TitleLabel.text = _currentMeetingEvent.Title;
            //RunWithUiLifetimeAsync(UpdateClockLoopAsync).Forget();
            StartTimerLoopIfNeeded();
            EnableActionButtonsByRole();
            RefreshAdaptiveMicIcon();
            RefreshMicIcon();
        }

        private void EnableActionButtonsByRole()
        {
            View.ChatButton.SetEnabled(true);
            View.MicButton.SetEnabled(true);
            EnforceRoleGates(); // GuidedAccess + AdaptiveMic gated by moderator
        }

        private void DisableActionButtonsExceptLeave()
        {
            if (View == null) return;
            View.GuidedAccessButton.SetEnabled(false);
            View.ChatButton.SetEnabled(false);
            View.AdaptiveMicButton.SetEnabled(false);
            View.MicButton.SetEnabled(false);
        }

        //private void SetStatusChip(string statusUpper)
        //{
        //    if (View.StatusLabel == null) return;
        //    var s = (statusUpper ?? "PLANNED").ToUpperInvariant();
        //    View.StatusLabel.text = s; // plain text; no join code

        //    // simple class swapping (kept minimal)
        //    View.StatusLabel.RemoveFromClassList("active");
        //    View.StatusLabel.RemoveFromClassList("missing-host");
        //    View.StatusLabel.RemoveFromClassList("planned");
        //    View.StatusLabel.RemoveFromClassList("ended");

        //    switch (s)
        //    {
        //        case "ACTIVE": View.StatusLabel.AddToClassList("active"); break;
        //        case "MISSING_HOST": View.StatusLabel.AddToClassList("missing-host"); break;
        //        case "END": View.StatusLabel.AddToClassList("ended"); break;
        //        default: View.StatusLabel.AddToClassList("planned"); break;
        //    }
        //}

        //  Buttons 

        private void OnClickGuidedAccess()
        {
            if (!Allow(View.GuidedAccessButton)) return;
            if (!_perm.IsModerator()) return; // gate
            //Debug.Log("Guided Access clicked.");
            Model.ToggleGuidedAccess();
            RefreshGuidedAccessButton();
            _collab.IsGuidedModeEnabled = Model.IsGuidedAccessOn;
            //send command
            var json = JsonUtility.ToJson(new CommandPayload
            {
                Type = "GuidedMode",
                ProjectId = Model.ProjectId,
                //Value = Model.IsGuidedAccessOn,
                Enabled = Model.IsGuidedAccessOn
            });
            var vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();
            vivoxHandler.SendCommandMessageAsync(json, _collab.CommandChannelName).Forget();
        }

        private void OnClickChat()
        {
            if (!Allow(View.ChatButton)) return;
            OpenChatAsync().Forget();
            //Debug.Log("Chat clicked.");
        }

        private async UniTask OpenChatAsync()
        {
            var uiManager = ServiceLocator.GetService<UIManager>();

            // toggle
            if (uiManager.IsShown<ChatPresenter>())
            {
                await uiManager.HideAsync<ChatPresenter>();
                return;
            }

            uiManager.ShowWindowAsync<
                ChatPresenter,
                ChatView,
                ChatModel
            >(
                new ChatModel(),
                new WindowOptions
                {
                    Region = UIProjectContext.UIRegion.Content1,
                    CenterScreen = false,
                    FadeInSec = 0.15f,
                    FadeOutSec = 0.15f,
                    StyleSheet = Resources.Load<StyleSheet>("Chat"),
                    CloseOthersInRegion = true //close other content1 windows
                }
            ).Forget();
        }

        private void OnClickAdaptiveMic()
        {
            if (!Allow(View.AdaptiveMicButton)) return;
            if (!_perm.IsModerator()) return; // gate
            Model.ToggleAdaptiveMic();
            RefreshAdaptiveMicIcon();
            //send command
            var json = JsonUtility.ToJson(new CommandPayload
            {
                Type = "MuteAll",
                Enabled = Model.IsAdaptiveMicOn,
                //Value = Model.IsAdaptiveMicOn
            });
            var vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();
            vivoxHandler.SendCommandMessageAsync(json, _collab.CommandChannelName).Forget();
        }

        private void OnClickMic()
        {
            if (!Allow(View.MicButton)) return;
            Model.ToggleMic();
            var result = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>().ToggleMuteParticipantLocally(Model.IsMicOn);
            if (!result)
            {
                ULog.Error("Failed to toggle microphone via Vivox.");
                Model.ToggleMic();
            }
            RefreshMicIcon();
        }

        private void OnClickLeave()
        {
            if (!Allow(View.LeaveButton, 1d)) return;
            if (_leaving) return;
            _leaving = true;
            View.LeaveButton.SetEnabled(false);
            //Debug.Log("Leave button clicked.");
            //LeaveClickedAsync().Forget();
            //CloseFromUserAsync().Forget(ex => Debug.LogError($"MeetingBar CloseFromUser failed: {ex.Message}"));
            LeaveAndHideAsync().Forget(ex => Debug.LogError($"MeetingBar LeaveAndHide failed: {ex.Message}"));
        }

        private async UniTask LeaveAndHideAsync()
        {
            try
            {
                //try { _collab?.StopSession(); } catch { }
                try { _collabCts?.Cancel(); } catch { }

                UnSubscribeEvents();
                OnSessionEnded();
                await CloseViewAndCleanupAsync();

                if (_perm.IsOwner() && !string.IsNullOrWhiteSpace(Model.EventId))
                {
                    try
                    {
                        _collab.EndSessionAsync(Model.EventId, CancellationToken.None)
                            .Forget(ex => Debug.LogWarning($"EndSession async failed: {ex.Message}"));
                    }
                    catch
                    {
                        ULog.Error("Could not end session.");
                    }
                }
            }
            finally
            {
                await UniTask.DelayFrame(1);
                _leaving = false;
            }
        }

        private void RefreshAdaptiveMicIcon()
        {
            var img = View.AdaptiveMicButton.Q<Image>("AdaptiveMicButton_Img");
            if (img == null) return;
            img.image = Model.IsAdaptiveMicOn ? _adaptiveOn : _adaptiveOff;
            View.AdaptiveMicButton.EnableInClassList("is-on", Model.IsAdaptiveMicOn);
        }

        private void RefreshMicIcon()
        {
            var img = View.MicButton.Q<Image>("MicButton_Img");
            if (img == null) return;
            img.image = Model.IsMicOn ? _micOn : _micOff;
            View.MicButton.EnableInClassList("is-on", Model.IsMicOn);
        }

        private void RefreshGuidedAccessButton()
        {
            //View.GuidedAccessButton.EnableInClassList("is-on", Model.IsGuidedAccessOn);
            View.GuidedAccessButton.EnableInClassList("is-enabled", Model.IsGuidedAccessOn);
            View.GuidedAccessButton.Q<Image>("GuidedAccessButton_Img").EnableInClassList("is-enabled", Model.IsGuidedAccessOn);
        }

        // Teardown 

        private async UniTask CloseViewAndCleanupAsync()
        {
            await _cleanupGate.WaitAsync();
            var uiManager = ServiceLocator.GetService<UIManager>();
            try
            {
                if (Interlocked.Exchange(ref _cleanupStarted, 1) == 1)
                {
                    // already cleaned this mount; still ensure hide removes any residuals
                    await uiManager.HideAsync(typeof(MeetingBarPresenter), 0.1f);
                    if (uiManager.IsShown<ChatPresenter>())
                    {
                        await uiManager.HideAsync<ChatPresenter>();
                    }
                    return;
                }

                if (View == null)
                {
                    await uiManager.HideAsync(typeof(MeetingBarPresenter), 0.1f);
                    if (uiManager.IsShown<ChatPresenter>())
                    {
                        await uiManager.HideAsync<ChatPresenter>();
                    }
                    return;
                }
                
                try { _collab?.StopSession(); } catch { }
                await uiManager.HideAsync(typeof(MeetingBarPresenter), 0.1f);
                if (uiManager.IsShown<ChatPresenter>())
                {
                    await uiManager.HideAsync<ChatPresenter>();
                }
                //MarkUnmounted();
            }
            finally
            {
                _cleanupGate.Release();
            }
        }

        private void UnSubscribeEvents()
        {
            if (_collab != null)
            {
                _collab.VivoxConnected -= OnVivoxConnected;
                //_collab.ClientDisconnected -= OnClientDisconnected;
                //_collab.MissingHost -= OnMissingHost;
                //_collab.ClientSessionActive -= OnClientSessionActive;
                //_collab.SessionEnded -= OnSessionEnded;
                //_collab.ClientStatusUpdate -= OnClientStatusUpdate;
                //_collab.HostSessionStarting -= OnHostSessionStarting;
                //_collab.HostSessionReady -= OnHostSessionReady;
            }
        }

        public override void Dispose()
        {
            if (_perm != null) _perm.UserBecameAnonymous -= OnUserBecameAnonymous;
            CloseViewAndCleanupAsync().Forget(ex => Debug.LogError($"MeetingBar Dispose close failed: {ex.Message}"));
            UnSubscribeEvents();
            base.Dispose();
        }
    }
}