using Cysharp.Threading.Tasks;

namespace Foxbyte.Core
{
    public interface IConfigurationService
    {
        UniTask LoadAppConfig();
    }
}