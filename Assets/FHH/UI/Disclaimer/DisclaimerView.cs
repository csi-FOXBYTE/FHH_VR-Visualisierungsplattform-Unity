using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Disclaimer
{
    public class DisclaimerView : ViewBase<DisclaimerPresenter>
    {
        protected override string LocalizationTableName => "General";
        
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(
            UIDocument uiDocument,
            VisualElement targetContainer)
        {
            var generator = ViewGeneratorBase<DisclaimerPresenter, DisclaimerView>
                .Create<DisclaimerViewGenerator>(this);

            return await generator.GenerateViewAsync();
        }
    }
}