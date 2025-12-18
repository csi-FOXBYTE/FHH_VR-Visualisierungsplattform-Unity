using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Variants
{
    public sealed class VariantsView : ViewBase<VariantsPresenter>
    {
        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside => true;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var generator = ViewGeneratorBase<VariantsPresenter, VariantsView>.Create<VariantsViewGenerator>(this);
            return await generator.GenerateViewAsync();
        }

        /// <summary>
        /// Update the radio button group with the given variant names and selection.
        /// Called from the presenter.
        /// </summary>
        public void SetVariants(System.Collections.Generic.IReadOnlyList<string> names, int selectedIndex)
        {
            var radioGroup = RootOfThisView.Q<RadioButtonGroup>("VariantsRadioGroup");
            if (radioGroup == null)
            {
                Debug.LogError("VariantsView: RadioButtonGroup 'VariantsRadioGroup' not found.");
                return;
            }

            radioGroup.choices = names ?? System.Array.Empty<string>();
            radioGroup.value = (names != null && selectedIndex >= 0 && selectedIndex < names.Count)
                ? selectedIndex
                : -1;
        }
    }
}