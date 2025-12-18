using System;
using CesiumForUnity;
using Cysharp.Threading.Tasks;
using Foxbyte.Core;
using Unity.Mathematics;
using Unity.Services.Vivox;
using UnityEngine;

namespace FHH.Logic.Components.Networking
{
    /// <summary>
    /// Listens to Vivox channel messages and forwards command messages
    /// from a dedicated command channel into the ICommandBus.
    /// </summary>
    public sealed class VivoxCommandHandler : IUGSModule
    {
        private readonly ICommandBus _commandBus;
        private readonly string _commandChannelName;
        private bool _isInitialized;
        private CesiumGeoreference _geoRef;

        public VivoxCommandHandler(ICommandBus commandBus, string commandChannelName = "cmd")
        {
            _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
            _commandChannelName = string.IsNullOrWhiteSpace(commandChannelName)
                ? "cmd"
                : commandChannelName;
        }

        public async UniTask InitAsync(UGSService context)
        {
            if (_isInitialized) return;

            if (context == null)
            {
                ULog.Error("VivoxCommandHandler.InitAsync: context is null.");
                return;
            }

            // Make sure UnityServices + Authentication are ready; Vivox login is handled by VivoxHandler.
            await context.EnsureReadyAsync();

            if (VivoxService.Instance == null)
            {
                ULog.Error("VivoxCommandHandler.InitAsync: VivoxService.Instance is null.");
                return;
            }

            VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
            _isInitialized = true;

            ULog.Info($"VivoxCommandHandler.InitAsync: subscribed to ChannelMessageReceived for command channel '{_commandChannelName}'.");

            var geoRefGo = GameObject.FindGameObjectWithTag("CesiumGeoRef"); 
            _geoRef = geoRefGo != null ? geoRefGo.GetComponent<CesiumForUnity.CesiumGeoreference>() : null;
            if (_geoRef == null)
            {
                ULog.Error("CesiumGeoreference not found on CesiumGeoRef.");
            }
        }

        public async UniTask DisposeAsync()
        {
            if (!_isInitialized)
            {
                await UniTask.CompletedTask;
                return;
            }

            if (VivoxService.Instance != null)
            {
                VivoxService.Instance.ChannelMessageReceived -= OnChannelMessageReceived;
                ULog.Info("VivoxCommandHandler.DisposeAsync: unsubscribed from ChannelMessageReceived.");
            }

            _isInitialized = false;
            await UniTask.CompletedTask;
        }

        private void OnChannelMessageReceived(VivoxMessage message)
        {
            if (!_isInitialized)
            {
                return;
            }

            if (message == null)
            {
                ULog.Error("VivoxCommandHandler.OnChannelMessageReceived: message is null.");
                return;
            }

            // Ignore directed messages
            if (message.ChannelName == null)
            {
                return;
            }

            // Only process messages from the command channel
            if (!message.ChannelName.StartsWith("cmd_", StringComparison.Ordinal))
            {
                return;
            }

            // Ignore self-originated commands
            var vivox = VivoxService.Instance;
            if (vivox == null)
            {
                ULog.Error("VivoxCommandHandler.OnChannelMessageReceived: VivoxService.Instance is null.");
                return;
            }

            string localPlayerId = vivox.SignedInPlayerId;
            if (!string.IsNullOrEmpty(localPlayerId) &&
                !string.IsNullOrEmpty(message.SenderPlayerId) &&
                string.Equals(localPlayerId, message.SenderPlayerId, StringComparison.Ordinal))
            {
                ULog.Info("VivoxCommandHandler.OnChannelMessageReceived: ignoring self-originated command.");
                return;
            }

            if (message.FromSelf)
            {
                ULog.Info("Ignored message from self (Additional check)");
                return;
            }

            ULog.Info(
                $"VivoxCommandHandler.OnChannelMessageReceived: command message " +
                $"from sender='{message.SenderPlayerId}' in channel='{message.ChannelName}': {message.MessageText}");

            CommandPayload payload;
            try
            {
                payload = JsonUtility.FromJson<CommandPayload>(message.MessageText);
            }
            catch (Exception ex)
            {
                ULog.Error($"VivoxCommandHandler.OnChannelMessageReceived: JSON parse error: {ex}");
                return;
            }

            if (payload == null || string.IsNullOrEmpty(payload.Type))
            {
                ULog.Error("VivoxCommandHandler.OnChannelMessageReceived: payload null or missing Type.");
                return;
            }

            try
            {
                RouteToCommandBus(payload);
            }
            catch (Exception ex)
            {
                ULog.Error($"VivoxCommandHandler.OnChannelMessageReceived: error while routing command: {ex}");
            }
        }

        private void RouteToCommandBus(CommandPayload payload)
        {
            switch (payload.Type)
            {
                case "MuteAll":
                    ULog.Info($"VivoxCommandHandler.RouteToCommandBus: enqueue MuteAll={payload.Enabled}.");
                    _commandBus.EnqueueMuteAll(payload.Enabled);
                    break;

                case "Indicator":
                {
                    //var geoPos =(Unity.Mathematics.float3)
                        _geoRef.TransformEarthCenteredEarthFixedPositionToUnity(new double3(payload.X, payload.Y, payload.Z));
                    //var position = new Vector3(geoPos.x, geoPos.y, geoPos.z);
                    var position = new Vector3((float)payload.X, (float)payload.Y, (float)payload.Z);
                    ULog.Info($"VivoxCommandHandler.RouteToCommandBus: enqueue Indicator position={position}.");
                    _commandBus.EnqueueIndicatorPosition(position);
                    break;
                }

                case "GuidedMode":
                    ULog.Info("VivoxCommandHandler.RouteToCommandBus: enqueue GuidedMode " +
                              $"project={payload.ProjectId}, variant={payload.Variant}, enabled={payload.Enabled}.");
                    _commandBus.EnqueueGuidedMode(payload.ProjectId, payload.Variant, payload.Enabled);
                    break;

                case "Teleport":
                    ULog.Info(
                        $"VivoxCommandHandler.RouteToCommandBus: handling Teleport command " +
                        $"to X={payload.X}, Y={payload.Y}, Z={payload.Z}.");
                    _commandBus.EnqueueTeleport(new double3(payload.X, payload.Y, payload.Z), payload.Rotation);
                    break;

                case "Sun":
                    ULog.Info($"VivoxCommandHandler.RouteToCommandBus: handling Sun command enabled={payload.Enabled}.");
                    _commandBus.EnqueueSunTime(payload.Year, payload.Month, payload.Day, payload.Hour, payload.Minute);
                    break;

                case "Variant":
                    ULog.Info($"VivoxCommandHandler.RouteToCommandBus: handling Variant command for variant={payload.Variant}.");
                    _commandBus.EnqueueVariant(payload.Variant);
                    break;

                default:
                    ULog.Error($"VivoxCommandHandler.RouteToCommandBus: unknown command Type='{payload.Type}'.");
                    break;
            }
        }
    }
}