using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Models;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Presentation;

namespace FHH.UI.Settings
{
    public sealed class SettingsModel : PresenterModelBase
    {
        public string Language { get; set; } = "German";

        // true = Performance, false = Quality
        public bool PerformanceMode { get; set; } = true;

        public float MoveSpeed   { get; set; } = 0.5f;
        public float TurnSpeed   { get; set; } = 0.5f;
        public float TurnAngle   { get; set; } = 15f;
        public bool VignetteEnabled { get; set; }
        public bool SupportPlatform { get; set; }

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            await LoadDataAsync();
        }
        
        public override async UniTask LoadDataAsync()
        {
            var userSettings = ServiceLocator.GetService<ConfigurationService>().LoadUserSettings<UserSettings>();

            Language = userSettings.Language;
            PerformanceMode = userSettings.PerformanceMode;
            MoveSpeed = userSettings.MoveSpeed;
            TurnSpeed = userSettings.TurnSpeed;
            TurnAngle = userSettings.TurnAngle;
            VignetteEnabled = userSettings.VignetteEnabled;
            SupportPlatform = userSettings.SupportPlatform;
            
            await UniTask.CompletedTask;
        }

        public override async UniTask SaveDataAsync()
        {
            var configService = ServiceLocator.GetService<ConfigurationService>();

            configService.SaveUserSettingProperty<UserSettings, string>(u => u.Language, Language);
            configService.SaveUserSettingProperty<UserSettings, bool>(u => u.PerformanceMode, PerformanceMode);
            configService.SaveUserSettingProperty<UserSettings, float>(u => u.MoveSpeed, MoveSpeed);
            configService.SaveUserSettingProperty<UserSettings, float>(u => u.TurnSpeed, TurnSpeed);
            configService.SaveUserSettingProperty<UserSettings, float>(u => u.TurnAngle, TurnAngle);
            configService.SaveUserSettingProperty<UserSettings, bool>(u => u.VignetteEnabled, VignetteEnabled);
            configService.SaveUserSettingProperty<UserSettings, bool>(u => u.SupportPlatform, SupportPlatform);

            await UniTask.CompletedTask;
        }
    }
}