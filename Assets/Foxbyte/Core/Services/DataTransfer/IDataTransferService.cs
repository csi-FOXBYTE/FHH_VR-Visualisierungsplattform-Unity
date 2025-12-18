using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Foxbyte.Core.Services.DataTransfer
{
    public interface IDataTransferService
    {
        void RegisterEndpoint<TDto, TDomain>(string name, Func<TDto, TDomain> map);
        UniTask<DataResult<TDomain>> FetchAsync<TDomain>(
            string endpointName,
            object body = null,
            CancellationToken ct = default);

    }
}