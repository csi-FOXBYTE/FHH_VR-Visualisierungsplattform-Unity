using Cysharp.Threading.Tasks;
using FHH.Logic;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Core.Services.UserState;
using Foxbyte.Presentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FHH.UI.User
{
    public sealed class UserModel : PresenterModelBase
    {
        private IAuthenticationService _auth;
        private IHttpClient _http;
        private AppConfig _appConfig;

        public IUser CurrentUser { get; private set; } = Logic.Models.User.Anonymous();
        
        public bool IsLoggedIn =>
            CurrentUser != null &&
            !string.IsNullOrWhiteSpace(CurrentUser.DisplayName) &&
            CurrentUser.DisplayName != "Anonymous";

        public string Initials =>
            IsLoggedIn
                ? string.Concat(CurrentUser.DisplayName
                    .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => char.ToUpperInvariant(p[0])))
                : string.Empty;


        public UserModel(object data = null) { }

        public override async UniTask InitializeAsync()
        {
            _auth = ServiceLocator.GetService<OAuth2AuthenticationService>();
            _http = new HttpClientWithRetry();
            _appConfig = ServiceLocator.GetService<ConfigurationService>().AppSettings;
            await UniTask.CompletedTask;
        }

        public async UniTask<bool> LoginAsync()
        {
            if (_auth == null) { ULog.Error("Auth service missing."); return false; }

            var loginResult = await _auth.LoginAsync();
            if (!loginResult.Success)
            {
                ULog.Error("Login error occurred: " + loginResult.Error);
                return false;
            }
            
            CurrentUser = Logic.Models.User.TestUser(); //overwritten by real user fetch below

            try
            {
                var baseUrl = _appConfig.ServerConfig.ApiBaseUrl;
                var tokenProvider = _auth.GetTokenStorageProvider();
                var accessToken = tokenProvider.GetAccessToken();

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    ULog.Warning("No access token found.");
                    return false;
                }

                var headers = new Dictionary<string, string>
                {
                    ["Authorization"] = $"Bearer {accessToken}",
                    ["Accept"] = "application/json"
                };

                var url = $"{baseUrl?.TrimEnd('/')}/user/info";
                var httpRes = await _http.GetAsync(url, headers);
                if (!httpRes.IsSuccess)
                {
                    ULog.Warning($"Failed to fetch user info. Status: {httpRes.StatusCode}. Error: {httpRes.ErrorMessage}");
                    return false;
                }

                var backendUser = JsonConvert.DeserializeObject<BackendUserInfo>(httpRes.Data);

                if (backendUser == null)
                {
                    ULog.Warning("User info response was empty or invalid.");
                    return false;
                }
                
                var dto = new Logic.Models.UserInfoDto
                {
                    //Id = null,
                    Name = backendUser.Name,
                    Email = backendUser.Email,
                    Image = backendUser.Image,
                    //Roles = new HashSet<string>(){"guest"};
                    //Permissions = Array.Empty<string>()
                };

                // will be guest by default
                var user = new FHH.Logic.Models.User(dto);

                // Save into services
                //var userStateService = ServiceLocator.GetService<IUserStateService>();
                var permissionService = ServiceLocator.GetService<PermissionService>();

                if (permissionService != null)
                {
                    permissionService.SetUser(user);
                }

                //if (userStateService != null)
                //{
                //    userStateService.SetCurrentUser(user);
                //    CurrentUser = userStateService.CurrentUser;
                //}
                //else
                //{
                CurrentUser = user; // fallback
                //}
                // log user details
                ULog.Info("Display name: " + CurrentUser.DisplayName);
                ULog.Info("Email: " + CurrentUser.Email);
                //ULog.Info("Id: " + CurrentUser.Id);
                //ULog.Info("Image: " + CurrentUser.Image);
                
                ULog.Info("Login successful.");
                return true;
            }
            catch (Exception ex)
            {
                ULog.Error("Login succeeded but user fetch failed: " + ex.Message);
                CurrentUser = Logic.Models.User.TestUser();
                return true;
            }
        }

        public async UniTask<bool> LogoutAsync()
        {
            if (_auth == null) { ULog.Error("Auth service missing."); return false; }

            var result = await _auth.LogoutAsync();
            if (!result.Success) { ULog.Error("Logout failed: " + result.Error); return false; }

            var permissionService = ServiceLocator.GetService<PermissionService>();
            permissionService.SetUser(Logic.Models.User.Anonymous());
            CurrentUser = permissionService.GetUser();
            ULog.Info("Logout successful.");
            return true;
        }

        private class BackendUserInfo
        {
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("email")] public string Email { get; set; }
            [JsonProperty("image")] public string Image { get; set; }
        }
    }
}