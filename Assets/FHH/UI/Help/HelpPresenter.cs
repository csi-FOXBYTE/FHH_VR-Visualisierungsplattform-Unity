using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Help
{
    public sealed class HelpPresenter : PresenterBase<HelpPresenter, HelpView, HelpModel>
    {
        public HelpPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            HelpModel model = null,
            Foxbyte.Core.Services.Permission.RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model, perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            Model ??= new HelpModel();
            await Model.InitializeAsync();
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}