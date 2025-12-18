using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using Foxbyte.Presentation.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.MainMenu
{
    public sealed class MainMenuViewGenerator : ViewGeneratorBase<MainMenuPresenter, MainMenuView>
    {
        private const string IconsRoot = "Icons"; // Resources/Icons/

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            await UniTask.Yield();

            var root = new VisualElement { name = "MainMenuRoot" }
                .AddClass("mainmenu-root", "row", "app-background0");
            root.pickingMode = PickingMode.Ignore;

            // Sidebar (left)
            var sidebar = root.CreateChild("mainmenu-sidebar", "col", "app-background1");
            sidebar.name = "MainMenuSidebar";
            
            var sidebarItems = sidebar.CreateChild("mainmenu-sidebar-items", "col");
            var sidebarFooter = sidebar.CreateChild("mainmenu-sidebar-footer", "col");

            // Content (right)
            var content = root.CreateChild("mainmenu-content", "col");
            content.name = "MainMenuContent";

            // Build nav items
            var items = new List<(string key, string iconFile, System.Action onClick)>
            {
                ("Library",  "library",        () => Presenter.OnSelectProjectLibrary()),
                ("Meetings", "meetings",       () => Presenter.OnSelectMeetings()),
                ("Topics",   "theme_download", () => Presenter.OnSelectTopics()),
                ("Settings", "settings",       () => Presenter.OnSelectSettings()),
                ("Help",     "question_mark",  () => Presenter.OnSelectHelp()),
                //("Search",   "search_lupe",    () => Presenter.OnSelectSearch()),
            };

            foreach (var (key, icon, handler) in items)
            {
                var btn = CreateNavButton(sidebarItems, key, LoadIcon(icon));
                btn.RegisterCallback<ClickEvent>(_ => handler());
            }

            var quitButton = CreateNavButton(sidebarFooter, "Quit", LoadIcon("meeting_room_join"));
            quitButton.AddToClassList("menu-item-quit");
            quitButton.AddToClassList("i18n-skip");
            quitButton.RegisterCallback<ClickEvent>(_ => Presenter.OnQuitApplication());

            // Expose for presenter
            Presenter.AttachContainers(sidebar, content);

            return root;
        }

        private static Texture2D LoadIcon(string fileNameWithoutExt)
        {
            // Assets/FHH/AppResources/Icons/Resources/Icons/<name>.png  =>  Resources.Load<Texture2D>("Icons/<name>")
            var tex = Resources.Load<Texture2D>($"{IconsRoot}/{fileNameWithoutExt}");
            return tex;
        }

        private static Button CreateNavButton(VisualElement parent, string textKey, Texture2D icon)
        {
            var btn = parent.CreateChild<Button>("menu-item", "row", "center");
            btn.name = $"MainMenu_Button_{textKey}"; // for querying/state
            btn.focusable = true;

            var img = btn.CreateChild<Image>("menu-item-icon");
            //img.AddToClassList("menu-icon-icon");
            img.name = $"MainMenu_Icon_{textKey}";
            img.image = icon;
            img.scaleMode = ScaleMode.ScaleToFit;

            var label = btn.CreateChild<Label>("menu-item-label");
            label.name = $"MainMenu_Label_{textKey}";
            label.text = ""; //textKey;

            return btn;
        }
    }
}