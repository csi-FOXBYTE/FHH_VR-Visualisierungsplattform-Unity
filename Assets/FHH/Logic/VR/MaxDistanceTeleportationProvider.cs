using CesiumForUnity;
using Cysharp.Threading.Tasks;
using FHH.Logic.Components.Collaboration;
using FHH.Logic.Components.Networking;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using System;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace FHH.Logic.VR
{
    public class MaxDistanceTeleportationProvider : TeleportationProvider
    {
        [SerializeField] private float _maxDistanceMeters = 500f;
        [SerializeField] private bool _horizontalOnly = true;
        [SerializeField] private Transform _originOverride;
        [SerializeField] private CesiumGeoreference _georef;
        private CollaborationService _collab;

        public float MaxDistanceMeters
        {
            get => _maxDistanceMeters;
            set => _maxDistanceMeters = Mathf.Max(0f, value);
        }

        public bool HorizontalOnly
        {
            get => _horizontalOnly;
            set => _horizontalOnly = value;
        }

        public Transform OriginOverride
        {
            get => _originOverride;
            set => _originOverride = value;
        }

        public bool QueueTeleportRequest(TeleportRequest teleportRequest, bool ignoreDistanceLimit)
        {
            if (ignoreDistanceLimit)
            {
                SendTeleportCommand(teleportRequest);
                return base.QueueTeleportRequest(teleportRequest);
            }
            return QueueTeleportRequest(teleportRequest);
        }

        public override bool QueueTeleportRequest(TeleportRequest teleportRequest)
        {
            var originPos = GetDistanceOrigin();
            var targetPos = teleportRequest.destinationPosition;

            float distance = _horizontalOnly
                ? Vector3.Distance(new Vector3(originPos.x, 0f, originPos.z),
                                   new Vector3(targetPos.x, 0f, targetPos.z))
                : Vector3.Distance(originPos, targetPos);

            if (_maxDistanceMeters > 0f && distance > _maxDistanceMeters)
                return false; // Reject: exceeds limit

            SendTeleportCommand(teleportRequest);

            return base.QueueTeleportRequest(teleportRequest);
        }

        private void SendTeleportCommand(TeleportRequest teleportRequest)
        {
            // collaboration command to teleport other users
            var destPos = teleportRequest.destinationPosition;
            var destRot = teleportRequest.destinationRotation;
            if (destRot == Quaternion.identity && mediator != null && mediator.xrOrigin != null && mediator.xrOrigin.Camera != null)
            {
                // Fallback: preserve current yaw when the teleport request doesn't specify rotation
                Vector3 fwd = mediator.xrOrigin.Camera.transform.forward; 
                fwd.y = 0f;
                if (fwd.sqrMagnitude > 1e-6f)
                    destRot = Quaternion.LookRotation(fwd.normalized, Vector3.up);
            }

            var ecefPos = _georef.TransformUnityPositionToEarthCenteredEarthFixed(new double3(destPos));
            if (_collab == null)
            {
                _collab = ServiceLocator.GetService<CollaborationService>();
            }
            if (_collab != null)
            {
                if (!_collab.IsGuidedModeEnabled) return;
            }
            if (!ServiceLocator.GetService<PermissionService>().IsModerator()) return;
            SendCommandAsync("Teleport", x: ecefPos.x, y: ecefPos.y, z: ecefPos.z, rotation: destRot).Forget();
            //ULog.Info($"SEND teleportRequest pos={teleportRequest.destinationPosition} rot={teleportRequest.destinationRotation}");
        }

        private Vector3 GetDistanceOrigin()
        {
            if (_originOverride != null)
                return _originOverride.position;

            // Prefer the XR Origin controlled by the Locomotion system
            var m = mediator;
            if (m != null && m.xrOrigin != null)
            {
                var originGO = m.xrOrigin.Origin != null ? m.xrOrigin.Origin.transform : m.xrOrigin.transform;
                return originGO.position;
            }

            // Fallback
            return transform.position;
        }

        private async UniTask SendCommandAsync(string type,
            bool value = false,
            double x = 0, double y = 0, double z = 0,
            string projectId = null,
            string variant = null,
            bool enabled = false,
            Quaternion rotation = default,
            int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0
        )
        {
            var json = JsonUtility.ToJson(new CommandPayload
            {
                Type = type,
                Value = value,
                X = x,
                Y = y,
                Z = z,
                ProjectId = projectId,
                Variant = variant,
                Enabled = enabled,
                Rotation = rotation,
                Year = year,
                Month = month,
                Day = day,
                Hour = hour,
                Minute = minute
            });

            try
            {
                var vivoxHandler = ServiceLocator.GetService<UGSService>().GetModule<VivoxHandler>();
                vivoxHandler.SendCommandMessageAsync(json, _collab.CommandChannelName).Forget();
            }
            catch (Exception ex)
            {
                ULog.Warning($"Failed to send command '{type}': {ex.Message}");
            }
            await UniTask.CompletedTask;
        }
    }
}
