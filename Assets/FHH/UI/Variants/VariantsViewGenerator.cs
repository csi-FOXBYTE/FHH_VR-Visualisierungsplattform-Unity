using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Variants
{
    public sealed class VariantsViewGenerator : ViewGeneratorBase<VariantsPresenter, VariantsView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            // Root panel
            var root = new VisualElement
            {
                name = "VariantsRoot"
            };
            root.AddToClassList("variants-window");
            root.AddToClassList("col");
            root.AddToClassList("i18n-skip"); // skip localization

            // Header row
            var header = new VisualElement
            {
                name = "VariantsHeader"
            };
            header.AddToClassList("variants-header");
            header.AddToClassList("row");
            header.AddToClassList("i18n-skip");

            var titleLabel = new Label
            {
                name = "VariantsTitle"
            };
            titleLabel.AddToClassList("variants-title");
            header.Add(titleLabel);

            root.Add(header);

            // Body
            var body = new VisualElement
            {
                name = "VariantsBody"
            };
            body.AddToClassList("variants-body");
            body.AddToClassList("col");
            body.AddToClassList("i18n-skip");

            // RadioButtonGroup for the variants
            var radioGroup = new RadioButtonGroup
            {
                name = "VariantsRadioGroup"
            };
            radioGroup.label = string.Empty;
            radioGroup.AddToClassList("variants-radio-group");
            radioGroup.AddToClassList("i18n-skip");

            body.Add(radioGroup);
            root.Add(body);

            await UniTask.CompletedTask;
            return root;
        }
    }
}