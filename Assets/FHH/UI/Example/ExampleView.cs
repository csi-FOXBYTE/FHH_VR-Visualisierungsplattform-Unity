using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Example
{
    public sealed class ExampleView : ViewBase<ExamplePresenter>
    {
        // Used by localization auto-binding: element.name == localization key
        protected override string LocalizationTableName => "General";

        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            await UniTask.Yield();

            // Primary path: build UI in code via ViewGenerator
            var gen = ViewGeneratorBase<ExamplePresenter, ExampleView>.Create<ExampleViewGenerator>(this);
            return await gen.GenerateViewAsync();

            // Alternative path: instantiate UXML from Resources
            //VisualTreeAsset vta = Resources.Load<VisualTreeAsset>("ExampleWithBuilder");
            //TemplateContainer ui = vta.Instantiate();
            //return ui;
        }
    }
}