using Cysharp.Threading.Tasks;

namespace Foxbyte.Core.Services.AuthService
{
    public interface IBrowserHandler
    {
        UniTask<string> LaunchBrowserAsync(string url, string redirectUri);
        void HandleRedirect(string url);
    }
}