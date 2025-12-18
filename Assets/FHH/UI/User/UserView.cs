using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.User
{
    public class UserView : ViewBase<UserPresenter>
    {
        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside { get; } = false;

        protected override bool IsModal { get; } = false;

        public Button UserButton { get; private set; }
        public Image  UserButtonIcon { get; private set; }

        // references
        //public Button MainBtn { get; internal set; }
        //public VisualElement Popup { get; internal set; }
        //public Label NameLabel { get; internal set; }
        //public Label MailLabel { get; internal set; }
        //public Button LoginLogoutBtn { get; internal set; }
        //public Image LoginLogoutIcon { get; internal set; }

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement target)
        {
            var gen = await ViewGeneratorBase<UserPresenter, UserView>.Create<UserViewGenerator>(this).GenerateViewAsync();
            UserButton = gen.Q<Button>("UserButton");
            UserButtonIcon = gen.Q<Image>("UserButtonIcon");
            return gen;
        }

        /// <summary>
        /// Update icon and keep text bound via localization/smart string.
        /// </summary>
       /* public void SetLoginLogoutVisual(bool isLoggedIn)
        {
            if (LoginLogoutIcon == null) return;
            var file = isLoggedIn ? "Icons/logout" : "Icons/login";
            LoginLogoutIcon.image = Resources.Load<Texture2D>(file);
        }*/

        public override Dictionary<string, string> GetSmartStringParameters(VisualElement element)
        {
            if (element.name == "UserButton")
            {
                //Debug.Log("GetSmartStringParameters for UserButton");
                return new Dictionary<string, string>
                {
                    ["loginState"] = Presenter.Model.IsLoggedIn ? "loggedIn" : "loggedOut" ,
                    ["initials"] = Presenter.Model.IsLoggedIn ? Presenter.Model.Initials : string.Empty
                };
            }
            return null;
        }
    }
}