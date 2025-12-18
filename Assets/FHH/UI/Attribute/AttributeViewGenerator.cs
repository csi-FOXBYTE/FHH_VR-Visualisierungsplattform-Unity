using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Attribute
{
    public sealed class AttributeViewGenerator
        : ViewGeneratorBase<AttributePresenter, AttributeView>
    {
        private const string I18nSkipClass = "i18n-skip";

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            // Root card
            var root = new VisualElement();
            root.AddToClassList("attribute-root");
            root.AddToClassList("col"); // from FHH.uss

            var closeButton = CreateCloseButton();
            root.Add(closeButton);

            // Title label – only element that should be localized
            var titleLabel = new Label("Attribute")
            {
                name = "AttributeTitle"
            };
            titleLabel.AddToClassList("attribute-title");
            root.Add(titleLabel);

            // Scroll area for attributes
            var scroll = new ScrollView
            {
                name = "AttributeScroll"
            };
            scroll.AddToClassList("attribute-scroll");
            scroll.AddToClassList(I18nSkipClass); // skip scroll container itself
            scroll.style.flexGrow = 1f;
            root.Add(scroll);

            // Build rows from model data (keys and values are not localized)
            var attributes = Presenter.GetAttributes();
            if (attributes != null)
            {
                foreach (var kvp in attributes)
                {
                    var row = CreateAttributeRow(kvp.Key, kvp.Value);
                    scroll.Add(row);
                }
            }
            closeButton.BringToFront();

            await UniTask.CompletedTask;
            return root;
        }

        private VisualElement CreateAttributeRow(string key, string value)
        {
            var row = new VisualElement();
            row.AddToClassList("attribute-row");
            row.AddToClassList(I18nSkipClass);

            // Top line: key label + underline
            var labelRow = new VisualElement();
            labelRow.AddToClassList("attribute-label-row");
            labelRow.AddToClassList(I18nSkipClass);

            var keyLabel = new Label(key);
            keyLabel.AddToClassList("attribute-key-label");
            keyLabel.AddToClassList(I18nSkipClass);

            var underline = new VisualElement();
            underline.AddToClassList("attribute-key-underline");
            underline.AddToClassList(I18nSkipClass);

            labelRow.Add(keyLabel);
            labelRow.Add(underline);
            
            var valueField = new Label();
            valueField.text = value ?? string.Empty;
            valueField.AddToClassList("attribute-value-field");
            valueField.AddToClassList(I18nSkipClass);

            row.Add(labelRow);
            row.Add(valueField);

            return row;
        }

        private Button CreateCloseButton()
        {
            var button = new Button();
            button.AddToClassList("attribute-close-button");
            button.AddToClassList(I18nSkipClass);

            // icon-only, no text
            button.text = string.Empty;

            button.clicked += OnCloseClicked;
            return button;
        }

        private void OnCloseClicked()
        {
            View?.Close();
        }
    }
}