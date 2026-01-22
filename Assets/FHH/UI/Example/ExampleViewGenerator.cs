using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.Example
{
    public sealed class ExampleViewGenerator : ViewGeneratorBase<ExamplePresenter, ExampleView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            await UniTask.Yield();

            // Root container
            var root = new VisualElement
            {
                name = "ExampleRoot"
            };

            // Use your global utilities (.col, .center) + button classes.
            root.AddToClassList("col");
            root.AddToClassList("center");
            root.style.flexGrow = 1;

            // Center button (name doubles as localization key)
            var helloButton = new Button
            {
                name = "ExampleHelloButton",
                text = "Hello Hamburg" // will be replaced if localization table has key "ExampleHelloButton"
            };
            helloButton.AddToClassList("btn");
            helloButton.AddToClassList("btn-primary");

            root.Add(helloButton);
            return root;
        }
    }
}