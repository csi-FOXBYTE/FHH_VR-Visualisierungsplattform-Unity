using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using Foxbyte.Presentation.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.User
{
    public sealed class UserViewGenerator : ViewGeneratorBase<UserPresenter,UserView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            var root = new VisualElement { name = "UserRoot" }
                .AddClass("row", "user-root");

            // header button
            //var mainBtn = new Button { name = "UserMainBtn", text = "Login" };
            //mainBtn.AddToClassList("user-main");
            //mainBtn.AddToClassList("btn");
            //mainBtn.AddToClassList("btn-primary");
            //mainBtn.RegisterCallback<ClickEvent>(_ => Presenter.TogglePopup());
            //root.Add(mainBtn);
            //View.MainBtn = mainBtn;

            // Button
            var btn = new Button { name = "UserButton" }
                .AddClass("btn", "user-button");
                //.AddClass("btn", "user-button", "i18n-skip");
            btn.style.flexDirection = FlexDirection.Row;
            btn.style.alignItems = Align.Center;

            // Icon
            var icon = new Image { name = "UserButtonIcon" }
                .AddClass("user-button__icon", "i18n-skip");
            icon.scaleMode = ScaleMode.ScaleToFit;
            icon.style.width = 16;
            icon.style.height = 16;
            icon.style.marginRight = 6;

            btn.Add(icon);
            root.Add(btn);

            //View.UserButton = btn;
            //View.UserButtonIcon = icon;

            // Click handler
            //btn.RegisterCallback<ClickEvent>(async _ => await View.Presenter.OnButtonClickedAsync());
            btn.RegisterCallback<ClickEvent>(async _ => await View.Presenter.HandleLoginLogoutAsync());

            // First-time text & icon from model/locale
            //View.Presenter.UpdateButtonVisual();
            

            return await UniTask.FromResult(root);
        }
    }
}
