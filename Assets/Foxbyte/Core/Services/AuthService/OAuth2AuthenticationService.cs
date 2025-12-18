using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine.Networking;

namespace Foxbyte.Core.Services.AuthService
{
    public class OAuth2AuthenticationService : IAuthenticationService, IAppService
    {
        private readonly string _authorizationEndpoint;
        private readonly string _tokenEndpoint;
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string _redirectUriForBrowser;
        private readonly string _scopes;
        private readonly string _revocationEndpoint;
        private readonly string _refreshEndpoint;
        private readonly string _logoutEndpoint;

        private readonly ITokenStorageProvider _tokenStorageProvider;
        private readonly IHttpClient _httpClient;
        private readonly IBrowserHandler _browserHandler;
        
        private string _codeVerifier;
        private string _authorizationCode;
        private const bool Debug = false;

        private readonly System.Threading.SemaphoreSlim _refreshLock = new System.Threading.SemaphoreSlim(1, 1);
        private const int _expirySkewSeconds = 60;

        public OAuth2AuthenticationService(
            ITokenStorageProvider tokenStorageProvider,
            IHttpClient httpClient,
            IBrowserHandler browserHandler,
            AppConfig appConfig
            )
        {
            _tokenStorageProvider = tokenStorageProvider;
            _httpClient = httpClient;
            _browserHandler = browserHandler;

            _authorizationEndpoint = appConfig.ServerConfig.AuthEndpoint;
            _tokenEndpoint = appConfig.ServerConfig.TokenEndpoint;
            _clientId = appConfig.ServerConfig.ClientId;
            _redirectUri = appConfig.ServerConfig.RedirectUri;
            _redirectUriForBrowser = appConfig.ServerConfig.RedirectUriForBrowser;
            _scopes = string.Join(" ", appConfig.ServerConfig.Scopes);
            _revocationEndpoint = appConfig.ServerConfig.RevocationEndpoint;
            _refreshEndpoint = appConfig.ServerConfig.RefreshEndpoint;
            _logoutEndpoint  = appConfig.ServerConfig.LogoutEndpoint;
        }

        public void InitService()
        {
            //ULog.Info("[OAuth2AuthenticationService] initialized.");
        }

        public void DisposeService()
        {
            //ULog.Info("[OAuth2AuthenticationService] disposed.");
        }

        public ITokenStorageProvider GetTokenStorageProvider()
        => _tokenStorageProvider;

        /// <summary>
        /// Single entry point to ensure valid authentication.
        /// </summary>
        /// <returns></returns>
        public async UniTask<DataResult> LoginAsync()
        {
            try
            {
                if (!await IsAccessTokenExpiredAsync())
                    return new DataResult(true, "Success", "Access token is still valid.");


                var refreshed = await TryRefreshGuardedAsync();
                if (refreshed) return new DataResult(true, "Success", "Token has been refreshed.");

                // interactive login fallback (keeps platform-specific code)
                var auth = await AuthenticateAsync();
                return new DataResult(auth.Success, auth.Success ? "Authenticated successfully." : "Authentication failed.", auth.Description, auth.Error);
            }
            catch (Exception ex)
            {
                ULog.Error($"[OAuth2] LoginAsync failed: {ex.Message}");
                return new DataResult(false, "Error", $"LoginAsync failed: {ex.Message}");
            }
        }

