using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Disclaimer
{
    public class DisclaimerViewGenerator
        : ViewGeneratorBase<DisclaimerPresenter, DisclaimerView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            // root overlay
            var root = new VisualElement
            {
                name = "DisclaimerRoot"
            };
            root.AddToClassList("disclaimer-root");
            root.AddToClassList("i18n-skip"); // container itself is not localized

            // central panel
            var panel = new VisualElement
            {
                name = "DisclaimerPanel"
            };
            panel.AddToClassList("disclaimer-panel");
            panel.AddToClassList("i18n-skip");
            panel.pickingMode = PickingMode.Ignore;
            root.Add(panel);

            var header = new VisualElement
            {
                name = "DisclaimerHeader"
            };
            header.AddToClassList("disclaimer-header");
            header.AddToClassList("i18n-skip");
            panel.Add(header);

            var closeButton = new Button
            {
                name = "DisclaimerCloseButton"
            };
            closeButton.text = string.Empty; // no text, icon only
            closeButton.AddToClassList("disclaimer-close-button");
            closeButton.AddToClassList("i18n-skip");

            // Icon inside the button
            var closeIconImage = new Image
            {
                name = "DisclaimerCloseIcon"
            };
            closeIconImage.AddToClassList("disclaimer-close-icon");
            closeIconImage.AddToClassList("i18n-skip");

            var closeTex = Resources.Load<Texture2D>("Icons/close"); // Resources/Icons/close.png
            if (closeTex != null)
            {
                closeIconImage.image = closeTex;
            }

            closeButton.Add(closeIconImage);
            header.Add(closeButton);



            // content wrapper (handles vertical centering)
            var contentWrapper = new VisualElement
            {
                name = "DisclaimerContentWrapper"
            };
            contentWrapper.AddToClassList("disclaimer-content-wrapper");
            contentWrapper.AddToClassList("i18n-skip");
            contentWrapper.pickingMode = PickingMode.Ignore;
            panel.Add(contentWrapper);

            // central text column
            var textColumn = new VisualElement
            {
                name = "DisclaimerTextColumn"
            };
            textColumn.AddToClassList("disclaimer-text-column");
            textColumn.AddToClassList("i18n-skip");
            textColumn.pickingMode = PickingMode.Ignore;
            contentWrapper.Add(textColumn);

            // headline
            var titleLabel = new Label
            {
                name = "Disclaimer_Title" // localization key
            };
            titleLabel.AddToClassList("disclaimer-title-label");
            titleLabel.pickingMode = PickingMode.Ignore;
            textColumn.Add(titleLabel);

            // description text (long paragraph)
            var descriptionLabel = new Label
            {
                name = "Disclaimer_Description" // localization key
            };
            descriptionLabel.AddToClassList("disclaimer-body-label");
            textColumn.Add(descriptionLabel);

            // second headline (Datenschutzhinweise)
            var privacyTitleLabel = new Label
            {
                name = "Disclaimer_PrivacyTitle" // localization key
            };
            privacyTitleLabel.AddToClassList("disclaimer-title-label");
            textColumn.Add(privacyTitleLabel);

            // privacy text
            var privacyScrollView = new ScrollView
            {
                name = "Disclaimer_PrivacyScrollView"
            };
            privacyScrollView.AddToClassList("disclaimer-privacy-scrollview");

            var privacyBodyLabel = new Label
            {
                name = "Disclaimer_PrivacyText" // localization key
            };
            privacyBodyLabel.enableRichText = true;
            privacyBodyLabel.AddToClassList("disclaimer-body-label");
            
            privacyScrollView.Add(privacyBodyLabel);

            textColumn.Add(privacyScrollView);
            //textColumn.Add(privacyBodyLabel);

            // footer (bottom-left area)
            var footer = new VisualElement
            {
                name = "DisclaimerFooter"
            };
            footer.AddToClassList("disclaimer-footer");
            footer.AddToClassList("i18n-skip");
            panel.Add(footer);

            // "Nicht mehr anzeigen" button (localized via inner label)
            var neverShowButton = new Button
            {
                name = "DisclaimerNeverShowButton"
            };
            neverShowButton.AddToClassList("btn");
            neverShowButton.AddToClassList("btn-primary");
            neverShowButton.AddToClassList("disclaimer-never-show-button");
            neverShowButton.AddToClassList("i18n-skip");

            var neverShowLabel = new Label
            {
                name = "Disclaimer_NeverShowAgain" // localization key for button text
            };
            neverShowLabel.AddToClassList("disclaimer-never-show-label");
            neverShowButton.Add(neverShowLabel);

            footer.Add(neverShowButton);

            return await UniTask.FromResult(root);
        }
    }
}