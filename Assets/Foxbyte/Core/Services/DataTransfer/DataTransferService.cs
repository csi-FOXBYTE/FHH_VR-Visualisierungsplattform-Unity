using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.AuthService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Foxbyte.Core.Services.DataTransfer
{
    public class DataTransferService : IDataTransferService, IAppServiceAsync
    {
        private readonly IHttpClient _http;
        private readonly ITokenStorageProvider _tokens;
        private readonly AppConfig _cfg;

        private readonly Dictionary<string,EndpointMap> _registry = new();
        
        public DataTransferService(
            IHttpClient http,
            ITokenStorageProvider tokens,
            AppConfig cfg)
        {
            _http = http;
            _tokens = tokens;
            _cfg = cfg;
        }

        public async UniTask InitServiceAsync()
        {
            //ULog.Info("[DataTransferService] initialized.");
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Register an endpoint with a mapping function.
        /// Example: RegisterEndpoint<UserInfoDto, User>( "GetUserInfo", dto => new User(dto) );
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="name"></param>
        /// <param name="map"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterEndpoint<TDto,TDomain>(string name, Func<TDto,TDomain> map)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (map == null) throw new ArgumentNullException(nameof(map));

            _registry[name] = new EndpointMap { DtoType = typeof(TDto), Mapper = map };
        }

        public async UniTask<DataResult<TDomain>> FetchAsync<TDomain>(
            string name, object body = null, CancellationToken ct = default)
        {
            if (!_registry.TryGetValue(name, out var map))
                throw new InvalidOperationException($"Endpoint '{name}' not registered.");

            // using reflection to get the type of the DTO from the map
            MethodInfo mi = typeof(DataTransferService)
                .GetMethod(nameof(FetchInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(map.DtoType, typeof(TDomain));

            var result = await (UniTask<DataResult<TDomain>>)mi.Invoke(this, new object[] { name, body, map.Mapper, ct });
            return result;
        }

        /// <summary>
        /// Fetch data from a registered endpoint and map it to a domain object.
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="endpointName"></param>
        /// <param name="body"></param>
        /// <param name="map"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async UniTask<DataResult<TDomain>> FetchInternalAsync<TDto,TDomain>(
            string endpointName,
            object body,
            Func<TDto,TDomain> map,
            CancellationToken ct)
        {
            var ep = _cfg.ServerConfig.Endpoints.First(e =>
                string.Equals(e.Name, endpointName, StringComparison.OrdinalIgnoreCase));

            string url = $"{_cfg.ServerConfig.ApiBaseUrl.TrimEnd('/')}/{_cfg.ServerConfig.ApiVersion.Trim('/')}/{ep.Path.TrimStart('/')}";
            var headers = BuildHeaders();

            HttpClientWithRetry.HttpClientResult raw;
            if (string.Equals(ep.Method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                raw = await _http.GetAsync(url, headers, ct);
            }
            else // POST, PUT, PATCH, DELETE handled as POST for brevity
            {
                string json = body == null ? null : JsonConvert.SerializeObject(body);
                raw = await _http.PostAsync(url, null, json, headers, ct);
            }

            if (!raw.IsSuccess)
            {
                return new DataResult<TDomain>(
                    false,
                    raw.StatusCode,
                    default,
                    "Error",
                    "No data received.",
                    raw.ErrorMessage
                    );
            }

            var dto = JsonConvert.DeserializeObject<TDto>(raw.Data);
            var domainObj = map(dto);
            return new DataResult<TDomain>(true, raw.StatusCode, domainObj, "Success");
        }

        private Dictionary<string, string> BuildHeaders()
        {
            var dict = _cfg.ServerConfig.Headers?.ToDictionary(h => h.Key, h => h.Value)
                       ?? new Dictionary<string, string>();
            dict["Authorization"] = $"Bearer {_tokens.GetAccessTokenAsync().GetAwaiter().GetResult()}";
            return dict;
        }

        public async UniTask DisposeServiceAsync()
        {
            await UniTask.CompletedTask;
        }

        private class EndpointMap
        {
            public Type     DtoType;
            public Delegate Mapper;   // Func<TDto, TDomain>
        }
    }
}