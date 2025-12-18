using System;
using Cysharp.Threading.Tasks;
using FHH.Logic;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Intro
{
    public class IntroPresenter : PresenterBase<IntroPresenter,IntroView, NoModel>
    {
        public IntroPresenter(GameObject targetGameobjectForView, UIDocument uidoc, StyleSheet styleSheet, VisualElement target, NoModel model = null, 
            RequiredPermissions perms = null) : base(targetGameobjectForView, uidoc, styleSheet, target, model, perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            View = TargetGameObjectForView.GetComponent<IntroView>() ?? TargetGameObjectForView.AddComponent<IntroView>();
            View.Presenter = this;

            // test start
            // call the UIManager with the RequiredPermissions to show the view
            // the current user must have the required permissions to access the view
            if (Permissions.IsAllowed(ServiceLocator.GetService<PermissionService>()))
            {
                ULog.Info("Test: Got Permissions");
            }
            else
            {
                ULog.Info("Test: Permissions not granted");
            }
            // test end

            await UniTask.Yield();
        }
        
        public override async UniTask InitializeAfterUiAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            await ((IntroView)View).FadeOutAsync();
        }
    }
}