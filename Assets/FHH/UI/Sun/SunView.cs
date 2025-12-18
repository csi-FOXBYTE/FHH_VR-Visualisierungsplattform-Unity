using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Sun
{
    public sealed class SunView : ViewBase<SunPresenter>
    {
        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        public IntegerField YearField { get; internal set; }
        public SliderInt DateSlider { get; internal set; }
        public SliderInt TimeSlider { get; internal set; }

        public Label DateValueLabel { get; internal set; }
        public Label DateTooltipLabel { get; internal set; }
        public Label TimeValueLabel { get; internal set; }
        public Label TimeTooltipLabel { get; internal set; }

        public RadioButtonGroup StudiesGroup { get; internal set; }

        protected override UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var generator = ViewGeneratorBase<SunPresenter, SunView>.Create<SunViewGenerator>(this);
            return generator.GenerateViewAsync();
        }
    }
}