using System;
using FHH.Input;
using FHH.Logic.Models;
using Foxbyte.Core.Services.ConfigurationService;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;


namespace FHH.Logic.VR
{
    public class SafetyPlatformHandler : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private XROrigin _xrOrigin;
        [SerializeField] private InputActionAsset _inputAsset;

        [SerializeField] [Range(-10f,10f)] private float _heightOffset = -1.3f;
        
        private bool _isEnabled; 
        private bool _isFlying = true; // app starts in fly mode
        private InputAction _resetPlatformAction;

        private Matrix4x4 _relativeSample = Matrix4x4.identity;

        public event Action<bool>  OnSafetyPlatformChanged;

        void Awake()
        {
            if (!_playerController) 
                Debug.LogError("SafetyPlatformHandler requires a PlayerController reference. Please assign it in the inspector.");
            if (!_xrOrigin)
                Debug.LogError("SafetyPlatformHandler requires an XROrigin reference. Please assign it in the inspector.");
            if (!_inputAsset)
                Debug.LogError("SafetyPlatformHandler requires an InputActionAsset reference. Please assign it in the inspector.");
        }

        void OnEnable()
        {
            _resetPlatformAction = _inputAsset.FindAction("Player/Reset Platform", true);
            _resetPlatformAction.performed += OnResetPlatform;
            _resetPlatformAction.Enable();
            Application.onBeforeRender += BeforeRenderFollow;
            _playerController.OnFlyModeToggled += HandleFlyModeToggled;
            SetEnabled(LoadSavedState(), persist:false);
        }

        void OnDisable()
        {
            Application.onBeforeRender -= BeforeRenderFollow;
            if (_resetPlatformAction != null)
            {
                _resetPlatformAction.performed -= OnResetPlatform;
                _resetPlatformAction.Disable();
            }
            _playerController.OnFlyModeToggled -= HandleFlyModeToggled;
        }

        void OnDestroy()
        {
            Application.onBeforeRender -= BeforeRenderFollow;
        }
        private void OnResetPlatform(InputAction.CallbackContext ctx)
        {
            SetEnabled(!_isEnabled);
        }

        void BeforeRenderFollow()
        {
            if (!IsVisibleNow()) return;
            ApplyToC();
        }

        public void Toggle(bool isEnabled)
        {
            SetEnabled(isEnabled); // persist user intent same as input action
        }

        /// <summary>
        /// The single place that sets the user preference, (re)samples pose,
        /// updates visibility, and optionally saves.
        /// </summary>
        private void SetEnabled(bool isEnabled, bool persist = true)
        {
            _isEnabled = isEnabled;
            if (_isEnabled)
            {
                // Sample camera pose relative to origin at the moment of enabling
                _relativeSample = _xrOrigin.transform.worldToLocalMatrix *
                                  _xrOrigin.Camera.transform.localToWorldMatrix;
            }
            UpdateVisibility();
            if (persist)
                SaveState(_isEnabled);
            OnSafetyPlatformChanged?.Invoke(_isEnabled);
        }

        private bool IsVisibleNow() => _isEnabled && _isFlying;

        private void UpdateVisibility()
        {
            bool visible = IsVisibleNow();
            //gameObject.SetActive(true);
            foreach (Transform child in transform)
                child.gameObject.SetActive(visible);
        }

        private void ApplyToC()
        {
            Matrix4x4 world = _xrOrigin.transform.localToWorldMatrix * _relativeSample;
            
            Vector3 pos = world.MultiplyPoint3x4(Vector3.zero);
            pos.y = pos.y + _heightOffset;

            Vector3 fwd = world.MultiplyVector(Vector3.forward).normalized;
            Vector3 flatFwd = Vector3.ProjectOnPlane(fwd, Vector3.up);
            Quaternion upright = Quaternion.LookRotation(flatFwd.normalized, Vector3.up);
            
            transform.SetPositionAndRotation(pos, upright);
        }

        private void HandleFlyModeToggled(bool isEnabled)
        {
            _isFlying = isEnabled;
            UpdateVisibility();
        }

        public bool LoadSavedState()
        {
            var userSettings = ServiceLocator.GetService<ConfigurationService>().LoadUserSettings<UserSettings>();
            return userSettings.SupportPlatform;
        }

        private void SaveState(bool value)
        {
            var config = ServiceLocator.GetService<ConfigurationService>();
            config.SaveUserSettingProperty<UserSettings, bool>(u => u.SupportPlatform, value);
        }
    }
}