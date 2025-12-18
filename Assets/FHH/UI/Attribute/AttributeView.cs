using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Attribute
{
    public sealed class AttributeView : ViewBase<AttributePresenter>
    {
        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(
            UIDocument uiDocument,
            VisualElement targetContainer)
        {
            var generator = ViewGeneratorBase<AttributePresenter, AttributeView>
                    .Create<AttributeViewGenerator>(this);

            return await generator.GenerateViewAsync();
        }

        internal void Close()
        {
            Presenter.HideAsync().Forget();
        }
    }
}