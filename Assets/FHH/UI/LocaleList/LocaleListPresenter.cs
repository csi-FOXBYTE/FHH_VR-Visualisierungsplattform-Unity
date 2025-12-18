using Cysharp.Threading.Tasks;
//using Foxbyte.Core.Localization.Utilities;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.LocaleList
{
   /* public class LocaleListPresenter : PresenterBase<LocaleListPresenter, LocaleListView, NoModel>
    {
        public LocaleListPresenter(GameObject targetGameobjectForView, UIDocument uidoc, StyleSheet styleSheet, VisualElement target, NoModel model = null, 
            RequiredPermissions perms = null) : base(targetGameobjectForView, uidoc, styleSheet, target, model, perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            View = TargetGameObjectForView.GetComponent<LocaleListView>() ?? TargetGameObjectForView.AddComponent<LocaleListView>();
            View.Presenter = this;
            //if (_localeSwitcher == null)
            //{
            //    _localeSwitcher = TargetGameObjectForView.GetComponent<LocaleSwitcher>();
            //}
            await UniTask.Yield();
        }

        public async UniTaskVoid LocaleClickHandler(string localeName)
        {
            //var localeSwitcher = ServiceLocator.Instance.GetService<LocaleSwitcher>();
            //localeSwitcher.SwitchLocale(localeName);
            await UniTask.Yield();
        }
    }*/
}