        private async UniTask<bool> TryRefreshGuardedAsync()
        {
            await _refreshLock.WaitAsync();
            try
            {
                // recheck inside the lock to avoid double refresh
                if (!await IsAccessTokenExpiredAsync())
                    return true;

                var res = await RefreshTokenAsync();
                return res.Success;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        public async UniTask<DataResult> AuthenticateAsync()
        {
            try
            {
                _codeVerifier = GenerateCodeVerifier();
                string codeChallenge = GenerateCodeChallenge(_codeVerifier);

                var state = GenerateState();
                var cfgUri = new Uri(_redirectUri);
                bool isHttp = cfgUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                              cfgUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && !UNITY_IOS && !UNITY_ANDROID
                
                if (!isHttp)
                {
                    // custom scheme path (single-instance + IPC will deliver the URL)
                    string authorizationUrl =
                        $"{_authorizationEndpoint}?response_type=code&client_id={_clientId}" +
                        $"&redirect_uri={Uri.EscapeDataString(_redirectUri)}" +
                        $"&scope={Uri.EscapeDataString(_scopes)}" +
                        $"&code_challenge={codeChallenge}&code_challenge_method=S256&state={state}";

                    //ULog.Info($"[OAuth2] Opening browser with URL: {authorizationUrl}");
                    _authorizationCode = await _browserHandler.LaunchBrowserAsync(authorizationUrl, _redirectUri);
                    if (string.IsNullOrEmpty(_authorizationCode))
                        return new DataResult(false, "Error", "Failed to obtain authorization code");

                    var tokenResponse = await ExchangeCodeForTokensAsync(_authorizationCode, _redirectUri);
                    if (tokenResponse != null)
                    {
                        //await _tokenStorageProvider.SaveTokensAsync(tokenResponse);
                        return new DataResult(true, "Success", "Token saved.");
                    }
                    return new DataResult(false, "Error", "Failed to exchange authorization code");
                }
                else
                {
                    // Loopback path
                    var configuredUri = new Uri(_redirectUri);
                    int port = configuredUri.Port;
                    string path = configuredUri.AbsolutePath;

                    using (var loopback = new LoopbackRedirectListener())
                    {
                        loopback.Start(new[] { port }, path, _redirectUri);
                        string effectiveRedirect = loopback.RedirectUri;

                        string authorizationUrl =
                            $"{_authorizationEndpoint}?response_type=code&client_id={_clientId}" +
                            $"&redirect_uri={Uri.EscapeDataString(effectiveRedirect)}" +
                            $"&scope={Uri.EscapeDataString(_scopes)}" +
                            $"&code_challenge={codeChallenge}&code_challenge_method=S256&state={state}";

                        //ULog.Info($"[OAuth2] Opening browser with URL: {authorizationUrl}");
                        var launchTask = _browserHandler.LaunchBrowserAsync(authorizationUrl, effectiveRedirect);

                        var waitForCallback = loopback.WaitForCallbackAsync(TimeSpan.FromMinutes(5))
                            .ContinueWith(result =>
                            {
                                ULog.Info($"[OAuth2] Received callback URL: {result}");
                                _browserHandler.HandleRedirect(result);
                            });

                        await waitForCallback;
                        _authorizationCode = await launchTask;

                        if (string.IsNullOrEmpty(_authorizationCode))
                            return new DataResult(false, "Error", "Failed to obtain authorization code");

                        var tokenResponse = await ExchangeCodeForTokensAsync(_authorizationCode, effectiveRedirect);
                        if (tokenResponse != null)
                        {
                            //await _tokenStorageProvider.SaveTokensAsync(tokenResponse);
                            return new DataResult(true, "Success", "Token saved.");
                        }

                        return new DataResult(false, "Error", "Failed to exchange authorization code");
                    }
                }
#else

                string authorizationUrl =
                    $"{_authorizationEndpoint}?response_type=code&client_id={_clientId}&redirect_uri={_redirectUri}&scope={Uri.EscapeDataString(_scopes)}&code_challenge={codeChallenge}&code_challenge_method=S256&state={state}";

                ULog.Info(authorizationUrl);
                _authorizationCode = await _browserHandler.LaunchBrowserAsync(authorizationUrl, _redirectUriForBrowser);

                if (string.IsNullOrEmpty(_authorizationCode))
                {
                    ULog.Error("Failed to obtain authorization code");
                    return new DataResult(false, "Error", "Failed to obtain authorization code", null);
                }

                var tokenResponse = await ExchangeCodeForTokensAsync(_authorizationCode);

                if (tokenResponse != null)
                {
                    await _tokenStorageProvider.SaveTokensAsync(tokenResponse);
                    return new DataResult(true, "Success", "Token saved.");
                }

                return new DataResult(false, "Error", "Failed to obtain authorization code", null);
#endif
            }
            catch (Exception ex)
            {
                ULog.Error($"Authentication failed: {ex.Message}");
                return new DataResult(false, "Error", "Authentication failed.",ex.Message );

            }
        }

        /// <summary>
        /// Attempts to refresh the token. It first checks if the identity server is reachable via UnityWebRequest.
        /// If available, it performs a standard refresh request.
        /// Otherwise, it decodes the JWT refresh token to determine its expiration.
        /// Debug logs display the refresh token's expiration and internet availability.
        /// </summary>
        public async UniTask<DataResult> RefreshTokenAsync(CancellationToken ct = default)
        {
            try
            {
                string refreshToken = await _tokenStorageProvider.GetRefreshTokenAsync();
                if (string.IsNullOrEmpty(refreshToken))
                {
                    ULog.Error("Refresh token not found");
                    return new DataResult(false, "Error", "Refresh token not found.", null);
                }

                // Decode and log refresh token expiry.
                //DateTime? refreshExpiry = GetJwtExpiry(refreshToken);
                //if (refreshExpiry.HasValue)
                //{
                //    ULog.Info($"Current Refresh Token Expiry: {refreshExpiry.Value.ToString("u")}");
                //}
                //else
                //{
                //    ULog.Info("Could not determine refresh token expiry.");
                //}

                //if (refreshExpiry.HasValue && refreshExpiry.Value <= DateTime.UtcNow)
                //{
                //    ULog.Error("Refresh token expired.");
                //    return new DataResult(false, "Error", "Refresh token expired.");
                //}

                // Check internet availability quickly via UnityWebRequest.
                //bool internetAvailable = await IsInternetAvailableAsync();
                //ULog.Info($"Internet Available: {internetAvailable}");
                
                //if (internetAvailable)
                //{
                var parameters = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", _clientId },
                    { "refresh_token", refreshToken }
                    // ["redirect_uri"] = _redirectUri],
                };

                var headers = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json"
                };

                //var response = await _httpClient.PostAsync(_refreshEndpoint, parameters);
                var response = await _httpClient.PostAsync(_refreshEndpoint, parameters, jsonData: null, headers: headers, cancellationToken: ct );
                
                if (response == null)
                    return new DataResult(false, "Error", $"Refresh HTTP failed: {response?.ErrorMessage ?? "no response"}");

                if (!response.IsSuccess)
                {
                    // Treat invalid_grant as an expected re-login path
                    if (response.StatusCode == 400 &&
                        !string.IsNullOrEmpty(response.Data) &&
                        response.Data.IndexOf("\"invalid_grant\"", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Refresh token is invalid/expired/rotated
                        await _tokenStorageProvider.ClearTokensAsync();
                        return new DataResult(false, "NeedsLogin", "Session expired. Please log in again.");
                    }

                    // Other non-success (e.g., 401/403/5xx)
                    return new DataResult(false, "Error", $"Token refresh failed ({response.StatusCode}): {response.ErrorMessage ?? "Unknown error"}");
                }

                if (string.IsNullOrEmpty(response.Data))
                    return new DataResult(false, "Error", "Token endpoint returned empty body.");

                if (response.IsSuccess)
                {
                    var tokenResponse = JObject.Parse(response.Data);
                    if (tokenResponse["access_token"] != null)
                    {
                        await PersistTokensAsync(tokenResponse);
                        //await _tokenStorageProvider.SaveTokensAsync(tokenResponse);
                        ULog.Info("Token refreshed successfully.");
                        return new DataResult(true, "Success", "Token refreshed successfully.", null);
                    }
                    else
                    {
                        ULog.Error("Token refresh failed: access_token not found in response.");
                    }
                }
                else
                {
                    ULog.Error("Token refresh HTTP call failed: " + response.ErrorMessage);
                }
                //}
                //else
                //{
                //    ULog.Info("Internet not available, skipping token refresh request.");
                //}

                // Fallback: If the refresh token is still valid based on its JWT expiry, allow offline mode.
                //if (refreshExpiry.HasValue && refreshExpiry.Value > DateTime.UtcNow)
                //{
                //    ULog.Info($"Offline mode: refresh token valid until {refreshExpiry.Value.ToString("u")}. Continuing offline.");
                //    return new DataResult(true, "Success", "Offline mode: refresh token valid until " + refreshExpiry.Value.ToString("u") + ".");
                //}
                //else
                //{
                //    ULog.Error("Refresh token has expired offline. User must log in again.");
                //    return new DataResult(false, "Error", "Refresh token has expired offline. User must log in again.");
                //}
            }
            catch (OperationCanceledException)
            {
                ULog.Error("Token refresh canceled.");
                return new DataResult(false, "Error", "Token refresh canceled.");
            }
            catch (Exception ex)
            {
                ULog.Error($"Exception during RefreshTokenAsync: {ex.Message}");
                return new DataResult(false, "Error", "Error during token refresh.", ex.Message);
            }
            return new DataResult(false, "Error", "Token refresh failed for unknown reasons.");
        }

        /// <summary>
        /// Public Wrapper to get auth headers for API calls.
        /// Usage: var headers = await authService.GetAuthHeadersForApiAsync();
        /// var response = await _httpClient.GetAsync(url, headers);
        /// </summary>
        /// <returns></returns>
        public async UniTask<Dictionary<string,string>> GetAuthHeadersForApiAsync()
        {
            if (await IsAccessTokenExpiredAsync())
            {
                var ok = await TryRefreshGuardedAsync();
                if (!ok) return null;
            }
            return await GetAuthHeadersAsync();
        }

        private async UniTask<Dictionary<string, string>> GetAuthHeadersAsync()
        {
            var accessToken = _tokenStorageProvider.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
                return null;

            await UniTask.Yield();
            return new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {accessToken}" },
                { "Accept", "application/json" }
            };
        }

