using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace FHH.UI.Help
{
    public sealed class HelpViewGenerator : ViewGeneratorBase<HelpPresenter, HelpView>
    {
        private readonly List<LocalizedString> _keyLocalizedStrings = new List<LocalizedString>();

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            var root = new VisualElement { name = "HelpRoot" };
            root.AddToClassList("help-root");
            root.AddToClassList("app-background3");

            var title = new Label { name = "Help_ControlsTitle" };
            title.AddToClassList("help-title");
            root.Add(title);

            var scroll = new ScrollView(ScrollViewMode.Vertical) { name = "HelpScroll" }; 
            scroll.AddToClassList("help-scroll");
            
            root.Add(scroll);

            scroll.Add(BuildSectionHeader("Help_BothSectionTitle"));
            scroll.Add(BuildRow("H","Help_Home"));
            scroll.Add(BuildRow("F","Help_MovementMode"));
            scroll.Add(BuildRow("F8","Help_UiModeSwitch"));
            scroll.Add(BuildSeparator());

            scroll.Add(BuildSectionHeader("Help_VRSectionTitle"));
            scroll.Add(BuildRow("P","Help_Platform"));
            scroll.Add(BuildRow("T","Help_SnapTurn"));
            scroll.Add(BuildRow("V","Help_Vignette"));
            scroll.Add(BuildRow("M","Help_SnapUi"));
            scroll.Add(BuildRowLoc("Key_LeftControllerTrigger", "Help_TeleportVR")); 
            scroll.Add(BuildRowLoc("Key_RightControllerTrigger", "Help_UiSelectVR")); 
            scroll.Add(BuildRowLoc("Key_RightControllerStick","Help_TurnFlyVR")); 
            scroll.Add(BuildSeparator());

            scroll.Add(BuildSectionHeader("Help_DesktopSectionTitle"));
            scroll.Add(BuildRow("WASDQE","Help_MovementDesktop"));
            scroll.Add(BuildRowLoc("Key_RightMouseHold", "Help_CameraRotationDesktop")); 
            scroll.Add(BuildRowLoc("Key_Ctrl", "Help_TeleportDesktop"));  
            scroll.Add(BuildRowLoc("Key_Attribute", "Help_AttributeDesktop"));  

            await UniTask.Yield();
            return root;
        }

        private VisualElement BuildSectionHeader(string locName)
        {
            var header = new Label { name = locName };
            header.AddToClassList("help-section-title");
            return header;
        }

        private VisualElement BuildRow(string keyText, string descLocName)
        {
            var row = new VisualElement();
            row.AddToClassList("help-row");

            var key = new Label(keyText);
            key.AddToClassList("help-key");
            key.AddToClassList("i18n-skip"); // keep literal

            var desc = new Label { name = descLocName };
            desc.AddToClassList("help-desc");

            row.Add(key);
            row.Add(desc);
            return row;
        }

        private VisualElement BuildSeparator()
        {
            var sep = new VisualElement();
            sep.AddToClassList("help-separator");
            return sep;
        }

        private VisualElement BuildRowLoc(string keyLocName, string descLocName)
        {
            var row = new VisualElement();
            row.AddToClassList("help-row");
            
            var key = new Label(); // text comes from localization
            key.AddToClassList("help-key");
            key.AddToClassList("i18n-skip"); // use manual localization

            var keyLoc = new LocalizedString("Help", keyLocName);
            keyLoc.StringChanged += s => key.text = s;
            _keyLocalizedStrings.Add(keyLoc);
            _ = keyLoc.GetLocalizedStringAsync(); // trigger initial resolve

            var desc = new Label { name = descLocName };
            desc.AddToClassList("help-desc");

            row.Add(key);
            row.Add(desc);
            return row;
        }
    }
}