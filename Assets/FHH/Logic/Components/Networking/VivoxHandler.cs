using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foxbyte.Core;
using Unity.Services.Vivox;

namespace FHH.Logic.Components.Networking
{
    public sealed class VivoxHandler : IUGSModule
    {
        private bool _isInitialized;
        private readonly LoginOptions _loginOptions;
        public string CurrentTextChannel;
        public string CurrentCommandChannel;
        private readonly Dictionary<string, List<VivoxParticipant>> _participantsByChannel = new();
        private bool _isForceMuted;
        //private bool _isLocalMuted;

        // rate limiter
        private readonly List<QueuedCommandMessage> _commandMessageQueue = new();
        private readonly Dictionary<string, QueuedCommandMessage> _latestCommandMessageByType = new();
        private bool _isProcessingCommandQueue;
        private DateTime _lastCommandSentUtc;

        public VivoxHandler(string displayName = null, bool enableTTS = false)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = Environment.UserName;
            }

            _loginOptions = new LoginOptions
            {
                DisplayName = displayName,
                EnableTTS = enableTTS
            };
        }

        public async UniTask InitAsync(UGSService context)
        {
            if (_isInitialized) return;
            await context.EnsureReadyAsync();
            await VivoxService.Instance.InitializeAsync();
            await VivoxService.Instance.LoginAsync(_loginOptions);
            
            VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
            VivoxService.Instance.ParticipantAddedToChannel += OnParticipantAddedToChannel;
            VivoxService.Instance.ParticipantRemovedFromChannel += OnParticipantRemovedFromChannel;
            
            _isInitialized = true;
        }

        private void OnChannelMessageReceived(VivoxMessage message)
        {
            ULog.Info($"Message received in channel {message.ChannelName}: {message.MessageText}");
        }

        private void OnParticipantAddedToChannel(VivoxParticipant participant)
        {
            if (!_participantsByChannel.ContainsKey(participant.ChannelName))
            {
                _participantsByChannel[participant.ChannelName] = new List<VivoxParticipant>();
            }

            _participantsByChannel[participant.ChannelName].Add(participant);
            ULog.Info($"Participant {participant.DisplayName} added to channel {participant.ChannelName}");
        }

        private void OnParticipantRemovedFromChannel(VivoxParticipant participant)
        {
            if (_participantsByChannel.ContainsKey(participant.ChannelName))
            {
                _participantsByChannel[participant.ChannelName].Remove(participant);
                if (_participantsByChannel[participant.ChannelName].Count == 0)
                {
                    _participantsByChannel.Remove(participant.ChannelName);
                }
            }

            ULog.Info($"Participant {participant.DisplayName} removed from channel {participant.ChannelName}");
        }


        public async UniTask DisposeAsync()
        {
            if (!_isInitialized) return;
            try
            {
                // leave current channel if joined
                if (!string.IsNullOrEmpty(CurrentTextChannel))
                {
                    await VivoxService.Instance.LeaveChannelAsync(CurrentTextChannel);
                    CurrentTextChannel = null;
                }

                if (!string.IsNullOrEmpty(CurrentCommandChannel))
                {
                    await VivoxService.Instance.LeaveChannelAsync(CurrentCommandChannel);
                    CurrentCommandChannel = null;
                    VivoxService.Instance.ChannelMessageReceived -= OnChannelMessageReceived;
                    VivoxService.Instance.ParticipantAddedToChannel -= OnParticipantAddedToChannel;
                    VivoxService.Instance.ParticipantRemovedFromChannel -= OnParticipantRemovedFromChannel;
                }

                await VivoxService.Instance.LogoutAsync();
                _isInitialized = false;
                ULog.Info("Vivox cleanup completed successfully.");
            }
            catch (Exception ex)
            {
                ULog.Error($"Error during Vivox cleanup: {ex.Message}");
            }
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Join a non-positional (non-spatialized) group channel by name.
        /// </summary>
        public async UniTask JoinChannelAsync(string channelName, ChatCapability capability = ChatCapability.TextAndAudio)
        {
            if (string.IsNullOrWhiteSpace(channelName))
                ULog.Error("Channel cannot be empty.");
            await VivoxService.Instance.JoinGroupChannelAsync(channelName, capability,
                new ChannelOptions { MakeActiveChannelUponJoining = true });
            CurrentTextChannel = channelName;
            _isForceMuted = false;
            //_isLocalMuted = false;
        }

        public async UniTask JoinCommandChannelAsync(string commandChannelName)
        {
            if (string.IsNullOrWhiteSpace(commandChannelName))
                ULog.Error("Command channel cannot be empty.");
            await VivoxService.Instance.JoinGroupChannelAsync(commandChannelName, ChatCapability.TextOnly,
                new ChannelOptions { MakeActiveChannelUponJoining = true });
            CurrentCommandChannel = commandChannelName;
        }

        /// <summary>
        /// Leave the specified channel (or current channel if null).
        /// </summary>
        public async UniTask LeaveChannelAsync(string channelName = null)
        {
            var toLeave = channelName;
            if (string.IsNullOrEmpty(toLeave))
                return;
            await VivoxService.Instance.LeaveChannelAsync(toLeave);
            if (_participantsByChannel.ContainsKey(toLeave))
            {
                _participantsByChannel.Remove(toLeave);
            }
            if (toLeave == CurrentTextChannel) CurrentTextChannel = null;
            if (toLeave == CurrentCommandChannel) CurrentCommandChannel = null; 
        }

        /// <summary>
        /// Send a text message to the current channel or specified channel.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public async UniTask SendTextMessageAsync(string text, string channelName = null)
        {
            var targetChannel = channelName ?? CurrentTextChannel;
            if (string.IsNullOrEmpty(targetChannel))
            {
                ULog.Error("Cannot send message: no channel joined.");
                return;
            }

            await VivoxService.Instance.SendChannelTextMessageAsync(targetChannel, text);
        }

        public async UniTask SendCommandMessageAsync(string text, string commandChannelName)
        {
            if (string.IsNullOrEmpty(commandChannelName))
            {
                ULog.Error("Cannot send command message: command channel name is empty.");
                //ServiceLocator.GetService<UIManager>().ShowNotification("Error: Could not send command to channel: "+commandChannelName, 4f);
                return;
            }
            EnqueueCommandMessage(text, commandChannelName);
            //await VivoxService.Instance.SendChannelTextMessageAsync(commandChannelName, text);
            //ServiceLocator.GetService<UIManager>().ShowNotification("Success: Sent command to channel: "+commandChannelName, 4f);
            await UniTask.CompletedTask;
        }

        public void RegisterTextMessageHandler(Action<VivoxMessage> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            VivoxService.Instance.ChannelMessageReceived += handler;
        }


        /// <summary>
        /// Toggle mute state of the local participant in the current channel.
        /// </summary>
        /// <param name="shouldBeMuted"></param>
        /// <returns></returns>
        public bool ToggleMuteParticipantLocally(bool shouldBeMuted)
        {
            if (_isForceMuted)
            {
                ULog.Info("Participant is force muted; cannot toggle local mute state.");
                return false;
            }

            var targetChannel = CurrentTextChannel;
            if (string.IsNullOrEmpty(targetChannel))
            {
                ULog.Error("Cannot mute participant: no channel joined.");
                return false;
            }

            // Using Vivox' authoritative view of participants
            if (!VivoxService.Instance.ActiveChannels.TryGetValue(targetChannel, out var participants) ||
                participants == null || participants.Count == 0)
            {
                ULog.Error($"Vivox reports no active participants for channel {targetChannel}");
                return false;
            }

            // IsSelf to find the local participant instead of PlayerId
            VivoxParticipant participant = null;
            foreach (var p in participants)
            {
                if (p == null)
                {
                    continue;
                }

                if (!p.IsSelf)
                {
                    continue;
                }

                participant = p;
                break;
            }
            if (participant == null)
            {
                ULog.Error($"Local Vivox participant (IsSelf) not found in channel {targetChannel}");
                return false;
            }

            try
            {
                if (participant.IsMuted)
                {
                    participant.UnmutePlayerLocally();
                    ULog.Info($"Unmuted local participant {participant.DisplayName} in channel {targetChannel}");
                }
                else
                {
                    participant.MutePlayerLocally();
                    ULog.Info($"Muted local participant {participant.DisplayName} in channel {targetChannel}");
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"Error toggling local mute for participant {participant.DisplayName} in channel {targetChannel}: {ex.Message}");
                return false;
            }

            return true;
        }

        public void ForceMuteLocalPlayer(bool isForceMuted)
        {
            _isForceMuted = isForceMuted;

            if (_isForceMuted)
            {
                try
                {
                    VivoxService.Instance.MuteInputDevice();
                    ULog.Info("Local player force-muted.");
                }
                catch (Exception ex)
                {
                    ULog.Error($"Error while force-muting local player: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    VivoxService.Instance.UnmuteInputDevice();
                }
                catch (Exception ex)
                {
                    ULog.Error($"Error while releasing force-mute on local player: {ex.Message}");
                }
                ULog.Info("Local player force-mute released. Local mute state can now be changed by ToggleMuteParticipantLocally.");
            }
        }

        // rate limiting

        private void EnqueueCommandMessage(string text, string commandChannelName)
        {
            if (!_isInitialized)
            {
                ULog.Error("Cannot enqueue command message: VivoxHandler is not initialized.");
                return;
            }

            var type = ExtractCommandTypeFromJson(text);
            if (string.IsNullOrEmpty(type))
            {
                // Fallback: treat as unique so we do not incorrectly coalesce unknown payloads
                type = Guid.NewGuid().ToString("N");
            }

            if (_latestCommandMessageByType.TryGetValue(type, out var existing))
            {
                // Coalesce: keep only the latest payload for this type
                existing.Text = text;
                existing.CommandChannelName = commandChannelName;
            }
            else
            {
                var message = new QueuedCommandMessage
                {
                    Text = text,
                    CommandChannelName = commandChannelName,
                    Type = type
                };

                _latestCommandMessageByType[type] = message;
                _commandMessageQueue.Add(message);
            }

            if (!_isProcessingCommandQueue)
            {
                ProcessCommandMessageQueueAsync().Forget();
            }
        }

        private async UniTask ProcessCommandMessageQueueAsync()
        {
            _isProcessingCommandQueue = true;

            try
            {
                while (_commandMessageQueue.Count > 0)
                {
                    var message = _commandMessageQueue[0];

                    // Enforce minimum 1 second between actual sends
                    var now = DateTime.UtcNow;
                    if (_lastCommandSentUtc != default)
                    {
                        var elapsed = now - _lastCommandSentUtc;
                        var remaining = TimeSpan.FromSeconds(1) - elapsed;
                        if (remaining > TimeSpan.Zero)
                        {
                            await UniTask.Delay(remaining);
                        }
                    }

                    try
                    {
                        await VivoxService.Instance.SendChannelTextMessageAsync(message.CommandChannelName, message.Text);
                        _lastCommandSentUtc = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        ULog.Error($"Error sending command message: {ex.Message}");
                    }

                    // Remove from queue
                    _commandMessageQueue.RemoveAt(0);

                    // Only clear the type mapping if it still points to the message we just sent
                    if (_latestCommandMessageByType.TryGetValue(message.Type, out var current) &&
                        ReferenceEquals(current, message))
                    {
                        _latestCommandMessageByType.Remove(message.Type);
                    }
                }
            }
            finally
            {
                _isProcessingCommandQueue = false;
            }
        }

        private string ExtractCommandTypeFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            const string typeKey = "\"Type\"";
            var index = json.IndexOf(typeKey, StringComparison.Ordinal);
            if (index < 0)
            {
                index = json.IndexOf("\"type\"", StringComparison.Ordinal);
                if (index < 0)
                {
                    return null;
                }
            }

            index = json.IndexOf(':', index);
            if (index < 0)
            {
                return null;
            }

            index++;
            while (index < json.Length && char.IsWhiteSpace(json[index]))
            {
                index++;
            }

            if (index >= json.Length)
            {
                return null;
            }

            if (json[index] == '"')
            {
                index++;
                var start = index;

                while (index < json.Length)
                {
                    if (json[index] == '\\')
                    {
                        index += 2;
                        continue;
                    }

                    if (json[index] == '"')
                    {
                        break;
                    }

                    index++;
                }

                if (index > start && index <= json.Length)
                {
                    return json.Substring(start, index - start);
                }

                return null;
            }
            else
            {
                var start = index;
                while (index < json.Length &&
                       json[index] != ',' &&
                       json[index] != '}' &&
                       !char.IsWhiteSpace(json[index]))
                {
                    index++;
                }

                if (index > start)
                {
                    return json.Substring(start, index - start);
                }

                return null;
            }
        }

        private sealed class QueuedCommandMessage
        {
            public string Text;
            public string CommandChannelName;
            public string Type;
        }
    }
}