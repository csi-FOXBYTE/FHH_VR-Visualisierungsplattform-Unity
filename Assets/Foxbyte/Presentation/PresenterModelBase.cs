using Cysharp.Threading.Tasks;
using System.Threading;

namespace Foxbyte.Presentation
{
    public abstract class PresenterModelBase
    {
        protected CancellationToken CancellationToken { get; private set; }

        // hand over the cancellation token from the presenter to the model
        // so that the model can use it to cancel any ongoing operations
        public void AttachCancellationToken(CancellationToken token) => CancellationToken = token;

        protected PresenterModelBase()
        {}

        public virtual async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// For cases where you e.g. provided data to the UIManager method and
        /// want to initialize the model with that data, or simply want to initialize the model
        /// before the UI is initialized.
        /// Not automatically called. Use e.g. Presenter.InitializeBeforeUIAsync() to manually call this method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async UniTask InitializeWithDataAsync<T>(T data)
        {
            await UniTask.CompletedTask;
        }
                
        public virtual async UniTask LoadDataAsync()
        {
            await UniTask.CompletedTask;
        }

        public virtual async UniTask SaveDataAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}