        private async UniTask<bool> IsAccessTokenExpiredAsync()
        {
            var accessToken = _tokenStorageProvider.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken)) return true;

            DateTime? exp = GetJwtExpiry(accessToken);
            if (!exp.HasValue) return true;
            await UniTask.Yield();
            return exp.Value <= DateTime.UtcNow.AddSeconds(_expirySkewSeconds);
        }

        private async UniTask PersistTokensAsync(JObject tokenResponse)
        {
            // Expect at least access_token; refresh_token may or may not be returned (rotation)
            var access = tokenResponse.Value<string>("access_token");
            if (string.IsNullOrEmpty(access))
                throw new Exception("[OAuth2] access_token missing in token response.");

            await _tokenStorageProvider.SaveTokensAsync(tokenResponse);
        }

        /// <summary>
        /// Checks the availability of the identity server using UnityWebRequest Get.
        /// This method quickly verifies connectivity with a short timeout.
        /// </summary>
        private async UniTask<bool> IsInternetAvailableAsync()
        {
            try
            {
                using (UnityWebRequest request = UnityWebRequest.Get("https://clients3.google.com/generate_204"))
                {
                    request.timeout = 3;
                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await UniTask.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        return true;
                    }
                    else
                    {
                        ULog.Error($"Internet availability check failed: {request.error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ULog.Error("Error checking internet availability: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Decodes a JWT token and returns its expiration (exp) claim as a UTC DateTime.
        /// </summary>
        private DateTime? GetJwtExpiry(string jwt)
        {
            try
            {
                // JWT tokens consist of three parts separated by '.'
                string[] parts = jwt.Split('.');
                if (parts.Length < 2)
                {
                    ULog.Error("Invalid JWT format.");
                    return null;
                }

                string payload = parts[1];
                // Convert from Base64 URL encoding to standard Base64.
                payload = payload.Replace('-', '+').Replace('_', '/');
                // Pad the string with '=' if needed.
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                byte[] jsonBytes = Convert.FromBase64String(payload);
                string jsonString = Encoding.UTF8.GetString(jsonBytes);
                var payloadData = JObject.Parse(jsonString);

                // The "exp" claim is a Unix timestamp.
                long exp = payloadData.Value<long>("exp");
                DateTime expiry = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                return expiry;
            }
            //catch (Exception ex)
            catch
            {
                //ULog.Warning("Failed to decode JWT expiry: " + ex.Message);
                return null;
            }
        }

        private async UniTask<JObject> ExchangeCodeForTokensAsync(string code)
        {
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _clientId },
                { "code", code },
                { "redirect_uri", _redirectUri },
                { "code_verifier", _codeVerifier }
            };

            var response = await _httpClient.PostAsync(_tokenEndpoint, parameters);
            if (response == null || !response.IsSuccess || string.IsNullOrEmpty(response.Data))
            {
                //throw new Exception($"[OAuth2] Token exchange failed: {response?.ErrorMessage ?? "no response"}");
                ULog.Error($"[OAuth2] Token exchange failed: {response?.ErrorMessage ?? "no response"}");
                return null;
            }

            var tokenResponse = JObject.Parse(response.Data);
            await PersistTokensAsync(tokenResponse);
            return tokenResponse;
        }

        private async UniTask<JObject> ExchangeCodeForTokensAsync(string code, string effectiveRedirect)
        {
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _clientId },
                { "code", code },
                { "redirect_uri", effectiveRedirect },
                { "code_verifier", _codeVerifier }
            };

            
            //ULog.Info("=== TOKEN EXCHANGE REQUEST ===");
            //ULog.Info($"Token Endpoint: {_tokenEndpoint}");
            //foreach (var param in parameters)
            //{
            //    ULog.Info($"  {param.Key}: {param.Value}");
            //}
            //ULog.Info("==============================");

            var response = await _httpClient.PostAsync(_tokenEndpoint, parameters);

            if (response == null || !response.IsSuccess || string.IsNullOrEmpty(response.Data))
            {
                ULog.Error($"[OAuth2] Token exchange failed: {response.ErrorMessage}");
                return null;
            }

            var tokenResponse = JObject.Parse(response.Data);
            await PersistTokensAsync(tokenResponse);
            return tokenResponse;
        }

        private string GenerateCodeVerifier()
        {
            byte[] randomBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }

        private string GenerateState(int byteLength = 12)
        {
            byte[] buffer = new byte[byteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer); 
            }
            string b64 = Convert.ToBase64String(buffer);
            return b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public async UniTask<DataResult> LogoutAsync()
        {
            try
            {
                //var accessToken = await _tokenStorageProvider.GetAccessTokenAsync();
                //if (string.IsNullOrEmpty(accessToken))
                //    return new DataResult(false, "Error", "No access token available for logout.");

                //var headers = new Dictionary<string, string>
                //{
                //    { "Authorization", $"Bearer {accessToken}" }
                //};
                var headers = await GetAuthHeadersAsync();
                if (headers == null || !headers.TryGetValue("Authorization", out _))
                {
                    // No token available—treat as already logged out locally
                    return new DataResult(true, "Success", "No token present; considered logged out.");
                }
                
                //var response = await _httpClient.PostAsync(_logoutEndpoint, headers);
                var response = await _httpClient.PostAsync(_logoutEndpoint, parameters: null, jsonData: null, headers: headers);

                if (response.IsSuccess)
                {
                    await _tokenStorageProvider.ClearTokensAsync();
                    return new DataResult(true, "Success", "Logged out successfully.");
                }

                return new DataResult(false, "Error", "Logout failed.", response.ErrorMessage);
            }
            catch (Exception ex)
            {
                return new DataResult(false, "Error", "Logout exception.", ex.Message);
            }
        }

        public async UniTask<DataResult> EndSessionAsync() 
        {
            DataResult dataResult;

            try
            {
                var idToken = await _tokenStorageProvider.GetIdTokenAsync();
                string url = _revocationEndpoint;
                var parameters = new Dictionary<string, string>
                {
                    { "id_token", idToken },
                    { "client_id", _clientId }
                };

                var response = await _httpClient.PostAsync(url, parameters);

                if (response.IsSuccess)
                {
                    dataResult = new DataResult(true, "Success", "Session ended successfully.");
                }
                else
                {
                    dataResult = new DataResult(false, "Error", "End session failed.", response.ErrorMessage);
                    
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"End session failed: {ex.Message}");
                dataResult = new DataResult(false, "Error", "End session exception.", ex.Message);
            }

            return dataResult;
        }

        public async UniTask<DataResult> RevokeTokenAsync() //string token)
        {
            try
            {
                var token = _tokenStorageProvider.GetAccessToken();
                string url = _revocationEndpoint;
                var parameters = new Dictionary<string, string>
                {
                    { "token", token },
                    { "client_id", _clientId }
                };

                var response = await _httpClient.PostAsync(url, parameters);

                // Check if the response indicates success
                if (response.IsSuccess)
                {
                    ULog.Info("Token revoked successfully.");
                    return new DataResult(true, "Success", "Token revoked successfully.");
                }
                else
                {
                    ULog.Error($"Failed to revoke token. Status Code: {response.IsSuccess}, Error: {response.ErrorMessage}");
                    return new DataResult(false, "Error", "Failed to revoke token.", response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"Token revocation failed: {ex.Message}");
                return new DataResult(false, "Error", "Token revocation failed.", ex.Message);
            }
        }
    }
}