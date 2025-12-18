using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System.Threading;
using FHH.Logic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Disclaimer
{
    public class DisclaimerPresenter
        : PresenterBase<DisclaimerPresenter, DisclaimerView, DisclaimerModel>, IConditionalViewPresenter
    {
        public bool ShouldBuildView { get; private set; } = true;

        public DisclaimerPresenter(
            GameObject targetGameObjectForView,
            UIDocument uiDocument,
            StyleSheet styleSheet,
            VisualElement targetContainer,
            DisclaimerModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameObjectForView, uiDocument, styleSheet, targetContainer,
                new DisclaimerModel(), perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            await Model.LoadDataAsync();

            // decide if the UI should be shown
            ShouldBuildView = !Model.HideDisclaimer;
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await base.InitializeAfterUiAsync();
            await WireUpUiAsync();
        }

        private async UniTask WireUpUiAsync()
        {
            var neverShowButton = View.RootOfThisView.Q<Button>("DisclaimerNeverShowButton");
            if (neverShowButton == null)
                return;

            neverShowButton.RegisterCallback<ClickEvent>(_ =>
            {
                UniTask.Void(async () =>
                {
                    await RunGuardedAsync(
                        neverShowButton,
                        OnNeverShowAgainClickedAsync,
                        DefaultCooldown,
                        CancellationToken.None
                    );
                });
            });

            var closeButton = View.RootOfThisView.Q<Button>("DisclaimerCloseButton");
            if (closeButton == null)
                return;
            closeButton.RegisterCallback<ClickEvent>(_ =>
            {
                UniTask.Void(async () =>
                {
                    await RunGuardedAsync(
                        closeButton,
                        OnCloseClicked,
                        DefaultCooldown,
                        CancellationToken.None
                    );
                });
            });
                

            await UniTask.CompletedTask;
        }

        private async UniTask OnNeverShowAgainClickedAsync(CancellationToken ct)
        {
            await Model.SaveHideDisclaimerAsync(true);

            // close / remove disclaimer view
            if (View != null && View.RootOfThisView != null)
            {
                View.RootOfThisView.RemoveFromHierarchy();
            }
        }

        private async UniTask OnCloseClicked(CancellationToken ct)
        {
            var uiManager = ServiceLocator.GetService<UIManager>();
            uiManager.HideAsync<DisclaimerPresenter>().Forget();
            
            await UniTask.CompletedTask;
        }
    }
}