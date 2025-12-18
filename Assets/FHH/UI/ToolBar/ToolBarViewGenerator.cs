using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using Foxbyte.Presentation.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.ToolBar
{
    public class ToolBarViewGenerator : ViewGeneratorBase<ToolBarPresenter, ToolBarView>
    {
        private const string RootName = "TB_Root";
        private const string StackName = "TB_Stack";

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            // Mount inside Sidebar
            var root = new VisualElement { name = RootName };
            root.AddToClassList("tb-root");

            var stack = new VisualElement { name = StackName };
            stack.AddToClassList("tb-stack");
            root.Add(stack);

            CreateIconButton(stack, "TB_StartingPoints", "location");

            CreateIconButton(stack, "TB_WalkFly", "pedestrian_view");

            CreateIconButton(stack, "TB_SpeedMode", "walk");

            CreateIconButton(stack, "TB_LaserPointer", "laserpointer");

            CreateIconButton(stack, "TB_Variants", "variants");

            CreateIconButton(stack, "TB_SunSimulation", "sun_simulation");

            // Bottom persistent Tools button (open/close)
            var tools = CreateIconButton(root, "TB_Tools", "tools");
            tools.AddToClassList("tb-tools");

            await UniTask.Yield();
            return root;
        }

        private Button CreateIconButton(VisualElement parent, string name, string iconNameNoExt)
        {
            var btn = parent.CreateChild<Button>("tb-btn");
            btn.AddToClassList("i18n-skip"); 
            btn.name = name;
            btn.text = string.Empty;

            // icon child
            var icon = btn.CreateChild<VisualElement>("tb-icon");
            icon.name = "Icon";

            // default image set here; runtime swaps handled in view
            var sprite = Resources.Load<Sprite>($"Icons/ToolBar/{iconNameNoExt}");
            if (sprite != null) icon.style.backgroundImage = new StyleBackground(sprite);

            return btn;
        }
    }
}