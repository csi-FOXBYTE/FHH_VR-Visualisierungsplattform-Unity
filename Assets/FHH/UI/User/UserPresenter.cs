using Cysharp.Threading.Tasks;
using FHH.Logic;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.User
{
    public sealed class UserPresenter : PresenterBase<UserPresenter,UserView,UserModel>
    {
        public UserPresenter(GameObject go, UIDocument doc, StyleSheet ss, VisualElement target,
            UserModel model = null, RequiredPermissions perms = null)
            : base(go, doc, ss, target, model, perms) { }

     
        public override async UniTask InitializeBeforeUiAsync()
        {
            if (Model == null)
            {
                Model = new UserModel();
                Model.AttachCancellationToken(Token); 
            }
            await UniTask.CompletedTask;
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await Model.InitializeAsync();
            await EnsureViewReadyAsync();   
            RefreshUI();
        }

        /// <summary>
        /// If the container was cleared (e.g., region was hidden), rebuild the view
        /// </summary>
        private async UniTask EnsureViewReadyAsync()
        {
            if (View?.ContainerOfThisView == null) return;

            var rootNow = View.ContainerOfThisView.Q("UserRoot");
            if (rootNow == null)
            {
                // Container was likely cleared — rebuild and rebind
                await View.InitAsync();
            }
        }

        /// <summary>
        /// Handles login or logout with guard, safe error handling and UI updates.
        /// </summary>
        public async UniTask HandleLoginLogoutAsync()
        {
            //if (View?.LoginLogoutBtn == null) return;
            if (View?.UserButton == null) return;
            await RunGuardedAsync(
                //View.LoginLogoutBtn,
                View.UserButton,
                async _ =>
                {
                    try
                    {
                        if (Model.IsLoggedIn)
                        {
                            var res = await Model.LogoutAsync();
                            //if (!res) Notify(GetCurrentLocaleCode() == Lang.de ? "Abmelden fehlgeschlagen" : "Logout failed");
                            if (!res) 
                                Notify("LoginFailed");
                            else
                                Notify("LogoutSuccessful");
                        }
                        else
                        {
                            var res = await Model.LoginAsync();
                            //if (!res) Notify(GetCurrentLocaleCode() == Lang.de ? "Anmelden fehlgeschlagen" : "Login failed");
                            if 
                                (!res) Notify("LoginFailed");
                            else
                                Notify("LoginSuccessful");
                        }
                    }
                    catch (Exception ex)
                    {
                        ULog.Error($"Login/Logout error: {ex.Message}");
                        //Notify(GetCurrentLocaleCode() == Lang.de ? "Vorgang fehlgeschlagen" : "Operation failed");
                        Notify("ActionFailed");
                    }
                    finally
                    {
                        RefreshUI();
                        //View.RefreshAllSmartStringValues();
                        //Notify(GetCurrentLocaleCode() == Lang.de ? "Login erfolgreich" : "Login successful");
                    }
                });
        }

        private void Notify(string msg)
        {
            var mgr = ServiceLocator.GetService<UIManager>();
            mgr?.ShowNotification(msg, 2.5f);
        }

        //private string LoginWordForCurrentLocale()
        //{
        //    return GetCurrentLocaleCode() == Lang.de ? "Anmelden" : "Login";
        //}

        //private Lang GetCurrentLocaleCode()
        //{
        //    try
        //    {
        //        var code = LocalizationSettings.SelectedLocale.Identifier.Code ?? "en";
        //        return code.StartsWith("de", StringComparison.OrdinalIgnoreCase) ? Lang.de : Lang.en;
        //    }
        //    catch { return Lang.en; }
        //}

        //private enum Lang
        //{
        //    en,
        //    de
        //}

     
        private void RefreshUI()
        {
            if (View?.UserButton == null) return;
            var isLoggedIn = Model.IsLoggedIn;
            View.RefreshAllSmartStringValues();
            var iconName = isLoggedIn ? "logout" : "login";
            var tex = Resources.Load<Texture2D>($"Icons/{iconName}");
            if (View.UserButtonIcon != null)
            {
                View.UserButtonIcon.image = tex;
                View.UserButtonIcon.style.display = tex != null ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}