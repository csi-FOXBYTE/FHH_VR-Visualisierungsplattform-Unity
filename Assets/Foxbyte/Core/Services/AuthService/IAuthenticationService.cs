using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Foxbyte.Core.Services.AuthService
{
    public interface IAuthenticationService
    {
        UniTask<DataResult> LoginAsync();
        UniTask<DataResult> AuthenticateAsync();
        UniTask<DataResult> RefreshTokenAsync(CancellationToken ct = default);

        UniTask<DataResult> EndSessionAsync();
        UniTask<DataResult> RevokeTokenAsync();
        UniTask<DataResult> LogoutAsync();
        UniTask<Dictionary<string, string>> GetAuthHeadersForApiAsync();
        ITokenStorageProvider GetTokenStorageProvider();
    }
}