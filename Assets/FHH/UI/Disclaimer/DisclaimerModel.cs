using Cysharp.Threading.Tasks;
using FHH.Logic;
using Foxbyte.Presentation;
using FHH.Logic.Models;
using Foxbyte.Core.Services.ConfigurationService;

namespace FHH.UI.Disclaimer
{
    public class DisclaimerModel : PresenterModelBase
    {
        public bool HideDisclaimer { get; private set; }

        public override async UniTask LoadDataAsync()
        {
            var userSettings = ServiceLocator
                .GetService<ConfigurationService>()
                .LoadUserSettings<UserSettings>();

            HideDisclaimer = userSettings.HideDisclaimer;

            await UniTask.CompletedTask;
        }

        public async UniTask SaveHideDisclaimerAsync(bool hide)
        {
            HideDisclaimer = hide;

            var configService = ServiceLocator.GetService<ConfigurationService>();
            configService.SaveUserSettingProperty<UserSettings, bool>(
                u => u.HideDisclaimer,
                hide
            );

            await UniTask.CompletedTask;
        }
    }
}