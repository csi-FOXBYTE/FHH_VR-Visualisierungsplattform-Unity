using Cysharp.Threading.Tasks;
using FHH.Input;
using FHH.Logic;
using FHH.Logic.Cesium;
using FHH.Logic.Models;
using FHH.Logic.VR;
using Foxbyte.Core.Localization.Utilities;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace FHH.UI.Settings
{
    public sealed class SettingsPresenter 
        : PresenterBase<SettingsPresenter, SettingsView, SettingsModel>
    {
        // UI refs (queried by name for clarity & future reuse)
        private DropdownField _languageDropdown;
        private Toggle _modeToggle;
        private Label _modeTitleLabel;
        private Label _modeHintLabel;

        private Slider _moveSpeed;
        private Slider _turnSpeed;
        private Slider _turnAngle;
        private Toggle _supportPlatformToggle;
        private Toggle _vignetteToggle;
        private Button _disclaimerButton;

        private PlayerController _pc;
        private SafetyPlatformHandler _platform;

        public SettingsPresenter(
            GameObject targetGameobjectForView, 
            UIDocument uidoc, 
            StyleSheet styleSheet, 
            VisualElement target,
            SettingsModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model ?? new SettingsModel(), perms)
        {}

        public override async UniTask InitializeAfterUiAsync()
        {
            await UniTask.Yield();
            _pc = Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None).FirstOrDefault();
            _pc.OnMoveSpeedChanged   += HandleMoveSpeedChanged;
            _pc.OnTurnSpeedChanged   += HandleTurnSpeedChanged;
            _pc.OnTurnAngleChanged   += HandleTurnAngleChanged;
            _pc.OnVignetteChanged    += HandleVignetteChanged;
            _platform = Object.FindFirstObjectByType<SafetyPlatformHandler>();
            _platform.OnSafetyPlatformChanged += HandleSafetyPlatformChanged;

            // look up elements once
            var root = View.RootOfThisView;

            _languageDropdown = root.Q<DropdownField>("Settings_LanguageDropdown");
            _modeToggle = root.Q<Toggle>("Settings_ModeToggle");
            _modeTitleLabel = root.Q<Label>("Settings_ModeTitle");
            _modeHintLabel = root.Q<Label>("Settings_ModeHint");
            _supportPlatformToggle = root.Q<Toggle>("Settings_SupportPlatformToggle");
            _vignetteToggle = root.Q<Toggle>("Settings_VignetteToggle"); 

            _moveSpeed = root.Q<Slider>("Settings_MoveSpeed");
            _turnSpeed = root.Q<Slider>("Settings_TurnSpeed");
            _turnAngle = root.Q<Slider>("Settings_TurnAngle");

            _disclaimerButton = root.Q<Button>("Settings_DisclaimerButton");
            _disclaimerButton.RegisterCallback<ClickEvent>(_ => ResetDisclaimer());

            // wire events -> placeholder handlers
            _languageDropdown.RegisterValueChangedCallback(e => OnLanguageChanged(e.newValue));
            _modeToggle.RegisterValueChangedCallback(e => OnModeToggled(e.newValue));
            _moveSpeed.RegisterValueChangedCallback(e => OnMoveSpeedChanged(e.newValue));
            _turnSpeed.RegisterValueChangedCallback(e => OnTurnSpeedChanged(e.newValue));
            _turnAngle.RegisterValueChangedCallback(e => OnTurnAngleChanged(e.newValue));
            _supportPlatformToggle.RegisterValueChangedCallback(e => OnSupportPlatformChanged(e.newValue));
            _vignetteToggle.RegisterValueChangedCallback(e => OnVignetteToggleChanged(e.newValue));

            // apply initial model values to UI
            ApplyModelToView();
        }

        public void OnVignetteToggleChanged(bool enabled)
        {
            //Model.VignetteEnabled = enabled;
            _pc.SetVignette(enabled);
            //Model.SaveDataAsync().Forget();
        }

        public void OnSupportPlatformChanged(bool enabled)
        {
            //Model.SupportPlatform = enabled;
            var p = Object.FindFirstObjectByType<SafetyPlatformHandler>();
            p.Toggle(enabled);
            //Model.SaveDataAsync().Forget(); // handled inside SafetyPlatformHandler
        }

        public void OnLanguageChanged(string language)
        {
            switch (language)
            {
                case "English":
                    Model.Language = "English";
                    break;
                case "German":
                    Model.Language = "German";
                    break;
                default:
                    Model.Language = "German";
                    break;
            }
            
            Model.SaveDataAsync().Forget();
            ServiceLocator.GetService<LocaleSwitcher>().SwitchLocale(Model.Language);
        }

        public void OnModeToggled(bool performanceMode)
        {
            Model.PerformanceMode = performanceMode;
            Model.SaveDataAsync().Forget();
            SetPerformanceMode(performanceMode);
        }

        public void OnMoveSpeedChanged(float value)
        {
            // saving handled in PlayerController, Model updated via event below
            _pc.MovementSpeed = value;
        }

        public void OnTurnSpeedChanged(float value)
        {
            _pc.SetTurnSpeed(value);
        }

        public void OnTurnAngleChanged(float value)
        {
            _pc.SetTurnAngle(value);
        }
        

        public void ApplyModelToView()
        {
            _languageDropdown.SetValueWithoutNotify(Model.Language);
            
            // mode
            _modeToggle.SetValueWithoutNotify(Model.PerformanceMode);
            _supportPlatformToggle.SetValueWithoutNotify(Model.SupportPlatform);
            _vignetteToggle.SetValueWithoutNotify(Model.VignetteEnabled);

            // sliders
            _moveSpeed.SetValueWithoutNotify(Model.MoveSpeed);
            _turnSpeed.SetValueWithoutNotify(Model.TurnSpeed);
            _turnAngle.SetValueWithoutNotify(Model.TurnAngle);
        }

        private void SetPerformanceMode(bool enable)
        {
            var name = enable ? "Performance" : "Beauty";
            var names = QualitySettings.names;
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] == name)
                {
                    QualitySettings.SetQualityLevel(i, true);
                    break;
                }
            }
            SetPostProcessing();
            SetMaximumTilesError();
        }

        private void SetPostProcessing()
        {
            UniversalAdditionalCameraData cameraData = Camera.main?.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData == null)
            {
                Debug.Log("No UniversalAdditionalCameraData found on camera.");
                return;
            }
            var userSettings = ServiceLocator.GetService<ConfigurationService>().LoadUserSettings<UserSettings>();
            cameraData.renderPostProcessing = !userSettings.PerformanceMode;
            Camera.main.farClipPlane = userSettings.PerformanceMode ? 2000 : 3000;
        }

        private void SetMaximumTilesError()
        {
            var userSettings = ServiceLocator.GetService<ConfigurationService>().LoadUserSettings<UserSettings>();
            float maxScreenError = userSettings.PerformanceMode ? 30f : 8f;
            CesiumTilesetUtilities.SetMaximumScreenSpaceErrorOnAllTilesets(maxScreenError);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_pc != null)
            {
                _pc.OnMoveSpeedChanged -= HandleMoveSpeedChanged;
                _pc.OnTurnSpeedChanged -= HandleTurnSpeedChanged;
                _pc.OnTurnAngleChanged -= HandleTurnAngleChanged;
                _pc.OnVignetteChanged -= HandleVignetteChanged;
                _platform.OnSafetyPlatformChanged -= HandleSafetyPlatformChanged;
            }
        }

        // event handlers

        private void HandleMoveSpeedChanged(float value)
        {
            Model.MoveSpeed = value;
            if (_moveSpeed != null) _moveSpeed.SetValueWithoutNotify(value);
        }

        private void HandleTurnSpeedChanged(float value)
        {
            Model.TurnSpeed = value;
            if (_turnSpeed != null) _turnSpeed.SetValueWithoutNotify(value);
        }

        private void HandleTurnAngleChanged(float value)
        {
            Model.TurnAngle = value;
            if (_turnAngle != null) _turnAngle.SetValueWithoutNotify(value);
        }

        private void HandleVignetteChanged(bool enabled)
        {
            Model.VignetteEnabled = enabled;
            if (_vignetteToggle != null) _vignetteToggle.SetValueWithoutNotify(enabled);
        }

        private void HandleSafetyPlatformChanged(bool enabled)
        {
            Model.SupportPlatform = enabled;
            if (_supportPlatformToggle != null) _supportPlatformToggle.SetValueWithoutNotify(enabled);
        }

        internal void ResetDisclaimer()
        {
            var configService = ServiceLocator.GetService<ConfigurationService>();
            configService.SaveUserSettingProperty<UserSettings, bool>(
                u => u.HideDisclaimer,
                false
            );
        }
    }
}