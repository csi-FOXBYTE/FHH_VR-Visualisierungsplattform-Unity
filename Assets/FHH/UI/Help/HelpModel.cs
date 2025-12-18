using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;

namespace FHH.UI.Help
{
    public sealed class HelpModel : PresenterModelBase
    {
        public override async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}