using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Components.HmdPresenceMonitor;
using FHH.UI.Help;
using FHH.UI.MainMenu;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI
{
    public sealed class MenuBarPresenter 
        : PresenterBase<MenuBarPresenter, MenuBarView, MenuBarModel>
    {
        
        //private const string _childUpdateExampleKey = "MenuBar:ButtonClicked";
        protected override double DefaultCooldown => .25;
        private UIManager _uiManager;
        private HmdPresenceMonitorService _hmdPresenceMonitorService;
            

        public MenuBarPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            MenuBarModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model, perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            if (Model == null) Model = new MenuBarModel();
            await Model.InitializeAsync();
            try
            {
                _hmdPresenceMonitorService = ServiceLocator.GetService<HmdPresenceMonitorService>();
            }
            catch { }
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await base.InitializeAfterUiAsync();
            if (View == null) throw new InvalidOperationException("View is null.");
            
            View.MenuBarLeft = View.RootOfThisView?.Q<VisualElement>("MenuBar_Left");
            View.MenuBarRight = View.RootOfThisView?.Q<VisualElement>("MenuBar_Right");
            View.Hamburger = View.RootOfThisView?.Q<VisualElement>("MenuBar_HamburgerButton");
            View.Help = View.RootOfThisView?.Q<VisualElement>("MenuBar_HelpButton");
            View.VrSwitch = View.RootOfThisView?.Q<VisualElement>("MenuBar_VRSwitchButton");
            //View.ViewModeGroup = View.RootOfThisView?.Q<VisualElement>("MenuBar_ViewModeGroup");

            _uiManager = ServiceLocator.GetService<UIManager>();

            View.Hamburger?.RegisterCallback<ClickEvent>(e =>
            {
                ToggleMainMenuAsync().Forget();
            });
            View.Help?.RegisterCallback<ClickEvent>(e =>
            {
                ToggleHelpAsync().Forget();
            });
            View.VrSwitch?.RegisterCallback<ClickEvent>(e =>
            {
                ToggleVrModeAsync().Forget();
            });
        }

        private async UniTask ToggleVrModeAsync()
        {
            if (_hmdPresenceMonitorService == null) return;
            var vrEnabled = _hmdPresenceMonitorService.IsDeviceEnabled;
            _hmdPresenceMonitorService.SetVrEnabled(!vrEnabled);
            await UniTask.Yield();
            if (vrEnabled)
            {
                Camera.main.fieldOfView = 60f;
            }
            else
            {
                Camera.main.fieldOfView = _hmdPresenceMonitorService.CameraFoV;
            }
            await UniTask.WaitForEndOfFrame();
            _uiManager.ToggleUIMode();
            await UniTask.CompletedTask;
        }

        private async UniTask ToggleHelpAsync()
        {
            if (_uiManager.IsShown<MainMenuPresenter>())
            {
                await _uiManager.HideAsync<MainMenuPresenter>();
                return;
            }

            await _uiManager.HideRegionAsync(UIProjectContext.UIRegion.Content1);

            var windowOptions = new WindowOptions
            {
                Region = UIProjectContext.UIRegion.Content1,
                CenterScreen = false,
                FadeInSec = 0.15f,
                FadeOutSec = 0.15f,
                StyleSheet = Resources.Load<StyleSheet>("MainMenu")
            };

            //await _uiManager.ShowWindowAsync<
            //    HelpPresenter,
            //    HelpView,
            //    HelpModel
            //>(
            //    new HelpModel(),
            //    windowOptions
            //);

            _uiManager.ShowWindowAsync<MainMenuPresenter, MainMenuView, MainMenuModel>(
                new MainMenuModel { InitialSection = MainMenuSection.Help },
                windowOptions,
                null, null).Forget();

            await UniTask.CompletedTask;
        }

        private async UniTask ToggleMainMenuAsync()
        {
            if (_uiManager.IsShown<MainMenuPresenter>())
            {
                await _uiManager.HideAsync<MainMenuPresenter>();
                return;
            }

            await _uiManager.HideRegionAsync(UIProjectContext.UIRegion.Content1);

            _uiManager.ShowWindowAsync<
                MainMenuPresenter,
                MainMenuView,
                MainMenuModel
            >(
                new MainMenuModel(),
                new WindowOptions
                {
                    Region = UIProjectContext.UIRegion.Content1,
                    CenterScreen = false,
                    FadeInSec = 0.15f,
                    FadeOutSec = 0.15f,
                    StyleSheet = Resources.Load<StyleSheet>("MainMenu") 
                }
            ).Forget();
            await UniTask.CompletedTask;
        }

        // (Optional) receive updates from child presenters
        //protected override async UniTask OnChildUpdate(object payload)
        //{
        //    await base.OnChildUpdate(payload);
        //    Debug.Log($"[MenuBarPresenter] OnChildUpdate received: {payload}");
        //}
    }
}