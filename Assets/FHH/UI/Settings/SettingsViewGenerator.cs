using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Foxbyte.Presentation;

namespace FHH.UI.Settings
{
    public sealed class SettingsViewGenerator 
        : ViewGeneratorBase<SettingsPresenter, SettingsView>
    {
        // Common slider factory (0.1 .. 1)
        private Slider MakeSlider(string name)
        {
            var s = new Slider(0.1f, 2f) { name = name };
            s.lowValue  = 0.1f;
            s.highValue = 2f;
            s.showInputField = false;
            s.AddToClassList("settings-slider");
            return s;
        }

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            await UniTask.Yield();

            var root = new VisualElement { name = "SettingsRoot" };
            root.AddToClassList("settings-root");
            root.AddToClassList("col");

            var scroll = new ScrollView(ScrollViewMode.Vertical) { name = "Settings_Scroll" };
            scroll.AddToClassList("settings-scroll");

            // language
            var languageSection = new VisualElement { name = "Settings_LanguageSection" };
            languageSection.AddToClassList("settings-section");

            var languageHeader = new Label { name = "Settings_LanguageHeader", text = "Sprache" };
            languageHeader.AddToClassList("settings-header");

            var languageRow = new VisualElement { name = "Settings_LanguageRow" };
            languageRow.AddToClassList("row");
            languageRow.AddToClassList("settings-row");

            //var languageLabel = new Label { name = "Settings_LanguageLabel", text = "Sprache" };
            //languageLabel.AddToClassList("settings-label");

            var languageDropdown = new DropdownField(new List<string> { "German", "English" }, 0)
            {
                name = "Settings_LanguageDropdown"
            };
            languageDropdown.AddToClassList("settings-dropdown");
            //languageRow.Add(languageLabel);
            languageRow.Add(languageDropdown);

            languageSection.Add(languageHeader);
            languageSection.Add(languageRow);

            
            // VR options
            var vrOptionsSection = new VisualElement { name = "Settings_VrOptionsSection" }; 
            vrOptionsSection.AddToClassList("settings-section");

            var vrOptionsHeader = new Label { name = "Settings_VrOptionsHeader", text = "VR-Optionen" };
            vrOptionsHeader.AddToClassList("settings-header");

            // One toggle instead of two radios
            var modeRow = new VisualElement { name = "Settings_ModeRow" };
            modeRow.AddToClassList("settings-mode-row");

            var modeTitle = new Label { name = "Settings_ModeTitle", text = "Performance-Modus / Qualitäts-Modus" };
            modeTitle.AddToClassList("settings-subtitle");

            var modeToggleWrap = new VisualElement { name = "Settings_ModeToggleWrap" };
            modeToggleWrap.AddToClassList("row");

            var modeToggle = new Toggle { name = "Settings_ModeToggle" };  // empty text; we show hints via labels
            modeToggle.AddToClassList("settings-toggle");

            //var modeHint = new Label { name = "Settings_ModeHint", text = "90 fps Minimum" };
            //modeHint.AddToClassList("settings-hint");

            modeToggleWrap.Add(modeToggle);
            //modeToggleWrap.Add(modeHint);

            modeRow.Add(modeTitle);
            modeRow.Add(modeToggleWrap);

            var supportPlatformRow = new VisualElement { name = "Settings_SupportPlatformRow" };
            supportPlatformRow.AddToClassList("settings-mode-row");

            var supportPlatformTitle = new Label { name = "Settings_SupportPlatformTitle", text = "Plattform-Unterstützung" };
            supportPlatformTitle.AddToClassList("settings-subtitle");

            var supportPlatformToggleWrap = new VisualElement { name = "Settings_SupportPlatformToggleWrap" };
            supportPlatformToggleWrap.AddToClassList("row");

            var supportPlatformToggle = new Toggle { name = "Settings_SupportPlatformToggle" };
            supportPlatformToggle.AddToClassList("settings-toggle");

            supportPlatformToggleWrap.Add(supportPlatformToggle);
            supportPlatformRow.Add(supportPlatformTitle);
            supportPlatformRow.Add(supportPlatformToggleWrap);

            var vignetteRow = new VisualElement { name = "Settings_VignetteRow" };
            vignetteRow.AddToClassList("settings-mode-row");

            var vignetteTitle = new Label { name = "Settings_VignetteTitle", text = "Vignette" };
            vignetteTitle.AddToClassList("settings-subtitle");

            var vignetteToggleWrap = new VisualElement { name = "Settings_VignetteToggleWrap" };
            vignetteToggleWrap.AddToClassList("row");

            var vignetteToggle = new Toggle { name = "Settings_VignetteToggle" };
            vignetteToggle.AddToClassList("settings-toggle");

            vignetteToggleWrap.Add(vignetteToggle);
            vignetteRow.Add(vignetteTitle);
            vignetteRow.Add(vignetteToggleWrap);

            vrOptionsSection.Add(vrOptionsHeader);
            vrOptionsSection.Add(modeRow);
            vrOptionsSection.Add(supportPlatformRow);
            vrOptionsSection.Add(vignetteRow);

            
            // controls
            var controlSection = new VisualElement { name = "Settings_ControlSection" };
            controlSection.AddToClassList("settings-section");

            var controlHeader = new Label { name = "Settings_ControlHeader", text = "Steuerung" };
            controlHeader.AddToClassList("settings-header");
            controlSection.Add(controlHeader);

            // Move speed row
            controlSection.Add(BuildSliderRow(
                titleName: "Settings_MoveTitle",
                titleText: "Fortbewegung",
                labelName: "Settings_MoveSpeedLabel",
                labelText: "Geschwindigkeit",
                sliderName: "Settings_MoveSpeed"));

            // Turn speed row
            controlSection.Add(BuildSliderRow(
                titleName: "Settings_TurnTitle",
                titleText: "Drehen",
                labelName: "Settings_TurnSpeedLabel",
                labelText: "Geschwindigkeit",
                sliderName: "Settings_TurnSpeed"));

            // Turn angle row
            var angleRow = new VisualElement { name = "Settings_AngleRow" };
            angleRow.AddToClassList("settings-block");

            var angleTitle = new Label { name = "Settings_AngleTitle", text = "Winkel" };
            angleTitle.AddToClassList("settings-subtitle");

            var angleSliderRow = new VisualElement();
            angleSliderRow.AddToClassList("settings-slider-row");

            var angleSlider = MakeSlider("Settings_TurnAngle");
            var angleMin = new Label { name = "Settings_AngleMinLabel", text = "Stufenlos" };
            var angleMax = new Label { name = "Settings_AngleMaxLabel", text = "Winkel [Grad]" };
            angleSlider.lowValue = 0f;
            angleSlider.highValue = 90f;

            angleSliderRow.Add(angleMin);
            angleSliderRow.Add(angleSlider);
            angleSliderRow.Add(angleMax);

            angleRow.Add(angleTitle);
            angleRow.Add(angleSliderRow);
            controlSection.Add(angleRow);

            // Compose
            //root.Add(languageSection);
            //root.Add(qualitySection);
            //root.Add(controlSection);

            scroll.Add(languageSection); 
            scroll.Add(vrOptionsSection);
            scroll.Add(controlSection);

            var disclaimerButton = new Button();
            disclaimerButton.name = "Settings_DisclaimerButton";
            disclaimerButton.AddToClassList("settings-disclaimer-button");
            
            scroll.Add(disclaimerButton);

            root.Add(scroll);

            return root;
        }

        private VisualElement BuildSliderRow(string titleName, string titleText, string labelName, string labelText, string sliderName)
        {
            var block = new VisualElement { name = $"{titleName}_Block" };
            block.AddToClassList("settings-block");

            var title = new Label { name = titleName, text = titleText };
            title.AddToClassList("settings-subtitle");

            var rowLabel = new Label { name = labelName, text = labelText };
            rowLabel.AddToClassList("settings-field-label");

            var sliderRow = new VisualElement();
            sliderRow.AddToClassList("settings-slider-row");

            var min = new Label { name = $"{sliderName}_MinLabel", text = "Langsam" };
            var slider = MakeSlider(sliderName);
            var max = new Label { name = $"{sliderName}_MaxLabel", text = "Schnell" };

            slider.lowValue = 0f;
            slider.highValue = 1f;

            sliderRow.Add(min);
            sliderRow.Add(slider);
            sliderRow.Add(max);

            block.Add(title);
            block.Add(rowLabel);
            block.Add(sliderRow);
            return block;
        }
    }
}