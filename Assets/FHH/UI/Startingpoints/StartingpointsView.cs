using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace FHH.UI.Startingpoints
{
    public class StartingpointsView : ViewBase<StartingpointsPresenter>
    {
        protected override string LocalizationTableName => "General";
        
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(
            UIDocument uiDocument,
            VisualElement targetContainer)
        {
            var generator =
                ViewGeneratorBase<StartingpointsPresenter, StartingpointsView>
                    .Create<StartingpointsViewGenerator>(this);

            return await generator.GenerateViewAsync();
        }

        protected override void SelectedLocaleChanged(Locale locale)
        {
            base.SelectedLocaleChanged(locale);
        }
    }
}