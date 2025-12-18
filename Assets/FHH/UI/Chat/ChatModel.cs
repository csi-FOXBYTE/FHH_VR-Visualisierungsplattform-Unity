using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Components.Networking;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using Unity.Services.Authentication;
using Unity.Services.Vivox;

namespace FHH.UI.Chat
{
    public sealed class ChatModel : PresenterModelBase
    {
        private VivoxHandler _vivoxHandler;
        private string _localAuthName;
        private string _localDisplayName;
        private string _localPlayerId;
        public string LocalDisplayName => _localDisplayName;
        public string LocalPlayerId => _localPlayerId;
        
        
        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            _vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();

            try
            {
                var permissionService = ServiceLocator.GetService<PermissionService>();
                var user = permissionService?.GetUser();
                if (!string.IsNullOrEmpty(user?.DisplayName))
                {
                    _localDisplayName = user.DisplayName;
                }
                _localPlayerId = AuthenticationService.Instance.PlayerId;
                var authName = await AuthenticationService.Instance.GetPlayerNameAsync();
                if (!string.IsNullOrEmpty(authName))
                {
                    _localAuthName = authName;
                    if (string.IsNullOrEmpty(_localDisplayName))
                    {
                        _localDisplayName = PlayerNameUtility.FromAuthNameForChat(authName);
                    }
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"[ChatModel] Failed to resolve player name: {ex.Message}");
            }
        }

        public async UniTask SendMessageAsync(string text, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (_vivoxHandler == null)
            {
                ULog.Error("[ChatModel] VivoxHandler not available; cannot send chat message.");
                return;
            }

            // Vivox default max size is typically ~320 bytes UTF-8
            try
            {
                await _vivoxHandler.SendTextMessageAsync(text);
            }
            catch (Exception ex)
            {
                ULog.Error($"[ChatModel] Failed to send chat message: {ex.Message}");
            }
        }

        public string ResolveDisplayName(VivoxMessage message)
        {
            if (message == null)
            {
                return string.Empty;
            }

            if (message.FromSelf && !string.IsNullOrEmpty(_localDisplayName))
            {
                return _localDisplayName;
            }

            if (!string.IsNullOrEmpty(message.SenderDisplayName))
            {
                return message.SenderDisplayName;
            }

            return message.SenderPlayerId ?? string.Empty;
        }
    }
}