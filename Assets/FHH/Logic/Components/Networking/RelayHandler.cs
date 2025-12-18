using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;

namespace FHH.Logic.Components.Networking
{
    /// <summary>
    /// Encapsulates Relay‐specific functionality
    /// </summary>
    public sealed class RelayHandler : IUGSModule
    {
        private bool _isInitialized;
        private ISession _activeSession;
        private const string PlayerNameKey = "playerName";
        private const string IsHostKey = "isHost";

        public ISession ActiveSession
        {
            get => _activeSession;
            private set
            {
                _activeSession = value;
                if (_activeSession != null)
                {
                    ULog.Info($"Active session set: {_activeSession.Code}");
                }
                else
                {
                    ULog.Info("Active session cleared.");
                }
            }
        }

        public async UniTask InitAsync(UGSService context)
        {
            if (_isInitialized) return;
            await context.EnsureReadyAsync();
            _isInitialized = true;
        }

        private void RegisterSessionEvents()
        {
            ActiveSession.Changed += OnSessionChanged;
            ActiveSession.StateChanged += OnSessionStateChanged;
            ActiveSession.PlayerJoined += OnPlayerJoined;
            ActiveSession.PlayerLeaving += OnPlayerLeaving;
            ActiveSession.Deleted += OnSessionDeleted;
            ActiveSession.RemovedFromSession += OnRemovedFromSession;
        }

        private void UnregisterSessionEvents()
        {
            if (ActiveSession == null) return;
            ActiveSession.Changed -= OnSessionChanged;
            ActiveSession.StateChanged -= OnSessionStateChanged;
            ActiveSession.PlayerJoined -= OnPlayerJoined;
            ActiveSession.PlayerLeaving -= OnPlayerLeaving;
            ActiveSession.Deleted -= OnSessionDeleted;
            ActiveSession.RemovedFromSession -= OnRemovedFromSession;
        }

        private void OnSessionChanged()
        {
            ULog.Info($"Session changed: {ActiveSession.Id} - {ActiveSession.Code}");
            if (ActiveSession.IsHost)
            {
                ULog.Info("You are the host of this session.");
            }
            else
            {
                ULog.Info("You are a player in this session.");
            }
        }

        private void OnSessionStateChanged(SessionState sessionState)
        {
            ULog.Info($"Session state changed: {sessionState}");
            switch (sessionState)
            {
                case SessionState.Connected:
                    ULog.Info("Session is now connected.");
                    break;
                case SessionState.Disconnected:
                    ULog.Info("Session has been disconnected.");
                    break;
                case SessionState.Deleted:
                    ULog.Info("Session has been deleted.");
                    break;
                case SessionState.None:
                    ULog.Info("Session is in an undefined state.");
                    break;
                default:
                    ULog.Info($"Session state changed to: {sessionState}");
                    break;
            }
        }

        private void OnPlayerJoined(string player)
        {
            ULog.Info($"Player {player} joined session {ActiveSession.Id}");
        }

        private void OnPlayerLeaving(string player)
        {
            ULog.Info($"Player {player} is leaving session {ActiveSession.Id}");
        }

        private void OnSessionDeleted()
        {
            ULog.Info("Session deleted on server. Cleaning up locally.");
            CleanupLocalSessionState();
        }

        private void OnRemovedFromSession()
        {
            ULog.Info("Removed from session. Cleaning up locally.");
            CleanupLocalSessionState();
        }

        private async UniTask<Dictionary<string, PlayerProperty>> GetPlayerPropertiesAsync()
        {
            //var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            var playerName = PlayerNameUtility.ToAuthName(ServiceLocator.GetService<PermissionService>().GetUser().DisplayName);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
            var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
            var isHost = NetworkManager.Singleton.IsHost.ToString();
            var isHostProperty = new PlayerProperty(isHost, VisibilityPropertyOptions.Member);
            return new Dictionary<string, PlayerProperty>
            {
                { PlayerNameKey, playerNameProperty },
                { IsHostKey, isHostProperty }
            };
        }

        /// <summary>
        /// Creates a host session and returns the ID.
        /// </summary>
        public async UniTask<SessionDetails> HostAsync(int maxPlayers, string protocol = "udp")
        {
            var playerProperties = await GetPlayerPropertiesAsync();
            ULog.Info(
                $"Player properties: {string.Join(", ", playerProperties.Select(kvp => $"{kvp.Key}: {kvp.Value.Value}"))}");

            var options = new SessionOptions
            {
                MaxPlayers = maxPlayers,
                IsLocked = false,
                IsPrivate = true,
                PlayerProperties = playerProperties
            //}.WithRelayNetwork(protocol);
            }.WithRelayNetwork<SessionOptions>();

            ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            if (ActiveSession == null || string.IsNullOrWhiteSpace(ActiveSession.Code))
            {
                ULog.Error("Failed to create session or retrieve join code.");
                return null;
            }

            ULog.Info($"Session {ActiveSession.Id} created with code: {ActiveSession.Code}");

            RegisterSessionEvents();

            return new SessionDetails(){Id = ActiveSession.Id, Code = ActiveSession.Code};
        }

