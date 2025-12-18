using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.MainMenu
{
    public sealed class MainMenuView : ViewBase<MainMenuPresenter>
    {
        protected override string LocalizationTableName => "General";

        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var gen = ViewGeneratorBase<MainMenuPresenter, MainMenuView>.Create<MainMenuViewGenerator>(this);
            return gen.GenerateViewAsync();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Presenter?.OnMainMenuClosed();
        }
    }
}