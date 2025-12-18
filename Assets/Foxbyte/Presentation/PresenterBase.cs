using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.Permission;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace Foxbyte.Presentation
{
    /// <summary>
    /// Provides the fields: <see cref="View"/>, <see cref="Model"/>, <see cref="TargetGameObjectForView"/> and <see cref="Permissions"/>.
    /// TargetGameObjectForView is the GameObject that the view will be attached to.
    /// The <see cref="Permissions"/> property is used to define the permissions required
    /// to access the presenter and if the UIManager will show the view or not.
    /// Set the RequiredPermissions property in the constructor to define the permissions.
    /// Override the methods <see cref="InitializeBeforeUiAsync"/>,
    /// <see cref="InitializeAfterUiAsync"/> and <see cref="InitializeWithDataBeforeUiAsync{T}"/>
    /// to customize the initialization logic of the presenter.
    /// Use NoModel as the model type if you don't need a model, like
    /// public class IntroPresenter : PresenterBase&lt;IntroView,NoModel&gt;
    /// </summary>
    public abstract class PresenterBase<TPresenter,TView,TModel> : IPresenter, IDisposable
        where TPresenter : PresenterBase<TPresenter,TView,TModel>
        where TView : ViewBase<TPresenter>
        where TModel : PresenterModelBase
    {
        public TView  View  { get; protected set; }
        public TModel Model { get; protected set; }
        ViewBase IPresenter.BaseView  => View;
        PresenterModelBase IPresenter.BaseModel => Model;
        public GameObject TargetGameObjectForView {get;}
        public UIDocument UiDocument { get; }
        public StyleSheet StyleSheet { get; }
        public VisualElement TargetContainer { get; }
        public bool IsViewMounted { get; private set; }
        
        // Lifetime cancellation token for the presenter and model
        // Use separate tokens for operations that should have their own lifetime
        private readonly CancellationTokenSource _lifetimeCts = new();
        public CancellationToken Token => _lifetimeCts.Token;
        // Presenter's view mount/unmount token
        private CancellationTokenSource _mountCts;
        public CancellationToken MountToken => _mountCts?.Token ?? CancellationToken.None;
        // Model-scope CTS: linked(lifetime + per-mount + view-mount). Always attached to Model.
        //private CancellationTokenSource _modelScopeCts;
        private readonly Dictionary<VisualElement,CancellationTokenSource> _guardTokens = new();
        
        private readonly Dictionary<VisualElement,double> _lastClicked = new();
        private readonly HashSet<VisualElement> _busy = new();
        private bool _afterUiInitialized;
        
        public RequiredPermissions Permissions { get; } = new RequiredPermissions
        {
            Roles = new List<string>(),
            Permissions = new List<string>() 
        };

        /// <summary>
        /// Called when the view is unmounted from the component or control.
        /// Use to unsubscribe from events or perform other cleanup actions.
        /// </summary>
        /// <remarks>This method is intended to be overridden in derived classes to perform cleanup or
        /// other  actions when the view is no longer attached. The base implementation does nothing.</remarks>
        protected virtual async UniTask OnViewUnmountedAsync()
        {
            await UniTask.Yield();
        }

        // Parent / child presenter communication
        // Presenter can call UIManager to show another presenter and can keep a handle to the child presenter
        // Child can use a callback to notify the parent presenter of updates
        // Parent presenter has an overridable default method to handle updates from child presenters

        /// <summary>
        /// Exposes a callback to the parent presenter to update it from a child presenter.
        /// Use it to notify the parent presenter of changes with optional payload.
        /// When instantiating child presenters, you can pass this callback to the UIManager
        /// like this: UIManager.ShowAsync(other, stuff, UpdateParent);
        /// In child presenters, you can call this callback to notify the parent presenter of updates.
        /// Like this: 
        /// </summary>
        public Func<object,UniTask> UpdateParent => OnChildUpdate;
        /// <summary>
        /// Is set automatically by the UIManager to the parent presenter callback.
        /// Use it in child like this:
        /// var slider = View.RootOfThisView.Q<Slider>("RadiusSlider");
        /// slider.RegisterValueChangedCallback(e =>
        /// UniTask.Void(async () => { await ChildUpdate((Id: "Radius", Value: e.newValue)); }));
        /// Or .Forget() or _ = func(payload) if no await is needed.
        /// </summary>
        public Func<object,UniTask> ChildUpdate { get; set; }

        /// <summary>
        /// Override this method to handle updates from child presenters.
        /// </summary>
        protected virtual UniTask OnChildUpdate(object payload) 
        { 
            return UniTask.CompletedTask; 
        }
        
        /// <summary>
        /// Constructor for the presenter that initializes the view, model, and permissions.
        /// UIManager will use this constructor to create the presenter and provides
        /// the required parameters.
        /// </summary>
        /// <param name="targetGameobjectForView"></param>
        /// <param name="uidoc"></param>
        /// <param name="styleSheet"></param>
        /// <param name="target"></param>
        /// <param name="model"></param>
        /// <param name="perms"></param>
        protected PresenterBase(GameObject targetGameobjectForView, UIDocument uidoc, StyleSheet styleSheet, VisualElement target, 
            TModel model = null, RequiredPermissions perms = null)
        {

            if (!targetGameobjectForView) throw new ArgumentNullException(nameof(targetGameobjectForView));
            if (!uidoc) throw new ArgumentNullException(nameof(uidoc));
            if (target == null) throw new ArgumentNullException(nameof(target));

            TargetGameObjectForView = targetGameobjectForView;
            UiDocument = uidoc;
            StyleSheet = styleSheet;
            TargetContainer = target;
            Model = model;
            SetRequiredPermissions(perms);
            // give model the same lifetime as the presenter
            Model?.AttachCancellationToken(Token);
        }

        private void BeginMountLifetime()
        {
            _mountCts?.Cancel();
            _mountCts?.Dispose();
            _mountCts = new CancellationTokenSource();
        }

        /// <summary>
        /// Build a CTS linked to presenter lifetime + per-mount + view mount + optional external.
        /// Caller must Dispose().
        /// </summary>
        protected CancellationTokenSource CreateLinkedCts(CancellationToken? external = null)
        {
            var tokens = new List<CancellationToken>(4) { Token };
            if (_mountCts != null) tokens.Add(_mountCts.Token);
            if (View != null && View.MountToken != CancellationToken.None) tokens.Add(View.MountToken);
            if (external.HasValue) tokens.Add(external.Value);
            return CancellationTokenSource.CreateLinkedTokenSource(tokens.ToArray());
        }

        /// <summary>
        /// Convenience alias for UI-bound work.
        /// Caller must Dispose().
        /// </summary>
        protected CancellationTokenSource UiScopeCts(CancellationToken? external = null) => CreateLinkedCts(external);

        /// <summary>
        /// Refresh the model-scope linked token and attach to Model.
        /// </summary>
        //private void RefreshModelScopeToken()
        //{
        //    _modelScopeCts?.Cancel();
        //    _modelScopeCts?.Dispose();
        //    _modelScopeCts = CreateLinkedCts();
        //    Model?.AttachCancellationToken(_modelScopeCts.Token);
        //}

        /// <summary>
        /// Public hook so external code (e.g., UIManager) can notify that the view mounted/rebuilt.
        /// This re-links and attaches the model-scope token including the new view mount token.
        /// </summary>
        public void NotifyViewMounted()
        {
            IsViewMounted = true;
            //RefreshModelScopeToken();
        }

        /// <summary>
        /// Initialize the view.
        /// When overriding this method, ensure to call the base method first to initialize the view.
        /// DO NOT run long-running task in there to not block the UIManager.
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask InitializeUIAsync()
        {
            await UniTask.Yield();
            View = TargetGameObjectForView.GetComponent<TView>() ?? TargetGameObjectForView.AddComponent<TView>();
            View.Presenter = (TPresenter)this;
            View.UIDocOfThisView = UiDocument;
            View.StyleSheetOfThisView = StyleSheet;
            View.ContainerOfThisView = TargetContainer;
            View.RootOfThisView = UiDocument.rootVisualElement; 
            if (View.RootOfThisView == null)
            {
                Debug.LogError($"RootElement not found in {TargetContainer.name}. Please ensure it exists in the UI document.");
                return;
            }
            // start view mount lifetime
            BeginMountLifetime();
            await View.InitAsync();
            // attach linked model-scope after view mount exists
            //RefreshModelScopeToken();
        }

        /// <summary>
        /// The method is called automatically by the UIManager when the presenter is created.
        /// Override this method to customize the initialization logic of the presenter.
        /// Attention: The view has not been initialized yet, so you cannot access it here.
        /// DO NOT run long-running task in there to not block the UIManager.
        /// </summary>
        public virtual async UniTask InitializeBeforeUiAsync()
        {
            await Model.InitializeAsync();
            await UniTask.Yield();
        }

        /// <summary>
        /// Override this method to customize the initialization logic of the presenter
        /// before the UI is initialized.
        /// DO NOT run long-running task in there to not block the UIManager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public virtual async UniTask InitializeWithDataBeforeUiAsync<T>(T data)
        {
            await UniTask.Yield();
        }

        /// <summary>
        /// Override this method to customize the final initialization logic of the presenter.
        /// This method is called after all other initialization methods and the view has been initialized.
        /// Use it to set up event handlers, update UI with data, etc.
        /// DO NOT run long-running task in there to not block the UIManager.
        /// </summary>
        public virtual async UniTask InitializeAfterUiAsync()
        {
            await UniTask.Yield();
        }
        
        public async UniTask EnsureAfterUiInitializedAsync()
        {
            if (_afterUiInitialized) return;
            await InitializeAfterUiAsync();
            _afterUiInitialized = true;
        }

        /// <summary>
        /// Notify the presenter that the view has been unmounted.
        /// Called by UIManager when the view is unmounted.
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask NotifyViewUnmountedAsync()
        {
            await OnViewUnmountedAsync(); // allow concrete presenters to unsubscribe
            await UniTask.WaitForEndOfFrame();
        }

        public void MarkUnmounted()
        {
            _afterUiInitialized = false;
            _mountCts?.Cancel();
            _mountCts?.Dispose();
            _mountCts = null;
            //_modelScopeCts?.Cancel();
            //_modelScopeCts?.Dispose();
            //_modelScopeCts = null;
            IsViewMounted = false;
            View = null;
        }

        private void SetRequiredPermissions(RequiredPermissions requiredPermissions)
        {
            if (requiredPermissions == null) return;
            Permissions.Roles = requiredPermissions.Roles ?? new List<string>();
            Permissions.Permissions = requiredPermissions.Permissions ?? new List<string>();
        }

        // Button mashing prevention with default minimal cooldown, using guardTokens to manage cooldowns
        protected virtual double DefaultCooldown => .2;


        /// <summary>
        /// For short actions like button clicks, this method checks if the action is allowed based on cooldown.
        /// Cooldown is in seconds.
        /// Example usage in callback: if (!Allow(button, minCooldown)) return; 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="cooldown"></param>
        /// <returns></returns>
        protected bool Allow(VisualElement src, double? cooldown = null)
        {
            double cd  = cooldown ?? DefaultCooldown;
            double now = Time.realtimeSinceStartupAsDouble;
            if (_busy.Contains(src))  return false;
            if (_lastClicked.TryGetValue(src, out var last) && now - last < cd) 
                return false;
            _lastClicked[src] = now;
            return true;
        }

        // helper to build a linked token that always includes presenter + mount (+ view mount) + optional external
        protected CancellationToken CreateLinkedToken(CancellationToken? external = null)
        {
            var tokens = new List<CancellationToken>(4) { Token };
            if (_mountCts != null) tokens.Add(_mountCts.Token);
            if (View != null && View.MountToken != CancellationToken.None) tokens.Add(View.MountToken);
            if (external.HasValue) tokens.Add(external.Value);
            return CancellationTokenSource.CreateLinkedTokenSource(tokens.ToArray()).Token;
        }

        // Convenience alias for UI-bound work
        protected CancellationToken UiScopeToken(CancellationToken? external = null) => CreateLinkedToken(external);

        /// <summary>
        /// Runs a job asynchronously with a cancellation token and cooldown prevention.
        /// Intended for use with UI elements that trigger long-running tasks.
        /// Example usage: await RunGuardedAsync(button, async (ct) => { await SomeAsyncJob(ct); });
        /// </summary>
        /// <param name="src"></param>
        /// <param name="job"></param>
        /// <param name="minCooldown"></param>
        /// <param name="external"></param>
        /// <returns></returns>
        protected async UniTask RunGuardedAsync(
            VisualElement src,
            Func<CancellationToken,UniTask> job,
            double? minCooldown = null,
            CancellationToken? external = null)
        {
            if (_busy.Contains(src)) return;
            if (!Allow(src, minCooldown)) return;

            var cts = UiScopeCts(external);

            _busy.Add(src);
            _guardTokens[src] = cts;
            try
            {
                await job(cts.Token);
            }
            finally
            {
                _busy.Remove(src);
                _guardTokens.Remove(src); 
                _lastClicked[src] = Time.realtimeSinceStartupAsDouble; // restart cooldown
                cts.Dispose();
            }
        }

        /// Optional: another control can cancel the running task
        /// Example usage: Cancel(button); // to cancel the job running on this button
        protected void Cancel(VisualElement src)
        {
            if (_guardTokens.TryGetValue(src, out var cts))
                cts.Cancel();
        }

        /// <summary>
        /// Presenter-owned background tasks tied to the current UI lifetime (not button-guarded).
        /// </summary>
        public async UniTask RunWithUiLifetimeAsync(Func<CancellationToken, UniTask> job, CancellationToken? external = null)
        {
            var cts = UiScopeCts(external);
            try
            {
                await job(cts.Token);
            }
            finally
            {
                cts.Dispose();
            }
        }

        /// <summary>
        /// Replace the current model with a new one. Attaches the current model-scope token.
        /// </summary>
        /// <param name="newModel"></param>
        public void ReplaceModel(TModel newModel)
        {
            if (newModel == null) return;
            Model = newModel;
            Model.AttachCancellationToken(Token);
            //if (_modelScopeCts != null)
            //    Model.AttachCancellationToken(_modelScopeCts.Token);
            //else
            //    Model.AttachCancellationToken(Token);
        }

        /// <summary>
        /// Called by UIManager after a model has been replaced. Default: initialize + load.
        /// Override if you need a different sequence.
        /// </summary>
        public virtual async UniTask OnModelReplacedAsync(object initData)
        {
            // model uses its attached token
            await Model.InitializeAsync();
            if (initData != null)
                await Model.InitializeWithDataAsync(initData);
            await Model.LoadDataAsync();
        }

        public virtual void Dispose()
        {
            _mountCts?.Cancel();
            _mountCts?.Dispose();
            _mountCts = null;

            //_modelScopeCts?.Cancel();
            //_modelScopeCts?.Dispose();
            //_modelScopeCts = null;

            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();

            _afterUiInitialized = false;
        }
    }

    public sealed class NoModel : PresenterModelBase
    {
        public static readonly NoModel Instance = new();
        private NoModel() { }
    }

    public interface IPresenter
    {
        ViewBase BaseView { get; }
        PresenterModelBase BaseModel { get; }
        UniTask EnsureAfterUiInitializedAsync();
        void MarkUnmounted();
        UniTask NotifyViewUnmountedAsync();
    }
}