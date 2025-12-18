using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.AuthService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Foxbyte.Core.Services.DataTransfer
{
    public sealed class ServerEventService : IAppServiceAsync,IDisposable
    {
        private readonly IHttpClient _http;
        private readonly IAuthenticationService _auth;
        private readonly ITokenStorageProvider _tokens;
        private readonly AppConfig _cfg;
        private readonly Dictionary<string,Delegate> _parsers = new();
        private readonly List<(CancellationTokenSource Cts, IDisposable Sub)> _subs = new();
        
        private string BuildUrlWithVersion(string path) =>
            $"{_cfg.ServerConfig.ApiBaseUrl.TrimEnd('/')}/" +
            $"{_cfg.ServerConfig.ApiVersion.Trim('/')}/{path.TrimStart('/')}";

        private string BuildUrl(string path) =>
            $"{_cfg.ServerConfig.ApiBaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";


        private Dictionary<string, string> BuildHeaders()
        {
            var dict = _cfg.ServerConfig.Headers?.ToDictionary(h => h.Key, h => h.Value)
                       ?? new Dictionary<string, string>();

            // Prefer synchronous accessor; do NOT block on async here
            var token = _tokens.GetAccessToken();
            if (!string.IsNullOrWhiteSpace(token))
                dict["Authorization"] = $"Bearer {token}";
            return dict;
        }

        /// <summary>
        /// ServerEventService constructor.
        /// </summary>
        /// <param name="http"></param>
        /// <param name="auth"></param>
        /// <param name="cfg"></param>
        /// <param name="tokens"></param>
        public ServerEventService(IHttpClient http, IAuthenticationService auth, AppConfig cfg, ITokenStorageProvider tokens)
        {
            _http = http; 
            _auth = auth; 
            _cfg = cfg;
            _tokens = tokens;
        }

        public async UniTask InitServiceAsync()
        {
            //ULog.Info("[ServerEventService] initialized.");
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Register a stream by providing an endpoint name and a how to parse the incoming data.
        /// Example: RegisterStream<User>("GetUser", json => JsonConvert.DeserializeObject<User>(json));
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="name"></param>
        /// <param name="parse"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterStream<TDomain>(string name, Func<string,TDomain> parse) =>
            _parsers[name] = parse ?? throw new ArgumentNullException(nameof(parse));
        
        
        /// <summary>
        /// Subscribe to a server event stream by name and provide a callback to handle the data.
        /// Example: SubscribeAsync<User>("GetUser", user => Debug.Log($"Received user: {user.Name}"));
        /// Add a cancellation token to stop the subscription when needed.
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="name"></param>
        /// <param name="onData"></param>
        /// <param name="externalCt"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async UniTask<IDisposable> SubscribeAsync<TDomain>(
            string name, Action<TDomain> onData, CancellationToken externalCt = default)
        {
            if (!_parsers.TryGetValue(name, out var del))
                throw new InvalidOperationException($"Stream '{name}' not registered.");
            var parse = (Func<string,TDomain>)del;

            var ep = _cfg.ServerConfig.SseEndpoints
                         .First(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            var cts = externalCt.CanBeCanceled
                ? CancellationTokenSource.CreateLinkedTokenSource(externalCt)
                : new CancellationTokenSource();

            async UniTask<IDisposable> Connect() =>
                await _http.SubscribeAsync(
                    BuildUrl(ep.Path),
                    BuildHeaders(),
                    json => onData?.Invoke(parse(json)),
                    cts.Token);
            
            var sub = await RetryLoopAsync(Connect, cts.Token);
            _subs.Add((cts, sub));

            return new SimpleDisposable(() =>
            {
                cts.Cancel();  // stop this stream only
                sub.Dispose(); // close the HTTP/SSE connection
                cts.Dispose();
                _subs.Remove((cts, sub));
            });
        }

        private async UniTask<IDisposable> RetryLoopAsync(Func<UniTask<IDisposable>> connect, CancellationToken ct)
        {
            TimeSpan backoff = TimeSpan.FromSeconds(2);
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    return await connect();
                }
                catch (HttpClientWithRetry.AuthRequiredException)
                {
                    // Only here do we refresh tokens
                    ULog.Warning("[ServerEventService] SSE auth required → refreshing token…");
                    await _auth.RefreshTokenAsync();
                    // small pause to allow storage to update
                    await UniTask.Delay(TimeSpan.FromMilliseconds(200), cancellationToken: ct);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    // Network or server transient issue
                    ULog.Warning($"[ServerEventService] SSE connect failed: {ex.Message}. Retrying in {backoff.TotalSeconds:0}s…");
                    await UniTask.Delay(backoff, cancellationToken: ct);
                    backoff = TimeSpan.FromSeconds(Math.Min(backoff.TotalSeconds * 2, 30));
                }
            }
            throw new OperationCanceledException();
        }

        public void Dispose()
        {
            foreach (var (cts, sub) in _subs)
            {
                cts.Cancel();
                sub.Dispose();
                cts.Dispose();
            }
            _subs.Clear();
        }
        
        public async UniTask DisposeServiceAsync()
        {
            Dispose();
            await UniTask.CompletedTask;
        }

        private sealed class SimpleDisposable : IDisposable
        {
            private readonly Action _dispose;
            private int _disposed; // 0 = live, 1 = disposed

            public SimpleDisposable(Action dispose) =>
                _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 0)
                    _dispose();
            }
        }

        /// <summary>
        /// Subscribe by URL overload (for dynamic paths with IDs).
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="url"></param>
        /// <param name="parse"></param>
        /// <param name="onData"></param>
        /// <param name="externalCt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async UniTask<IDisposable> SubscribeUrlAsync<TDomain>(
            string url,
            Func<string, TDomain> parse,
            Action<TDomain> onData,
            CancellationToken externalCt = default)
        {
            if (parse == null) throw new ArgumentNullException(nameof(parse));
            if (onData == null) throw new ArgumentNullException(nameof(onData));

            var cts = externalCt.CanBeCanceled
                ? CancellationTokenSource.CreateLinkedTokenSource(externalCt)
                : new CancellationTokenSource();

            async UniTask<IDisposable> Connect() =>
                await _http.SubscribeAsync(
                    url,
                    BuildHeaders(),
                    json => onData?.Invoke(parse(json)),
                    cts.Token);

            var sub = await RetryLoopAsync(Connect, cts.Token);
            _subs.Add((cts, sub));

            return new SimpleDisposable(() =>
            {
                cts.Cancel();
                sub.Dispose();
                cts.Dispose();
                _subs.Remove((cts, sub));
            });
        }
    }
}
