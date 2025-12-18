using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using static Foxbyte.Core.Services.AuthService.HttpClientWithRetry;

namespace Foxbyte.Core.Services.AuthService
{
    public interface IHttpClient
    {
        UniTask<HttpClientResult> GetAsync(string url, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
        UniTask<HttpClientResult> PostAsync(string url, Dictionary<string, string> parameters = null, string jsonData = null, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
        UniTask<IDisposable> SubscribeAsync(
            string url,
            Dictionary<string, string> headers = null,
            Action<string> onData = null,
            CancellationToken cancellationToken = default
        );
        UniTask<HttpClientBinaryResult> GetBytesAsync(
            string url,
            Dictionary<string, string> headers = null,
            CancellationToken cancellationToken = default
        );
    }
}