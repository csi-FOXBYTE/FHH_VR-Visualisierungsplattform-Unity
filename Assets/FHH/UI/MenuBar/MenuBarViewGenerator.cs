using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI
{
    public sealed class MenuBarViewGenerator : ViewGeneratorBase<MenuBarPresenter, MenuBarView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            await UniTask.Yield();

            var parent = View.ContainerOfThisView; 

            var menuBar = new VisualElement { name = "MenuBar" };
            menuBar.AddToClassList("menu-bar"); 
            menuBar.AddToClassList("app-background0"); 

            var left  = new VisualElement { name = "MenuBar_Left"  };
            left.style.flexDirection = FlexDirection.Row;
            left.style.justifyContent = Justify.FlexStart;
            left.style.width = Length.Percent(100);
            left.style.marginLeft = 10;

            var right = new VisualElement { name = "MenuBar_Right" };
            right.style.flexDirection = FlexDirection.Row;
            right.style.alignContent = Align.FlexEnd;
            right.style.justifyContent = Justify.FlexEnd;
            right.style.width = Length.Percent(100);
            right.style.marginRight = 10;

            // Logo
            var logo = new VisualElement();
            var logoImage = new Image();
            logoImage.image = Resources.Load<Texture2D>("Images/HH_Sonder-Logo_RGB_positiv");
            logoImage.scaleMode = ScaleMode.ScaleToFit;
            logoImage.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Left);
            logoImage.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            logo.style.backgroundImage = new StyleBackground((Texture2D)logoImage.image);
            logo.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Left);
            logo.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            logo.style.flexGrow = 1;
            logo.AddToClassList("menu-bar__logo"); // for actual styling

            // Hamburger Menu
            var hamburgerBtn = new Button { name = "MenuBar_HamburgerButton" };
            hamburgerBtn.AddToClassList("btn");
            hamburgerBtn.AddToClassList("btn-primary");
            hamburgerBtn.AddToClassList("menu-bar__button");
            hamburgerBtn.AddToClassList("i18n-skip");
            var hamburgerIcon = new Image();
            hamburgerIcon.AddToClassList("menu-bar-button__icon");
            hamburgerIcon.image = Resources.Load<Texture2D>("Icons/menu");
            hamburgerIcon.scaleMode = ScaleMode.ScaleToFit;
            hamburgerBtn.Add(hamburgerIcon);

            // Help
            var helpBtn = new Button { name = "MenuBar_HelpButton" };
            helpBtn.AddToClassList("btn");
            helpBtn.AddToClassList("btn-primary");
            helpBtn.AddToClassList("menu-bar__button");
            helpBtn.AddToClassList("i18n-skip");
            var helpIcon = new Image();
            helpIcon.AddToClassList("menu-bar-button__icon");
            helpIcon.image = Resources.Load<Texture2D>("Icons/question_mark");
            helpIcon.scaleMode = ScaleMode.ScaleToFit;
            helpBtn.Add(helpIcon);


            // VR Switch
            var vrSwitchBtn = new Button { name = "MenuBar_VRSwitchButton" };
            vrSwitchBtn.AddToClassList("btn");
            vrSwitchBtn.AddToClassList("btn-primary");
            vrSwitchBtn.AddToClassList("menu-bar__button");
            vrSwitchBtn.AddToClassList("i18n-skip");
            var vrSwitchIcon = new Image();
            vrSwitchIcon.AddToClassList("menu-bar-button__icon");
            vrSwitchIcon.image = Resources.Load<Texture2D>("Icons/VR_head_mounted_device");
            vrSwitchIcon.scaleMode = ScaleMode.ScaleToFit;
            vrSwitchBtn.Add(vrSwitchIcon);
            
            left.Add(logo);
            //right.Add(btn);
            //right.Add(viewModeGroup);
            right.Add(vrSwitchBtn);
            right.Add(hamburgerBtn);
            right.Add(helpBtn);
            menuBar.Add(left);
            menuBar.Add(right);

            View.MenuBarLeft = left;
            View.MenuBarRight = right;

            return menuBar;
        }
    }
}