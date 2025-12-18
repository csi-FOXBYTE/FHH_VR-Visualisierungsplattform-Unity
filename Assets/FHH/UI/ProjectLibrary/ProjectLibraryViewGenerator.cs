using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using Foxbyte.Presentation.Extensions;
using UnityEngine.UIElements;

namespace FHH.UI.ProjectLibrary
{
    public sealed class ProjectLibraryViewGenerator : ViewGeneratorBase<ProjectLibraryPresenter, ProjectLibraryView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            var root = new VisualElement { name = "ProjectLibraryRoot" };
            root.AddClass("pl-root"); 
            root.AddClass("col"); 

            // Tabs
            var tabs = root.CreateChild("row", "pl-tabs");
            var mine = new Button { name = "ProjectLibrary_TabMine", text = "Meine Projekte" };
            mine.AddClass("btn", "pl-tab");
            tabs.Add(mine);

            var shared = new Button { name = "ProjectLibrary_TabShared", text = "Mit mir geteilte" };
            shared.AddClass("btn", "pl-tab");
            tabs.Add(shared);

            var scroll = new ScrollView(ScrollViewMode.Vertical) { name = "ProjectLibrary_Scroll" };
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.AddToClassList("pl-scroll");
            root.Add(scroll);

            // Grid
            var grid = new VisualElement { name = "ProjectLibrary_Grid" };
            grid.AddClass("pl-grid");
            scroll.Add(grid);

            await UniTask.Yield();
            return root;
        }
    }
}