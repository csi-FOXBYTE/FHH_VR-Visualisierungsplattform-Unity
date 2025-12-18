using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Foxbyte.Core.Services.AuthService
{
    public interface ITokenStorageProvider
    {
        UniTask SaveTokensAsync(JObject tokenResponse);
        UniTask<string> GetAccessTokenAsync();
        string GetAccessToken();
        UniTask<string> GetRefreshTokenAsync();
        UniTask<bool> IsTokenExpiredAsync();
        UniTask<string> GetIdTokenAsync();
        UniTask ClearTokensAsync();
    }
}