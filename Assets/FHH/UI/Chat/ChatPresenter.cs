using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Components.Networking;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FHH.Input;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace FHH.UI.Chat
{
    public sealed class ChatPresenter : PresenterBase<ChatPresenter, ChatView, ChatModel>
    {
        private VivoxHandler _vivoxHandler;
        private PlayerController _pc;
        private InputActionAsset _inputAsset;
        private string _textChannelName;

        public ChatPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            ChatModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model ?? new ChatModel(), perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            await Model.InitializeAsync();
            _vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();
            _textChannelName = _vivoxHandler.CurrentTextChannel;
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await base.InitializeAfterUiAsync();

            if (View == null)
            {
                return;
            }

            if (View.InputField != null)
            {
                View.InputField.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
            }

            if (View.SendButton != null)
            {
                View.SendButton.clicked += () => { UniTask.Void(async () => await OnSendClickedAsync()); };
            }

            VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;

            _pc = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
            if (!_pc)
            {
                ULog.Error("[ChatPresenter] PlayerController not found in scene.");
            }
            else
            {
                _inputAsset = _pc.InputAsset;
            }

            if (_vivoxHandler != null)
            {
                _textChannelName = _vivoxHandler.CurrentTextChannel;
            }
            
            // pre-fetch recent history for the channel (oldest-to-newest)
            if (!string.IsNullOrEmpty(_textChannelName))
            {
                var vivox = VivoxService.Instance;
                // Ensure we are actually in this text channel and it's not a command channel
                if (vivox.ActiveChannels != null
                    && vivox.ActiveChannels.ContainsKey(_textChannelName)
                    && !_textChannelName.StartsWith("cmd_", StringComparison.Ordinal))
                {
                    await LoadInitialHistoryAsync(_textChannelName);
                }
            }

        }

        protected override async UniTask OnViewUnmountedAsync()
        {
            VivoxService.Instance.ChannelMessageReceived -= OnChannelMessageReceived;
            await base.OnViewUnmountedAsync();
        }

        private void OnInputKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                evt.StopPropagation();
                UniTask.Void(async () => await OnSendClickedAsync());
            }
        }

        private async UniTask OnSendClickedAsync()
        {
            if (View?.InputField == null)
            {
                return;
            }

            var text = View.InputField.value;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var guardSource = (VisualElement)View.SendButton;
            if (guardSource == null)
            {
                guardSource = View.InputField;
            }

            await RunGuardedAsync(
                guardSource,
                async ct =>
                {
                    await Model.SendMessageAsync(text, ct);
                    View.InputField.SetValueWithoutNotify(string.Empty);
                    View.InputField.Focus();
                });
        }

        private void OnChannelMessageReceived(VivoxMessage message)
        {
            if (message == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(message.ChannelName)
                && message.ChannelName.StartsWith("cmd_", StringComparison.Ordinal))
            {
                return;
            }

            var currentChannel = _vivoxHandler?.CurrentTextChannel;
            if (!string.IsNullOrEmpty(currentChannel) &&
                !string.Equals(message.ChannelName, currentChannel, StringComparison.Ordinal))
            {
                return;
            }

            var senderName = Model.ResolveDisplayName(message);
            var timeLocal = message.ReceivedTime.ToLocalTime();
            var timeText = timeLocal.ToString("HH:mm", CultureInfo.InvariantCulture);
            var timestampText = FormatTimestamp(message.ReceivedTime);

            //var fromSelf = message.FromSelf;
            var fromSelf = false;
            var localPlayerId = Model.LocalPlayerId;

            if (!string.IsNullOrEmpty(localPlayerId) &&
                string.Equals(message.SenderPlayerId, localPlayerId, StringComparison.Ordinal))
            {
                fromSelf = true;
            }

            //View?.AppendMessage(senderName, timeText, message.MessageText, fromSelf);
            View?.AppendMessage(senderName, timestampText, message.MessageText, fromSelf);
        }

        private async UniTask LoadInitialHistoryAsync(string channelName)
        {
            try
            {
                var history = await VivoxService.Instance.GetChannelTextMessageHistoryAsync(channelName, 25);

                if (history == null || history.Count == 0)
                {
                    return;
                }

                // Ensure oldest-at-top, newest-at-bottom ordering
                var ordered = new List<VivoxMessage>(history);
                ordered.Sort((a, b) => a.ReceivedTime.CompareTo(b.ReceivedTime));

                foreach (var message in ordered)
                {
                    var senderName = Model.ResolveDisplayName(message);
                    var timeLocal = message.ReceivedTime.ToLocalTime();
                    var timeText = timeLocal.ToString("HH:mm", CultureInfo.InvariantCulture);
                    var timestampText = FormatTimestamp(message.ReceivedTime);
                    //var fromSelf = message.FromSelf;
                    var fromSelf = false;
                    var localPlayerId = Model.LocalPlayerId;

                    if (!string.IsNullOrEmpty(localPlayerId) &&
                        string.Equals(message.SenderPlayerId, localPlayerId, StringComparison.Ordinal))
                    {
                        fromSelf = true;
                    }

                    //View?.AppendMessage(senderName, timeText, message.MessageText, fromSelf);
                    View?.AppendMessage(senderName, timestampText, message.MessageText, fromSelf);
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"[ChatPresenter] Failed to load chat history: {ex.Message}");
            }
        }

        internal void OnInputFieldFocusIn()
        {
            if (_inputAsset == null) return; 
            InputActionMap map = _inputAsset.FindActionMap("Player", true);
            map.Disable();
        }

        internal void OnInputFieldFocusOut()
        {
            if (_inputAsset == null) return; 
            InputActionMap map = _inputAsset.FindActionMap("Player", true);
            map.Enable();
        }

        private string FormatTimestamp(DateTime receivedTime)
        {
            var local = receivedTime.ToLocalTime();
            var timeText = local.ToString("HH:mm", CultureInfo.InvariantCulture);
            var dateText = local.ToString("dd.MM", CultureInfo.CurrentCulture);
            return $"{dateText}, {timeText}";
        }
    }
}