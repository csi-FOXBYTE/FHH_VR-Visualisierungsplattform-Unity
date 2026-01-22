using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;

namespace FHH.UI.Example
{
    public sealed class ExampleModel : PresenterModelBase
    {
        public override async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }

        public override async UniTask LoadDataAsync()
        {
            await UniTask.CompletedTask;
        }

        public override async UniTask SaveDataAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}