using System.Collections;
using System.Collections.Generic;
using FHH.Logic.Components.HmdPresenceMonitor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Unity.XR.CoreUtils;
using UnityEngine.XR.OpenXR;

namespace FHH.Logic.VR
{
    /// <summary>
    /// At startup, aligns the XROrigin yaw so that the camera forward matches a specified reference (or world forward).
    /// </summary> 
    [DefaultExecutionOrder(-500)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XROrigin))]
    public class XrOriginStartupAligner : MonoBehaviour
    {
        public enum StartupFloorMode
        {
            None,
            SwitchToFloorAfterAlign
        }

        [SerializeField] private XROrigin _xrOrigin;
        [SerializeField] private bool _alignOnStart = true;
        [SerializeField] private StartupFloorMode _floorMode = StartupFloorMode.SwitchToFloorAfterAlign;
        [SerializeField] private Transform _appForwardReference; // optional
        [SerializeField] private float _minHeadsetTrackingTimeout = 2f; // Seconds
        [SerializeField] private bool _allowSystemRecentering = true; // OpenXR recenter policy
        [SerializeField] private bool _logDiagnostics = true;

        private XRInputSubsystem _inputSubsystem;
        private bool _aligned;

        public TrackingOriginModeFlags CurrentOriginMode =>
            _inputSubsystem != null ? _inputSubsystem.GetTrackingOriginMode() : TrackingOriginModeFlags.Unknown;

        void Awake()
        {
            if (_xrOrigin == null) _xrOrigin = GetComponent<XROrigin>();
        }

        IEnumerator Start()
        {
            if (!_alignOnStart) yield break;

            yield return EnsureInputSubsystem();

            try
            {
                ServiceLocator.GetService<HmdPresenceMonitorService>().AreSubSystemsChecked = true;
            }
            catch {}

            // Allow runtime-level recentering on platforms that support LOCAL_FLOOR (e.g., SteamVR/Meta via OpenXR)
            // No-op on older OpenXR plugin versions
            try
            {
                OpenXRSettings.SetAllowRecentering(_allowSystemRecentering, 1.5f);
            }
            catch
            {
            }

            // Normalize to Device/Local first; this avoids room-setup yaw differences on Stage/Floor spaces
            SetOriginToDevice(); // XROrigin + XRInputSubsystem
            
            AlignYaw(); // Make camera-forward == app forward on the horizontal plane
            
            if (_floorMode == StartupFloorMode.SwitchToFloorAfterAlign)
                TrySwitchToFloorKeepingYaw();

            //yield return WaitForHeadsetTracking(_minHeadsetTrackingTimeout);
            StartCoroutine(RealignAfterTracking());

            if (_logDiagnostics) LogState("Startup alignment complete");
        }

        public void FaceFront()
        {
            if (_xrOrigin == null) return;
            AlignYaw();
            if (_floorMode == StartupFloorMode.SwitchToFloorAfterAlign)
                TrySwitchToFloorKeepingYaw();
            if (_logDiagnostics) LogState("Manual FaceFront");
        }

        private IEnumerator EnsureInputSubsystem()
        {
            const float maxWait = 8.0f;
            float t = 0f;
            while (_inputSubsystem == null && t < maxWait)
            {
                _inputSubsystem = TryGetInputSubsystem();
                if (_inputSubsystem == null)
                {
                    t += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
        }

        private XRInputSubsystem TryGetInputSubsystem()
        {
            XRInputSubsystem result = null;
            var gs = XRGeneralSettings.Instance;
            if (gs != null && gs.Manager != null && gs.Manager.activeLoader != null)
                result = gs.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();

            if (result != null) return result;

            var list = new List<XRInputSubsystem>();
            SubsystemManager.GetSubsystems(list);
            return list.Count > 0 ? list[0] : null;
        }

        private void SetOriginToDevice()
        {
            if (_xrOrigin != null) _xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
            if (_inputSubsystem != null) _inputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
        }

        private IEnumerator WaitForHeadsetTracking(float timeout)
        {
            float t = 0f;
            bool hasTracked = false;
            var head = InputDevices.GetDeviceAtXRNode(XRNode.Head);

            while (t < timeout)
            {
                if (!head.isValid) head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
                if (head.isValid && head.TryGetFeatureValue(CommonUsages.isTracked, out bool tracked) && tracked)
                {
                    hasTracked = true;
                    break;
                }

                t += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!hasTracked && _logDiagnostics)
                Debug.LogWarning("[XrOriginStartupAligner] Headset tracking not confirmed; proceeding anyway.");
        }

        private void AlignYaw()
        {
            if (_xrOrigin == null || _xrOrigin.Camera == null) return;

            Vector3 up = Vector3.up;
            Vector3 desiredForward = _appForwardReference
                ? ProjectOnPlane(_appForwardReference.forward, up)
                : Vector3.forward;

            // Rotates the origin so the camera forward (projected) == desiredForward; up == world up
            _aligned = _xrOrigin.MatchOriginUpCameraForward(up, desiredForward);
        }

        private IEnumerator RealignAfterTracking()
        {
            if (_minHeadsetTrackingTimeout <= 0f) yield break; // respect 0 to disable
            yield return WaitForHeadsetTracking(_minHeadsetTrackingTimeout);
            AlignYaw();
            if (_floorMode == StartupFloorMode.SwitchToFloorAfterAlign)
                TrySwitchToFloorKeepingYaw();
            if (_logDiagnostics) LogState("Background re-align after tracking check");
        }

        private void TrySwitchToFloorKeepingYaw()
        {
            if (_inputSubsystem == null) return;

            var supported = _inputSubsystem.GetSupportedTrackingOriginModes();
            if ((supported & TrackingOriginModeFlags.Floor) != 0)
            {
                _inputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
                if (_xrOrigin != null) _xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
                // Floor/Stage forward varies by room setup, which is why we align first, then switch
            }
        }

        private static Vector3 ProjectOnPlane(Vector3 v, Vector3 n) => (v - Vector3.Dot(v, n) * n).normalized;

        private void LogState(string prefix)
        {
            string origin = CurrentOriginMode.ToString();
            var supported = _inputSubsystem != null
                ? _inputSubsystem.GetSupportedTrackingOriginModes().ToString()
                : "n/a";
            Debug.Log(
                $"[XrOriginStartupAligner] {prefix}. CurrentOrigin={origin}, Supported={supported}, Aligned={_aligned}");
        }
    }
}