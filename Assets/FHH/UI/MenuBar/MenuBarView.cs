using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI
{
    public sealed class MenuBarView : ViewBase<MenuBarPresenter>
    {
        protected override string LocalizationTableName => "General";

        // The menu bar is persistent and not modal.
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;
        public VisualElement MenuBarLeft { get; internal set; }
        public VisualElement MenuBarRight { get; internal set; }
        public VisualElement Hamburger { get; internal set; }
        public VisualElement Help { get; internal set; }
        public VisualElement VrSwitch { get; internal set; }
        //public VisualElement ViewModeGroup { get; internal set; }

        // Optional: expose important controls to Presenter (found via Q in presenter too).
        //public VisualElement MenuBarRoot { get; internal set; }
        //public Button MenuButton { get; internal set; }

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var gen = ViewGeneratorBase<MenuBarPresenter, MenuBarView>.Create<MenuBarViewGenerator>(this);
            var root = await gen.GenerateViewAsync();

            // Keep references for Presenter convenience.
            //MenuBarRoot = root;
            //MenuButton  = root?.Q<Button>("MenuBar_Button");
            return root;
        }
    }
}