        /// <summary>
        /// Joins an existing session by ID or join code.
        /// </summary>
        public async UniTask<SessionDetails> JoinAsync(string id = null, string joinCode = null, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            bool hasId  = !string.IsNullOrWhiteSpace(id);
            bool hasJoinCode = !string.IsNullOrWhiteSpace(joinCode);
            if (hasId == hasJoinCode)
            {
                ULog.Error("You must supply exactly one of session ID or join code.");
                return null;
            }
            ISession activeSession = null;
            try
            {
                if (!hasId)
                {
                    activeSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
                    if (activeSession == null)
                    {
                        ULog.Error($"No session found for join code: {joinCode}");
                        return null;
                    }
                }
                else
                {
                    activeSession = await MultiplayerService.Instance.JoinSessionByIdAsync(id);
                    if (activeSession == null)
                    {
                        ULog.Error($"Failed to join session with ID: {id}");
                        return null;
                    }
                }
                ct.ThrowIfCancellationRequested();
                ActiveSession = activeSession;  
                ULog.Info($"Joined session {activeSession.Id} (code: {activeSession.Code})");
                RegisterSessionEvents();
                return new SessionDetails(){Id = activeSession.Id, Code = activeSession.Code};
            }
            catch (OperationCanceledException)
            {
                ULog.Info("Join was canceled.");
                throw; 
            }
        }


        /// <summary>
        /// Kick a player from the session by their ID.
        /// </summary>
        public async UniTask<bool> KickPlayerAsync(string playerId)
        {
            if (!ActiveSession.IsHost) return false;
            if (string.IsNullOrWhiteSpace(playerId))
            {
                ULog.Error("Player ID cannot be null or empty.");
                return false;
            }

            await ActiveSession.AsHost().RemovePlayerAsync(playerId);

            ULog.Info($"Player {playerId} kicked from session {ActiveSession.Id}");
            return true;
        }

        /// <summary>
        /// Queries all active sessions and returns a list of session info.
        /// </summary>
        public async UniTask<IList<ISessionInfo>> QuerySessionsAsync()
        {
            var sessionQueryOptions = new QuerySessionsOptions();
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(sessionQueryOptions);
            if (results == null || results.Sessions == null || !results.Sessions.Any())
            {
                ULog.Warning("No sessions found.");
                return new List<ISessionInfo>();
            }

            return results.Sessions;
        }

        private void CleanupLocalSessionState()
        {
            UnregisterSessionEvents(); // safe if already unregistered or null
            ActiveSession = null;
            //if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            //{
            //    NetworkManager.Singleton.Shutdown();
            //}
            ULog.Info("Local session and Netcode state cleaned up.");
        }

        /// <summary>
        /// Leave the current session (host or client).
        /// If endForAllIfHost is true and we are the host, the session is deleted for everyone.
        /// </summary>
        public async UniTask<bool> LeaveSessionAsync(bool endForAllIfHost = false)
        {
            if (ActiveSession == null)
            {
                ULog.Warning("LeaveSessionAsync called but there is no active session.");
                return false;
            }

            try
            {
                if (endForAllIfHost && ActiveSession.IsHost)
                {
                    ULog.Info("Host ending session for all players.");
                    await ActiveSession.AsHost().DeleteAsync(); // ends session globally
                }
                else
                {
                    ULog.Info("Leaving session.");
                    await ActiveSession.LeaveAsync();// only this player leaves
                }
            }
            catch (Exception e)
            {
                ULog.Warning($"Error while leaving/ending session: {e.Message}");
            }
            CleanupLocalSessionState();
            return true;
        }

        /// <summary>
        /// Host-only helper to explicitly end the current session for all players.
        /// </summary>
        public async UniTask<bool> EndSessionAsync()
        {
            if (ActiveSession == null)
            {
                ULog.Warning("EndSessionAsync called but there is no active session.");
                return false;
            }

            if (!ActiveSession.IsHost)
            {
                ULog.Warning("EndSessionAsync called by non-host; leaving instead.");
                await LeaveSessionAsync(false);
                return false;
            }

            ULog.Info("Host deleting session for all players.");

            try
            {
                await ActiveSession.AsHost().DeleteAsync();
            }
            catch (Exception e)
            {
                ULog.Warning($"Error while deleting session: {e.Message}");
            }
            CleanupLocalSessionState();
            return true;
        }

        
        public async UniTask DisposeAsync()
        {
            if (!_isInitialized) return;
            try
            {
                // leave the session first (frees Lobby & Relay allocation)
                if (_activeSession != null)
                {
                    await LeaveSessionAsync(true);
                    UnregisterSessionEvents();
                }

                if (NetworkManager.Singleton.IsListening)
                    NetworkManager.Singleton.Shutdown();

                ULog.Info("Events, Session, Relay, and Netcode cleaned up.");
            }
            catch (Exception e)
            {
                ULog.Warning($"Cleanup issues: {e.Message}");
            }

            _isInitialized = false;
            await UniTask.CompletedTask;
        }
    }

    public class SessionDetails
    {
        public string Id { get; set; }
        public string Code { get; set; }
    }
}