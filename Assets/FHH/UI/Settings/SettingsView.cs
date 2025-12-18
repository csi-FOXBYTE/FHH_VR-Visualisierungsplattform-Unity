using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Settings
{
    public sealed class SettingsView : ViewBase<SettingsPresenter>
    {
        protected override string LocalizationTableName => "Settings";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var gen = ViewGeneratorBase<SettingsPresenter, SettingsView>.Create<SettingsViewGenerator>(this);
            return gen.GenerateViewAsync();
        }
    }
}