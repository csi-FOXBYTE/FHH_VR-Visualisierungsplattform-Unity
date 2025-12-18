using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Foxbyte.Core.Services.AuthService
{
    public class BrowserHandler : IBrowserHandler
    {
        private UniTaskCompletionSource<string> _authorizationCodeTaskCompletionSource;
        private bool _deepLinkSubscribed;
        private string _expectedRedirectScheme;

#if UNITY_EDITOR
        private AuthorizationWindow _authWindow;
#endif

        public async UniTask<string> LaunchBrowserAsync(string url, string redirectUri)
        {
            try
            {
                _authorizationCodeTaskCompletionSource = new UniTaskCompletionSource<string>();
                
#if UNITY_IOS && !UNITY_EDITOR
            string result = await iOSBrowserWrapper.LaunchBrowser(url, redirectUri);
            HandleRedirect(result);
#elif UNITY_ANDROID && !UNITY_EDITOR
            _expectedRedirectScheme = GetScheme(redirectUri);
            SubscribeDeepLinkIfNeeded(_expectedRedirectScheme);
            //Application.OpenURL(url);
#elif UNITY_EDITOR
                Application.OpenURL(url);
                //Debug.Log("Please enter the authorization code in the UI:");
                //ShowEditorUI(true); // if no local server is used, show UI to enter code manually
#else
            _expectedRedirectScheme = GetScheme(redirectUri);
            SubscribeDeepLinkIfNeeded(_expectedRedirectScheme);
            Application.OpenURL(url);
#endif
                //return await _authorizationCodeTaskCompletionSource.Task;
                return await _authorizationCodeTaskCompletionSource.Task.Timeout(TimeSpan.FromMinutes(5));
            }
            catch (System.Exception ex)
            {
                _authorizationCodeTaskCompletionSource.TrySetException(ex);
                throw;
            }
            finally
            {
#if UNITY_EDITOR
                //ShowEditorUI(false);
#endif
                UnsubscribeDeepLink();
            }
        }

        private void OnProtocolUrl(string url)
        {
            // Accept only if scheme matches (custom scheme path)
            if (string.IsNullOrEmpty(_expectedRedirectScheme)) return;
            try
            {
                var u = new Uri(url);
                if (!u.Scheme.Equals(_expectedRedirectScheme, StringComparison.OrdinalIgnoreCase)) return;
                HandleRedirect(url);
            }
            catch { /* ignore */ }
        }

        public void HandleRedirect(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                _authorizationCodeTaskCompletionSource.TrySetException(
                    new Exception("Authentication was cancelled"));
                return;
            }
            //Uri uri = new Uri(url);
            //string code = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("code");
            //if (!string.IsNullOrEmpty(code))
            //{
            //    _authorizationCodeTaskCompletionSource.TrySetResult(code);
            //}
            //else
            //{
            //    _authorizationCodeTaskCompletionSource.TrySetException(
            //        new Exception("Failed to get authorization code"));
            //}
            var uri = new Uri(url);
            var query = uri.Query.TrimStart('?');
            string code = null;
            // Parse manually to avoid '+' => ' ' conversion.
            // Use WebUtility.UrlDecode which does NOT treat '+' as space.
            foreach (var part in query.Split('&'))
            {
                var kv = part.Split(new[] { '=' }, 2);
                if (kv.Length == 2 && kv[0] == "code")
                {
                    code = System.Net.WebUtility.UrlDecode(kv[1]); // preserves '+'
                    break;
                }
            }

            if (!string.IsNullOrEmpty(code))
            {
                code = code.Trim();
                _authorizationCodeTaskCompletionSource.TrySetResult(code);
                UnsubscribeDeepLink();
            }
            else
            {
                _authorizationCodeTaskCompletionSource.TrySetException(new Exception("Failed to get authorization code"));
                UnsubscribeDeepLink();
            }
        }

#if UNITY_EDITOR
        private void ShowEditorUI(bool show)
        {
            if (show)
            {
                _authWindow = EditorWindow.GetWindow<AuthorizationWindow>("Authorization");
                _authWindow.OnSubmit += OnSubmitButtonClicked;
                _authWindow.Show();
            }
            else if (_authWindow != null)
            {
                _authWindow.OnSubmit -= OnSubmitButtonClicked;
                _authWindow.Close();
                _authWindow = null;
            }
        }

        private void OnSubmitButtonClicked(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Debug.LogWarning("Please enter a valid authorization code or redirect URL.");
                return;
            }

            string code = input;

            // if pasted a full URL, extract ?code=...
            if (input.Contains("://"))
            {
                try
                {
                    var uri = new Uri(input);
                    var q = uri.Query.TrimStart('?');
                    foreach (var part in q.Split('&'))
                    {
                        var kv = part.Split(new[] { '=' }, 2);
                        if (kv.Length == 2 && kv[0] == "code")
                        {
                            code = System.Net.WebUtility.UrlDecode(kv[1]).Trim();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to parse URL: {ex.Message}");
                }
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                Debug.Log("Received authorization code via editor UI.");
                _authorizationCodeTaskCompletionSource.TrySetResult(code);
                ShowEditorUI(false);
            }
            else
            {
                Debug.LogWarning("Could not extract authorization code.");
            }
        }


        private class AuthorizationWindow : EditorWindow
        {
            public event Action<string> OnSubmit;

            private TextField _inputField;
            private Button _submitButton;

            private void CreateGUI()
            {
                rootVisualElement.style.flexGrow = 1;
                rootVisualElement.style.flexDirection = FlexDirection.Column;
                rootVisualElement.style.alignItems = Align.Center;
                rootVisualElement.style.justifyContent = Justify.Center;

                _inputField = new TextField("Authorization Code or Redirect URL");
                _inputField.style.width = 300;
                rootVisualElement.Add(_inputField);

                _submitButton = new Button(() => OnSubmit?.Invoke(_inputField.value)) { text = "Submit" };
                _submitButton.style.width = 100;
                _submitButton.style.marginTop = 10;
                rootVisualElement.Add(_submitButton);

                this.minSize = new Vector2(350, 100);
                this.maxSize = new Vector2(350, 100);
            }
        }
#endif

        private static string GetScheme(string redirectUri)
        {
            try
            {
                var u = new Uri(redirectUri);
                return u.Scheme; // e.g., "de.foxbyte.abc.vrvis"
            }
            catch { return null; }
        }

        private void SubscribeDeepLinkIfNeeded(string scheme)
        {
            if (_deepLinkSubscribed) return;
            if (string.IsNullOrEmpty(scheme)) return;
            // For loopback (http/https) we won't get deepLinkActivated
            if (scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) return;

            Application.deepLinkActivated += OnDeepLinkActivated;
            _deepLinkSubscribed = true;
        }

        private void UnsubscribeDeepLink()
        {
            if (_deepLinkSubscribed)
            {
                Application.deepLinkActivated -= OnDeepLinkActivated;
                _deepLinkSubscribed = false;
            }
        }

        private void OnDeepLinkActivated(string url)
        {
            HandleRedirect(url);
        }
    }
}