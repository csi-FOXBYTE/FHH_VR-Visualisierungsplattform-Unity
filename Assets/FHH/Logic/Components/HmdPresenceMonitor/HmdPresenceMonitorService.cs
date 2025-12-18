using Foxbyte.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace FHH.Logic.Components.HmdPresenceMonitor
{
    /// <summary>
    /// Provides functionality to monitor the presence and connection state of a head-mounted display (HMD) device.
    /// Currently suffers from a known OpenXR bug that fails to report a disconnect properly.
    /// </summary>
    
    public class HmdPresenceMonitorService : IAppService, IDisposable
    {
        //public bool IsUserPresent { get; private set; }
        //public event Action<bool> OnUserPresenceChanged;
        public float CameraFoV { get; private set; }
        public bool IsDeviceEnabled { get; private set; } // workaround for now to at least initially check for device
        public bool AreSubSystemsChecked { get; set; } // checked by XrOriginStartupAligner
        public event Action<bool> OnDeviceEnabled;
        
        private InputDevice _hmd;

        private static readonly InputDeviceCharacteristics _hmdChars =
            InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice;

        public void InitService()
        {
            //FindHmd();
            
            //InputDevices.deviceConnected += OnDeviceConnected;
            //InputDevices.deviceDisconnected += OnDeviceDisconnected;
            //CheckPresence(forceNotify: true); //this feature is currently officially bugged
        }

        public void DisposeService()
        {
            //InputDevices.deviceConnected -= OnDeviceConnected;
            //InputDevices.deviceDisconnected -= OnDeviceDisconnected;
        }

        public void Dispose() => DisposeService();
        void OnDestroy() => DisposeService();

        private void OnDeviceConnected(InputDevice device)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
            {
                _hmd = device;
                IsDeviceEnabled = true;
                OnDeviceEnabled?.Invoke(true);
                //CheckPresence(forceNotify: true);
                ULog.Info($"HMD connected: {device.name} ({device.characteristics})");
            }
        }

        /// <summary>
        /// Currently officially bugged and will only fire when device reconnects.
        /// </summary>
        /// <param name="device"></param>
        private void OnDeviceDisconnected(InputDevice device)
        {
            if (_hmd == device)
            {
                _hmd = default;
                IsDeviceEnabled = false;
                OnDeviceEnabled?.Invoke(false);
                //CheckPresence(forceNotify: true); // will flip to false
                ULog.Info($"HMD disconnected: {device.name} ({device.characteristics})");
            }
        }
        
        private void FindHmd()
        {
            _hmd = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (_hmd.isValid) return;

            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(_hmdChars, devices);
            _hmd = devices.Count > 0 ? devices[0] : default;
        }

        public bool CheckIfXRDeviceIsPresent()
        {
            FindHmd();
            IsDeviceEnabled = _hmd.isValid;
            //ULog.Info($"Checking device enabled status: {IsDeviceEnabled}");
            if (AreSubSystemsChecked && IsDeviceEnabled)
            {
                if (TryGetCurrentFovDegrees(out float h, out float v, Camera.main))
                {
                    Debug.Log($"FoV H={h:F1}°, V={v:F1}°");
                    CameraFoV = Math.Max(h, v);
                }
                else
                {
                    CameraFoV = 100f; // fallback
                    ULog.Info("Could not get current FOV, using fallback.");
                }
                //ULog.Info($"HMD found during init: {_hmd.name} ({_hmd.characteristics}), FOV Zoom: {CameraFoV}");
            }
            else
            {
                CameraFoV = 100f; // default fallback
                //ULog.Info("No HMD found during init.");
            }
            return IsDeviceEnabled;
        }

        public void SetVrEnabled(bool enable)
        {
            bool success = false;

            // Try XR Plugin Management path (OpenXR)
            var xrGeneral = XRGeneralSettings.Instance;
            var xrManager = xrGeneral != null ? xrGeneral.Manager : null;

            try
            {
                if (xrManager != null)
                {
                    if (enable)
                    {
                        if (xrManager.activeLoader == null)
                        {
                            xrManager.InitializeLoaderSync();
                        }
                        xrManager.StartSubsystems();
                    }
                    else
                    {
                        // Stop and deinit
                        xrManager.StopSubsystems();
                        xrManager.DeinitializeLoader();
                    }

                    success = true;
                }
            }
            catch (Exception ex)
            {
                ULog.Warning($"XR Management toggle failed, attempting XRSettings fallback. {ex.Message}");
            }

            // Fallback to legacy XRSettings if needed
            if (!success) 
            {
                try
                {
#pragma warning disable CS0618
                    XRSettings.enabled = enable;
#pragma warning restore CS0618
                    success = true;
                }
                catch (Exception ex)
                {
                    ULog.Error($"Failed to toggle VR via XRSettings: {ex.Message}");
                }
            }

            // Update internal state and notify listeners so input mode can switch
            IsDeviceEnabled = enable;
            OnDeviceEnabled?.Invoke(enable);
            ULog.Info($"VR {(enable ? "ENABLED" : "DISABLED")} via SetVrEnabled (success={success}).");
        }

        //public void CheckPresence(bool forceNotify = false)
        //{
        //    bool present = IsUserPresent;
        //    bool hasValue = _hmd.isValid && _hmd.TryGetFeatureValue(CommonUsages.userPresence, out present);

        //    // fallback: conservative assumption if runtime doesn’t support userPresence
        //    if (!hasValue && _hmd.isValid &&
        //        _hmd.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked))
        //    {
        //        present = isTracked;
        //        hasValue = true;
        //    }

        //    if (forceNotify || present != IsUserPresent)
        //    {
        //        IsUserPresent = present;
        //        OnUserPresenceChanged?.Invoke(present);
        //    }
        //}

        public struct EyeFovDeg  // public fields use PascalCase (your preference)
        {
            public float Left;    // degrees
            public float Right;   // degrees
            public float Top;     // degrees
            public float Bottom;  // degrees
        }

        /// <summary>
        /// One-call method: returns average horizontal/vertical FoV across both eyes (degrees).
        /// Returns false if XR not active or camera missing.
        /// </summary>
        public bool TryGetCurrentFovDegrees(out float horizontal, out float vertical, Camera cam = null)
        {
            horizontal = 0f;
            vertical = 0f;

            if (!XRSettings.isDeviceActive)
                return false;

            EyeFovDeg left, right;
            if (!TryGetEyeFov(out left, out right, cam))
                return false;

            // Approx combined per-eye FoV. If you need canting-aware totals, handle that separately.
            horizontal = left.Left + left.Right; // degrees
            vertical = left.Top + left.Bottom; // symmetric in most setups; either eye is fine
            return true;
        }

        /// <summary>
        /// Gets per-eye FoV (degrees) from the current stereo projection matrices.
        /// </summary>
        public bool TryGetEyeFov(out EyeFovDeg leftEye, out EyeFovDeg rightEye, Camera cam = null)
        {
            leftEye = default;
            rightEye = default;

            if (!XRSettings.isDeviceActive)
                return false;

            var camera = ResolveCamera(cam);
            if (camera == null)
                return false;

            // check if getting the matrices returns something valid and if not gracefully fail


            var projL = camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            var projR = camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);


            if (projL == Matrix4x4.identity || projR == Matrix4x4.identity)
                return false;
            
            leftEye = ProjectionToFovDeg(projL);
            rightEye = ProjectionToFovDeg(projR);
            return true;
        }

        private Camera ResolveCamera(Camera candidate)
        {
            if (candidate != null) return candidate;
            if (Camera.main != null) return Camera.main;
            // Fallback: first enabled camera in scene
            //var all = GameObject.FindObjectsOfType<Camera>();
            var all = GameObject.FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < all.Length; i++)
                if (all[i].enabled) return all[i];
            return null;
        }

        // Converts a projection matrix (including asymmetric frustums) into tangential FoV angles (degrees)
        private EyeFovDeg ProjectionToFovDeg(Matrix4x4 p)
        {
            // r/n = (1 + m02) / m00,  l/n = (1 - m02) / m00
            // t/n = (1 + m12) / m11,  b/n = (1 - m12) / m11
            float m00 = p.m00, m11 = p.m11, m02 = p.m02, m12 = p.m12;

            float tanRight = (1f + m02) / m00;
            float tanLeft = (1f - m02) / m00;
            float tanTop = (1f + m12) / m11;
            float tanBottom = (1f - m12) / m11;

            EyeFovDeg f;
            f.Right = Mathf.Rad2Deg * Mathf.Atan(tanRight);
            f.Left = Mathf.Rad2Deg * Mathf.Atan(tanLeft);
            f.Top = Mathf.Rad2Deg * Mathf.Atan(tanTop);
            f.Bottom = Mathf.Rad2Deg * Mathf.Atan(tanBottom);
            return f;
        }
    }
}