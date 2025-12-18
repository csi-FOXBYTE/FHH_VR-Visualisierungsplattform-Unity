using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using UnityEngine;

namespace Foxbyte.Core.Services.AuthService
{
    public abstract class BaseTokenStorageProvider : ITokenStorageProvider
    {
        protected readonly string ServiceName = "de.foxbyte." + Application.productName;
        protected const string AccessTokenKey = "AccessToken";
        protected const string RefreshTokenKey = "RefreshToken";
        protected const string IdTokenKey = "IdToken";
        protected const string ExpirationTimeKey = "ExpirationTime";

        private string _accessToken = String.Empty;
        private string _expirationTime = String.Empty;

        public async UniTask SaveTokensAsync(JObject tokenResponse)
        {
            try
            {
                string accessToken = tokenResponse["access_token"].ToString();
                _accessToken = accessToken;
                string refreshToken = tokenResponse["refresh_token"]?.ToString();
                int expiresIn = tokenResponse["expires_in"].Value<int>();
                string idToken = tokenResponse["id_token"]?.ToString();


                if (string.IsNullOrEmpty(accessToken))
                {
                    Debug.LogWarning("Access token is missing");
                    return;
                }

                if (expiresIn <= 0)
                {
                    Debug.LogWarning("Invalid expiration time");
                    expiresIn = 3600;
                }

                if (string.IsNullOrEmpty(refreshToken))
                {
                    Debug.LogWarning("Refresh token is missing");
                }

                if (string.IsNullOrEmpty(idToken))
                {
                    //Debug.LogWarning("ID token is missing");
                }

                Debug.Log($"Access token: {accessToken}");
                Debug.Log($"Refresh token: {refreshToken}");
                //Debug.Log($"ID token: {idToken}");
                Debug.Log($"Expires in: {expiresIn}");

                DateTime expirationTime = DateTime.UtcNow.AddSeconds(expiresIn);
                _expirationTime = expirationTime.ToString("O");

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    //await SaveValueAsync($"{ServiceName}.{AccessTokenKey}", accessToken);
                    //await SaveValueAsync($"{ServiceName}.{ExpirationTimeKey}", expirationTime.ToString("O"));
                }
                
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await SaveValueAsync($"{ServiceName}.{RefreshTokenKey}", refreshToken);
                }

                if (!string.IsNullOrEmpty(idToken))
                {
                    await SaveValueAsync($"{ServiceName}.{IdTokenKey}", idToken);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save tokens: {e.Message}");
            }
        }
        
        public async UniTask<string> GetAccessTokenAsync()
        {
            //return await GetValueAsync($"{ServiceName}.{AccessTokenKey}");
            await UniTask.FromResult(String.Empty);
            throw new NotImplementedException("Asynchronous access token retrieval is not implemented. Use GetAccessTokenAsync() instead.");
        }
        public string GetAccessToken()
        {
            return _accessToken;
        }

        public async UniTask<string> GetRefreshTokenAsync()
        {
            return await GetValueAsync($"{ServiceName}.{RefreshTokenKey}");
        }

        public async UniTask<string> GetIdTokenAsync()
        {
            return await GetValueAsync($"{ServiceName}.{IdTokenKey}");
        }

        public async UniTask<bool> IsTokenExpiredAsync()
        {
            //string expirationTimeString = await GetValueAsync($"{ServiceName}.{ExpirationTimeKey}");
            string expirationTimeString = _expirationTime;
            if (string.IsNullOrEmpty(expirationTimeString))
            {
                Debug.Log("No Token found.");
                return true;
            }
            if (!DateTime.TryParseExact(expirationTimeString, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime expirationTime))
            {
                Debug.Log($"Failed to parse expiration time: {expirationTimeString}");
                return true;
            }

            //Debug.Log($"Expiration time string: {expirationTimeString}");
            //Debug.Log($"Expiration time: {expirationTime:O}");
            //Debug.Log($"Current UTC time: {DateTime.UtcNow:O}");

            if (DateTime.UtcNow >= expirationTime)
            {
                Debug.Log("Token is expired.");
                return true;
            }
            else
            {
                Debug.Log($"Token is not expired. Time remaining: {expirationTime - DateTime.UtcNow}");
            }
            await UniTask.Yield();
            return false;
        }

        public async UniTask ClearTokensAsync()
        {
            await DeleteValueAsync($"{ServiceName}.{RefreshTokenKey}");
            await DeleteValueAsync($"{ServiceName}.{IdTokenKey}");
            //await DeleteValueAsync($"{ServiceName}.{AccessTokenKey}");
            //await DeleteValueAsync($"{ServiceName}.{ExpirationTimeKey}");
        }

        protected abstract UniTask SaveValueAsync(string key, string value);
        protected abstract UniTask<string> GetValueAsync(string key);
        protected abstract UniTask DeleteValueAsync(string key);
    }
}