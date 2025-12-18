using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Foxbyte.Presentation
{
    /// <summary>
    /// UIProjectContext is a central place to define UI regions and their identifiers.
    /// Will be used by UIManger.
    /// </summary>
    public class UIProjectContext
    {
        /// <summary>
        /// Possible UI regions in the project.
        /// </summary>
        public enum UIRegion
        {
            Header, Footer, Menu, Menubar1, Menubar2, Toolbar1, Toolbar2, User, Settings, 
            ContentParent, Content1, Content2, Content3, Content4,
            Sidebar, SidebarContent, Status, Notifications, Dashboard, Communication, 
            Modal, Toast, Overlay, Tooltip, Context, Popover, 
            Widget, Map, 
            Debug
        }

        private readonly Dictionary<UIRegion, VisualElement> _containers = new();
        private VisualElement _root;
        //private float _viewWidth;
        //private float _viewHeight;

        /// <summary>
        /// UIManager initializes the UIProjectContext by providing a UIDocument.
        /// It sets up the root VisualElement and adds predefined regions with their styles.
        /// This is project-specific.
        /// </summary>
        public async UniTask InitializeAsync(UIDocument uiDocument)
        {
            _root = uiDocument.rootVisualElement;

            _root.pickingMode = PickingMode.Ignore;

            //var distance = 4f;
            //float verticalSizeWorld = 2f * distance * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad);
            //float horizontalSizeWorld = verticalSizeWorld * Camera.main.aspect;
            //float widthPixels  = horizontalSizeWorld * 100f;
            //float heightPixels = verticalSizeWorld * 100f;
            //_viewWidth = widthPixels;
            //_viewHeight = heightPixels;

            _root.style.flexDirection = FlexDirection.Column;
            _root.style.flexGrow = 1;
            _root.style.flexShrink = 0;
            _root.style.minWidth = Length.Percent(10);
            _root.style.minHeight = Length.Percent(10);
            //_root.style.width = Length.Pixels(widthPixels);
            //_root.style.height = Length.Pixels(heightPixels);
            //_root.style.backgroundColor = Color.gray;


            AddHeader();
            //AddMenu();
            
            //AddMenubar1();
            //AddMenubar2();
            AddToolbar1();
            //AddToolbar2();
            //AddUser();
            //AddSettings();

            AddContentParent();
            AddContent1();
            //AddContent2();
            //AddContent3();
            //AddContent4();
            AddSidebar();
            AddSidebarContent();

            AddFooter();

            //AddStatus();
            AddNotifications();
            //AddDashboard();
            //AddCommunication();
            
            AddModal();
            AddToast();
            //AddOverlay();
            //AddTooltip();
            //AddContext();
            AddPopover();
            
            //AddWidget();
            //AddMap();
            //AddDebug();

            await UniTask.Yield();
        }

        public void AddRegion(UIRegion region, Action<VisualElement> customizer = null)
        {
            if (_containers.ContainsKey(region)) return;
            var ve = new VisualElement { name = region.Id() };
            ve.pickingMode = PickingMode.Ignore;
            customizer?.Invoke(ve);
            _root.Add(ve);
            _containers[region] = ve;
        }

        private void AddRegionToParent(UIRegion region, UIRegion parentRegion, Action<VisualElement> customizer = null)
        {
            if (_containers.ContainsKey(region)) return;
            if (!_containers.TryGetValue(parentRegion, out var parent))
                throw new Exception($"Parent region {parentRegion} not found for adding region {region}.");
            var ve = new VisualElement { name = region.Id() };
            customizer?.Invoke(ve);
            parent.Add(ve);
            _containers[region] = ve;
        }

        public bool TryGet(UIRegion region, out VisualElement container) => _containers.TryGetValue(region, out container);

        
        // UI regions, customized per project

        private void AddHeader() =>
            AddRegion(UIRegion.Header, ve =>
            {
                ve.style.flexDirection = FlexDirection.Row;
                ve.style.flexShrink = 0;
                ve.style.height = 50;
                ve.style.width = Length.Percent(100);
                ve.style.minWidth = Length.Percent(100);
                //ve.style.backgroundColor = Color.red;
                //var b = new Button();
                //b.style.width = 300;
                //b.text = "Click Me";
                //ve.Add(b);
            });

        private void AddFooter() =>
            AddRegion(UIRegion.Footer, ve =>
            {
                //ve.style.flexDirection = FlexDirection.Row;
                //ve.style.height = 50;
                //ve.style.width = Length.Percent(100);
                //ve.style.minWidth = Length.Percent(100);
                //ve.style.alignSelf = Align.FlexEnd;
            });

        private void AddMenu() =>
            AddRegion(UIRegion.Menu, ve =>
            {
                ve.style.width = 240;
                ve.style.maxWidth = 300;
                ve.style.minWidth = 180;
                ve.style.flexShrink = 0;
                ve.style.flexDirection = FlexDirection.Column;
                ve.style.display = DisplayStyle.None; // hidden by default, shown when toggled
            });

        private void AddMenubar1() =>
            AddRegion(UIRegion.Menubar1, ve =>
            {
                ve.style.height = 30;
                ve.style.width = Length.Percent(100);
                ve.style.flexDirection = FlexDirection.Row;
            });

        private void AddMenubar2() =>
            AddRegion(UIRegion.Menubar2, ve =>
            {
                ve.style.height = 30;
                ve.style.width = Length.Percent(100);
                ve.style.flexDirection = FlexDirection.Row;
            });

        private void AddToolbar1() =>
            AddRegion(UIRegion.Toolbar1, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.top = 10;
                ve.style.left = 10;
                ve.style.width = Length.Percent(100);
                ve.style.height = 50;
                ve.style.flexDirection = FlexDirection.Row;
                ve.pickingMode = PickingMode.Ignore;
            });

        private void AddToolbar2() =>
            AddRegion(UIRegion.Toolbar2, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.top = 100;
                ve.style.bottom = 60;
                ve.style.right = 20;
                ve.style.width = 60;
                ve.style.flexDirection = FlexDirection.Column;
            });

        private void AddUser() =>
            AddRegion(UIRegion.User, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.top = 100;           // aligned with toolbars
                ve.style.right = 20;
                ve.style.width = 260;
                ve.style.maxHeight = 400;     // scrollable if longer
                ve.style.flexDirection = FlexDirection.Column;
                ve.style.display = DisplayStyle.None;
                ve.style.paddingLeft = 10;
                ve.style.paddingRight = 10;
            });

        private void AddSettings() =>
            AddRegion(UIRegion.Settings, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.left = 40;
                ve.style.top = 100;
                ve.style.right = 40;
                ve.style.bottom = 80;
                ve.style.display = DisplayStyle.None;
                ve.style.alignItems = Align.Center;
                ve.style.justifyContent = Justify.Center;
            });

        
        // Content

        private void AddContentParent() =>
            AddRegion(UIRegion.ContentParent, ve =>
            {
                ve.pickingMode = PickingMode.Ignore;
            });
        private void AddContent1() => AddRegionToParent(UIRegion.Content1, UIRegion.ContentParent, ve =>
        {
            ve.pickingMode = PickingMode.Ignore;
        });
        private void AddContent2() => AddRegionToParent(UIRegion.Content2, UIRegion.ContentParent, ve => ve.style.flexGrow = 1);
        private void AddContent3() => AddRegionToParent(UIRegion.Content3, UIRegion.ContentParent, ve => ve.style.flexGrow = 1);
        private void AddContent4() => AddRegionToParent(UIRegion.Content4, UIRegion.ContentParent, ve => ve.style.flexGrow = 1);


        // Sidebar, Status, Dashboard, Communication, Notifications

        private void AddSidebar() =>
            //AddRegionToParent(UIRegion.Sidebar, UIRegion.ContentParent, ve =>
            AddRegion(UIRegion.Sidebar, ve =>
            {
                ve.pickingMode = PickingMode.Ignore;
            });
        
        private void AddSidebarContent() =>
            AddRegion(UIRegion.SidebarContent, ve =>
            {
                ve.pickingMode = PickingMode.Ignore;
            });

        private void AddStatus() =>
            AddRegion(UIRegion.Status, ve =>
            {
                //ve.style.height = 20;
                //ve.style.flexDirection = FlexDirection.Row;
            });

        private void AddDashboard() =>
            AddRegion(UIRegion.Dashboard, ve => ve.style.flexGrow = 1);

        private void AddCommunication() =>
            AddRegion(UIRegion.Communication, ve =>
            {
                
            });

        private void AddNotifications() =>
            AddRegion(UIRegion.Notifications, ve =>
            {
                //ve.style.position = Position.Absolute;
                //ve.style.top = 200;
                //ve.style.width = 300;
                //ve.style.flexDirection = FlexDirection.Column;
            });


        private void AddModal() =>
            AddRegion(UIRegion.Modal, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.left = 0;
                ve.style.top = 0;
                ve.style.right = 0;
                ve.style.bottom = 0;
                ve.style.display = DisplayStyle.None;
                ve.style.alignItems = Align.Center;
                ve.style.justifyContent = Justify.Center;
            });

        private void AddToast() =>
            AddRegion(UIRegion.Toast, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.bottom = 100;
                ve.style.left = Length.Percent(50);
                ve.style.translate = new Translate(Length.Percent(-50), Length.Percent(0), 0);
                ve.style.display = DisplayStyle.None;
                ve.pickingMode = PickingMode.Ignore;
                ve.BringToFront();
            });

        private void AddOverlay() =>
            AddRegion(UIRegion.Overlay, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.left = 0;
                ve.style.top = 0;
                ve.style.right = 0;
                ve.style.bottom = 0;
                ve.style.display = DisplayStyle.None;
            });

        private void AddTooltip() =>
            AddRegion(UIRegion.Tooltip, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.display = DisplayStyle.None;
            });

        private void AddContext() =>
            AddRegion(UIRegion.Context, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.display = DisplayStyle.None;
            });

        private void AddPopover() =>
            AddRegion(UIRegion.Popover, ve =>
            {
                ve.style.position = Position.Absolute;
                ve.style.display = DisplayStyle.None;
            });

        private void AddWidget() => AddRegion(UIRegion.Widget, ve => ve.style.flexGrow = 1);
        private void AddMap()    => AddRegion(UIRegion.Map,    ve => ve.style.flexGrow = 1);

        private void AddDebug() =>
            AddRegion(UIRegion.Debug, ve =>
            {
                ve.style.height = 200;
                ve.style.backgroundColor = new Color(0, 0, 0, 0.5f);
                ve.style.display = DisplayStyle.None;
            });

    }

    public static class UIRegionExtensions
    {
        // Converts enum to exact PascalCase string for UXML/USS names
        public static string Id(this UIProjectContext.UIRegion region) => region.ToString();
    }
}