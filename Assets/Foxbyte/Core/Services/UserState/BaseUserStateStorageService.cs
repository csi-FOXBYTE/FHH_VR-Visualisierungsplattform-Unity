using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Foxbyte.Core.Services.UserStateService
{
    //to be refactored
    public abstract class BaseUserStateStorageService : IUserStateStorageService
    {
        /*
        protected const string CurrentUserKey = "CurrentUser";
        protected const string LoggedInUserKey = "LoggedInUser";
        protected const string ServiceName = "de.foxbyte.projectxy";
        protected const string PermissionsKey = "Permissions";
        

        public async UniTask SaveCurrentUserAsync(CurrentUser currentUser)
        {
            try
            {
                string currentUserJson = JsonConvert.SerializeObject(currentUser);
                await SaveValueAsync($"{ServiceName}.{CurrentUserKey}", currentUserJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save current user: {e.Message}");
            }
        }

        public async UniTask<CurrentUser> GetCurrentUserAsync()
        {
            string currentUserJson = await GetValueAsync($"{ServiceName}.{CurrentUserKey}");
            if (string.IsNullOrEmpty(currentUserJson))
            {
                Debug.LogWarning("No current user found.");
                return null;
            }
            try
            {
                CurrentUser currentUser = JsonConvert.DeserializeObject<CurrentUser>(currentUserJson);
                return currentUser;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse current user: {e.Message}");
                return null;
            }
        }

        public async UniTask<User> GetLoggedInUserAsync()
        {
            string loggedInUserJson = await GetValueAsync($"{ServiceName}.{LoggedInUserKey}");
            if (string.IsNullOrEmpty(loggedInUserJson))
            {
                Debug.LogWarning("No logged in user found.");
                return null;
            }
            try
            {
                User loggedInUser = JsonConvert.DeserializeObject<User>(loggedInUserJson);
                return loggedInUser;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse logged in user: {e.Message}");
                return null;
            }
        }

        public async UniTask SaveLoggedInUserAsync(User user)
        {
            try
            {
                string userJson = JsonConvert.SerializeObject(user);
                await SaveValueAsync($"{ServiceName}.{LoggedInUserKey}", userJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save logged in user: {e.Message}");
            }
        }

        public async UniTask SaveUserPermissionsAsync(List<Permission> permissions)
        {
            try
            {
                string permissionsJson = JsonConvert.SerializeObject(permissions);
                await SaveValueAsync($"{ServiceName}.{PermissionsKey}", permissionsJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save permissions: {e.Message}");
            }
        }

        public async UniTask<List<Permission>> GetUserPermissionsAsync()
        {
            string permissionsJson = await GetValueAsync($"{ServiceName}.{PermissionsKey}");
            if (string.IsNullOrEmpty(permissionsJson))
            {
                Debug.LogWarning("No permissions found.");
                return null;
            }
            try
            {
                List<Permission> permissions = JsonConvert.DeserializeObject<List<Permission>>(permissionsJson);
                return permissions;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse permissions: {e.Message}");
                return null;
            }
        }

        public async UniTask ClearUserStateAsync()
        {
            await DeleteValueAsync($"{ServiceName}.{CurrentUserKey}");
            await DeleteValueAsync($"{ServiceName}.{LoggedInUserKey}");
            await DeleteValueAsync($"{ServiceName}.{PermissionsKey}");
        }

        protected abstract UniTask SaveValueAsync(string key, string value);
        protected abstract UniTask<string> GetValueAsync(string key);
        protected abstract UniTask DeleteValueAsync(string key);
    */
    }
}