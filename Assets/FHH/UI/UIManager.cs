using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using FHH.Logic;
using Foxbyte.Core;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;
using Foxbyte.Presentation.Extensions;
using UnityEngine.Localization.Settings;
using UnityEngine.InputSystem;

namespace FHH.UI
{
    public enum UIMode { Overlay, WorldSpace }

    public enum AnchorCorner { TopLeft, TopRight, BottomLeft, BottomRight }

    public sealed class UIManager : IAppServiceAsync
    {
        private readonly Dictionary<Type, IPresenter> _presentersByType = new();
        private readonly Dictionary<UIProjectContext.UIRegion, List<Type>> _regionToPresenters = new();

        private readonly HashSet<UIProjectContext.UIRegion> _ephemeralRegions =
            new()
            {
                UIProjectContext.UIRegion.Toast,
                UIProjectContext.UIRegion.Overlay,
                UIProjectContext.UIRegion.Tooltip,
                UIProjectContext.UIRegion.Context,
                UIProjectContext.UIRegion.Popover
            };

        private sealed class MountRecord
        {
            public VisualElement Root;
            public VisualElement Container;
            public UIDocument Doc;
            public bool Ephemeral;
        }
        private readonly Dictionary<Type, MountRecord> _mountRecords = new();

        private UIDocument _uiDocument;
        private PanelSettings _overlayPanelSettings;
        private PanelSettings _worldspacePanelSettings;
        private MonoBehaviour _panelInputConfig;
        private UIMode _mode = UIMode.Overlay;

        private UIProjectContext _uiProjectContext;
        private readonly Dictionary<UIProjectContext.UIRegion, VisualElement> _regionMap = new();

        private VisualElement _glassPane; // used for ephemeral auto-hide

        private readonly SemaphoreSlim _transition = new(1, 1);
        //private bool _isShowing;
        //private bool _isHiding;

        private const float DefaultFadeSec = 0.2f;

        // Notification queue
        private readonly Queue<ToastItem> _toastQueue = new();
        private bool _toastBusy;

        private class ToastItem
        {
            public string Message;
            public float Seconds;
            public Action OnClick;
            public string Entry;
            public object[] Args;
            public string Unlocalized;
        }

        private InputAction _uiModeSwitch;
        
        public async UniTask InitServiceAsync()
        {
            _uiModeSwitch = InputSystem.actions.FindAction("UIModeSwitch", true);
            _uiModeSwitch.performed += OnUiSwitchPerformed;
            _uiModeSwitch.Enable();
            await UniTask.CompletedTask;
        }

        private void OnUiSwitchPerformed(InputAction.CallbackContext obj)
        {
            SetUIMode(_mode == UIMode.WorldSpace ? UIMode.Overlay : UIMode.WorldSpace, null);
        }

        public async UniTask DisposeServiceAsync()
        {
            foreach (var p in _presentersByType.Values)
                (p as IDisposable)?.Dispose();

            _presentersByType.Clear();
            _regionToPresenters.Clear();
            _regionMap.Clear();
            _glassPane = null;

            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Call once at app startup.
        /// </summary>
        public async UniTask InitializeRootAsync(
            UIDocument uidoc,
            PanelSettings overlayPS,
            PanelSettings worldspacePS,
            UIProjectContext projectContext,
            MonoBehaviour panelInputConfigForWorldspace = null)
        {
            _uiDocument = uidoc ?? throw new ArgumentNullException(nameof(uidoc));
            _overlayPanelSettings = overlayPS;
            _worldspacePanelSettings = worldspacePS;
            _panelInputConfig = panelInputConfigForWorldspace;

            _uiProjectContext = projectContext ?? new UIProjectContext();
            await _uiProjectContext.InitializeAsync(_uiDocument);

            // Cache region visual elements
            foreach (UIProjectContext.UIRegion r in Enum.GetValues(typeof(UIProjectContext.UIRegion)))
            {
                if (_uiProjectContext.TryGet(r, out var ve))
                    _regionMap[r] = ve;
            }

            EnsureGlassPane();

            // Default to Overlay mode on startup
            //SetUIMode(UIMode.Overlay, null);
        }

        private void EnsureGlassPane()
        {
            if (_glassPane != null) return;
            if (!_regionMap.TryGetValue(UIProjectContext.UIRegion.Overlay, out var overlay))
            {
                // Fallback: create a full-screen glass on root
                overlay = _uiDocument.rootVisualElement;
            }

            _glassPane = new VisualElement { name = "UIManager_GlassPane" };
            _glassPane.style.position = Position.Absolute;
            _glassPane.style.left = _glassPane.style.top = _glassPane.style.right = _glassPane.style.bottom = 0;
            _glassPane.pickingMode = PickingMode.Position;
            _glassPane.style.backgroundColor = new Color(0, 0, 0, 0);
            _glassPane.style.display = DisplayStyle.None;

            // Handle click-to-dismiss for ephemeral content (Modal excluded)
            _glassPane.RegisterCallback<PointerDownEvent>(evt =>
            {
                // Determine if click is inside any ephemeral element; if not, hide them
                var clicked = evt.target as VisualElement;
                if (clicked == null) { HideAllEphemeral(); return; }

                bool insideAny = false;
                foreach (var region in _ephemeralRegions)
                {
                    if (!_regionMap.TryGetValue(region, out var ve)) continue;
                    if (clicked.IsDescendantOf(ve)) { insideAny = true; break; }
                }
                if (!insideAny) HideAllEphemeral();
            }, TrickleDown.TrickleDown);

            _uiDocument.rootVisualElement.Add(_glassPane);
            _glassPane.SendToBack();
        }

        public void ToggleUIMode()
        {
            if (_mode == UIMode.WorldSpace)
                SetUIMode(UIMode.Overlay, null);
            else
                SetUIMode(UIMode.WorldSpace, null);
        }

        public void SetUIMode(UIMode mode, Transform worldSpaceMount)
        {
            _mode = mode;
            if (_uiDocument == null) return;

            if (_mode == UIMode.Overlay)
            {
                if (_overlayPanelSettings != null)
                    _uiDocument.panelSettings = _overlayPanelSettings;
                //if (_panelInputConfig != null) _panelInputConfig.enabled = false;
                _uiDocument.rootVisualElement.style.height = Length.Percent(100);
                _uiDocument.rootVisualElement.style.width = Length.Percent(100);
            }
            else
            {
                if (_worldspacePanelSettings != null)
                    _uiDocument.panelSettings = _worldspacePanelSettings;
                //if (_panelInputConfig != null) _panelInputConfig.enabled = true;

                if (worldSpaceMount != null)
                {
                    _uiDocument.transform.SetParent(worldSpaceMount, worldPositionStays: false);
                }
                var distance = 3f;
                float verticalSizeWorld = 2f * distance * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad);
                float horizontalSizeWorld = verticalSizeWorld * Camera.main.aspect;
                float widthPixels  = horizontalSizeWorld * 100f;
                float heightPixels = verticalSizeWorld * 100f;
                _uiDocument.rootVisualElement.style.height = Length.Pixels(heightPixels);
                _uiDocument.rootVisualElement.style.width = Length.Pixels(widthPixels);
            }
        }

        public async UniTask<WindowHandle> ShowWindowAsync<TPresenter, TView, TModel>(
            TModel model = default,
            WindowOptions options = null,
            object initData = null,
            Func<object, UniTask> callback = null,
            CancellationToken ct = default)
            where TPresenter : PresenterBase<TPresenter, TView, TModel>
            where TView : ViewBase<TPresenter>
            where TModel : PresenterModelBase
        {
            options ??= new WindowOptions();

            await _transition.WaitAsync(ct);
            try
            {
                var region = ResolveRegion<TPresenter>(options);
                if (!_uiDocument || !_regionMap.TryGetValue(region, out var container))
                {
                    ULog.Error($"[UIManager] Region container not found: {region}");
                    return null;
                }

                if (options.CloseOthersInRegion)
                {
                    await HideRegionPresentersInternalAsync(region, options.FadeOutSec ?? DefaultFadeSec, untrack: true);
                }

                var containerToUse = options.TargetContainer ?? container;

                if (!IsAllowed(options.Permissions))
                    return null;

                var type = typeof(TPresenter);

                if (!_presentersByType.TryGetValue(type, out var existing))
                {
                    var presenter = Activator.CreateInstance(
                        type,
                        options.TargetGameObject ?? _uiDocument.gameObject,
                        options.UIDoc ?? _uiDocument,
                        options.StyleSheet,
                        containerToUse,
                        model,
                        options.Permissions) as TPresenter;

                    if (presenter == null)
                    {
                        ULog.Error($"[UIManager] Failed to create presenter of type {type.Name}");
                        return null;
                    }

                    if (callback != null)
                        presenter.ChildUpdate = callback;

                    await presenter.InitializeBeforeUiAsync();
                    if (initData != null)
                        await presenter.InitializeWithDataBeforeUiAsync(initData);
                    
                    if (!ShouldBuildViewForPresenter(presenter)) // for UIs with "never show again" logic
                    {
                        (presenter as IDisposable)?.Dispose();
                        return null;
                    }

                    await presenter.InitializeUIAsync();
                    //await presenter.InitializeAfterUiAsync();
                    await ((IPresenter)presenter).EnsureAfterUiInitializedAsync();

                    _presentersByType[type] = presenter;
                    TrackPresenterRegion(type, region);
                    existing = presenter;
                }
                else
                {
                    var presenter = (TPresenter)existing;

                    if (model != null)
                    {
                        presenter.ReplaceModel(model);
                        await presenter.OnModelReplacedAsync(initData);
                    }

                    if (initData != null)
                        await presenter.InitializeWithDataBeforeUiAsync(initData);

                    if (!ShouldBuildViewForPresenter(presenter))
                    {
                        // if it was visible before and the condition has changed, hide it now
                        await HidePresenterInternalAsync(type, options.FadeOutSec ?? DefaultFadeSec);
                        return null;
                    }
                    
                    var newContainer = options.TargetContainer ?? container;
                    var oldContainer = presenter.View?.ContainerOfThisView;

                    bool containerChanged = oldContainer != newContainer;
                    bool mustRebuild = options.ForceRebuild || containerChanged;

                    // destroyed or removed?
                    bool viewMissing = presenter.View == null;
                    bool unmounted   = viewMissing
                                       || presenter.View.RootOfThisView == null
                                       || presenter.View.RootOfThisView.panel == null
                                       || presenter.View.RootOfThisView.parent == null;

                    // if a root still hangs around, remove it
                    if (!viewMissing)
                    {
                        var oldRoot = presenter.View.RootOfThisView;
                        if (oldRoot != null && oldRoot.parent != null)
                            oldRoot.RemoveFromHierarchy();
                    }

                    if (viewMissing)
                    {
                        // Builds once (InitializeUIAsync internally calls View.InitAsync)
                        await presenter.InitializeUIAsync();

                        // If caller changed doc/container/stylesheet compared to the presenter’s current settings, re-mount once
                        if (containerChanged || options.UIDoc != null || options.StyleSheet != null)
                        {
                            var builtRoot = presenter.View.RootOfThisView;
                            if (builtRoot != null && builtRoot.parent != null)
                                builtRoot.RemoveFromHierarchy();

                            presenter.View.ContainerOfThisView  = newContainer;
                            presenter.View.UIDocOfThisView      = options.UIDoc ?? presenter.View.UIDocOfThisView;
                            presenter.View.StyleSheetOfThisView = options.StyleSheet ?? presenter.View.StyleSheetOfThisView;

                            await presenter.View.InitAsync(); // one extra build only when overrides differ
                        }
                    }
                    else if (mustRebuild || unmounted)
                    {
                        // Move or force-rebuild existing component
                        presenter.View.ContainerOfThisView = newContainer;
                        presenter.View.UIDocOfThisView = options.UIDoc ?? presenter.View.UIDocOfThisView;
                        presenter.View.StyleSheetOfThisView = options.StyleSheet ?? presenter.View.StyleSheetOfThisView;

                        await presenter.View.InitAsync(); // single rebuild, no container Clear()
                    }

                    presenter.NotifyViewMounted();
                    await ((IPresenter)presenter).EnsureAfterUiInitializedAsync();

                    if (FindRegionOfPresenter(type) == null)
                        TrackPresenterRegion(type, region);


                }

                //await PrepareRegionForShowAsync(region, options);
                if (options.TargetContainer != null)
                {
                    containerToUse.style.display = DisplayStyle.Flex;
                }
                else
                {
                    await PrepareRegionForShowAsync(region, options);
                    containerToUse.style.opacity = 1f;
                }
                //await FadeRegion(containerToUse, true, options.FadeInSec ?? DefaultFadeSec);

                // identify the specific presenter's root
                var root = ((IPresenter)existing).BaseView?.RootOfThisView;
                
                // apply absolute positioning to the specific element (root if present),
                // even when TargetContainer is provided (so regions are bypassed)
                if (options.Position.HasValue)
                {
                    var target = root ?? containerToUse;
                    target.style.position = Position.Absolute;
                    target.style.left = options.Position.Value.x;
                    target.style.top = options.Position.Value.y;
                }

                // fade in only the presenter's root, not the whole region
                await FadeRegion(root ?? containerToUse, true, options.FadeInSec ?? DefaultFadeSec); 


                if (options.Anchor != null && _ephemeralRegions.Contains(region))
                    AlignToAnchor(
                        root ?? containerToUse, 
                        options.Anchor, 
                        options.AnchorCorner, 
                        options.PopupCorner,
                        options.AnchorOffset);

                if (_ephemeralRegions.Contains(region) && options.TargetContainer == null)
                {
                    ShowGlassPane();
                    _glassPane?.BringToFront();
                    (root ?? containerToUse).BringToFront();
                }
                else if (!AnyEphemeralVisible())
                {
                    HideGlassPane();
                }

                var t = typeof(TPresenter);
                _mountRecords[t] = new MountRecord
                {
                    Root = root,
                    Container = containerToUse,
                    Doc = options.UIDoc ?? _uiDocument,
                    Ephemeral = _ephemeralRegions.Contains(region) && options.TargetContainer == null
                };

                return new WindowHandle((IPresenter)existing, options, this);
            }
            finally
            {
                _transition.Release();
            }
        }
        
        private static bool ShouldBuildViewForPresenter(object presenter)
        {
            if (presenter is IConditionalViewPresenter conditional)
                return conditional.ShouldBuildView;

            return true; // default: build for all other presenters
        }

        public async UniTask HideAsync<TPresenter>(float? fadeOutSec = null)
            where TPresenter : IPresenter
        {
            await HideAsync(typeof(TPresenter), fadeOutSec);
        }

        public async UniTask HideAsync(Type presenterType, float? fadeOutSec = null)
        {
            await _transition.WaitAsync();
            try
            {
                await HidePresenterInternalAsync(presenterType, fadeOutSec ?? DefaultFadeSec);
            }
            finally
            {
                _transition.Release();
            }
        }

        private async UniTask HidePresenterInternalAsync(Type presenterType, float fadeOutSec)
        {
            if (presenterType == null) return;
            if (!_presentersByType.TryGetValue(presenterType, out var p) || p == null)
                return;

            _mountRecords.TryGetValue(presenterType, out var rec);

            var root = rec?.Root ?? GetPresenterRoot(presenterType);
            if (root != null)
            {
                await FadeElementAsync(root, false, fadeOutSec);
                if (root.parent != null) root.RemoveFromHierarchy();
            }

            await ((IPresenter)p).NotifyViewUnmountedAsync();

            var vb = p.BaseView as ViewBase;
            if (vb != null)
                UnityEngine.Object.Destroy(vb);

            await UniTask.WaitForEndOfFrame();
            p.MarkUnmounted();

            if (rec != null)
                _mountRecords.Remove(presenterType);

            if (!AnyEphemeralVisible())
                HideGlassPane();

            if (IsShown(presenterType))
            {
                ULog.Warning($"[UIManager] Post-hide sanity detected visible root for {presenterType.Name}; forcing removal.");
                var r2 = GetPresenterRoot(presenterType);
                if (r2 != null && r2.parent != null)
                    r2.RemoveFromHierarchy();
            }
        }

        public bool IsShown<TPresenter>() where TPresenter : IPresenter
        {
            return IsShown(typeof(TPresenter));
        }

        public bool IsShown(Type presenterType)
        {
            if (presenterType == null) return false;
            if (!_presentersByType.TryGetValue(presenterType, out var ip) || ip?.BaseView == null)
                return false;

            var region = FindRegionOfPresenter(presenterType);
            if (region == null) return false;

            // must have a root that is currently mounted & visible
            var root = ip.BaseView.RootOfThisView;
            if (root == null) return false;

            // mounted to a panel? (null if removed from hierarchy)
            if (root.panel == null) return false;
            
            return root.resolvedStyle.display == DisplayStyle.Flex
                   && root.resolvedStyle.opacity > 0f;
        }

        private VisualElement GetPresenterRoot(Type presenterType)
        {
            if (!_presentersByType.TryGetValue(presenterType, out var ip) || ip?.BaseView == null)
                return null;
            return ip.BaseView.RootOfThisView;
        }

        private VisualElement GetPresenterContainer(Type presenterType)
        {
            if (!_presentersByType.TryGetValue(presenterType, out var ip) || ip?.BaseView == null)
                return null;
            return ip.BaseView.ContainerOfThisView;
        }

        private async UniTask FadeElementAsync(VisualElement element, bool show, float seconds)
        {
            if (element == null) return;

            if (show)
            {
                element.style.display = DisplayStyle.Flex;
                element.style.opacity = 0f;
                await element.FadeToAsync(1f, seconds);
            }
            else
            {
                await element.FadeToAsync(0f, seconds);
                element.style.display = DisplayStyle.None;
            }
        }
        
        //  hide all presenters currently tracked for a region, but only if actually mounted in that region
        private async UniTask HideRegionPresentersInternalAsync(
            UIProjectContext.UIRegion region,
            float fadeOutSec,
            bool untrack = true)
        {
            if (!_regionMap.TryGetValue(region, out var regionContainer))
                return;

            if (!_regionToPresenters.TryGetValue(region, out var list) || list.Count == 0)
                return;

            var snapshot = list.ToList(); // avoid modification during iteration

            foreach (var t in snapshot)
            {
                if (!_presentersByType.TryGetValue(t, out var ip) || ip?.BaseView == null)
                    continue;

                var view = ip.BaseView;
                var root = view.RootOfThisView;
                var presenterContainer = view.ContainerOfThisView;

                if (root == null)
                    continue;

                // only hide presenters actually mounted under this region
                var isInRegion =
                    presenterContainer == regionContainer ||
                    root.IsDescendantOf(regionContainer);

                if (!isInRegion)
                    continue;

                await HidePresenterInternalAsync(t, fadeOutSec);

                if (untrack)
                    UntrackPresenterRegion(t, region);
            }

            // region/container itself is left untouched:
            // we do NOT modify regionContainer.style or Clear() here
        }

        // optional public wrapper that locks
        public async UniTask HideRegionAsync(UIProjectContext.UIRegion region, float? fadeOutSec = null)
        {
            await _transition.WaitAsync();
            try
            {
                await HideRegionPresentersInternalAsync(region, fadeOutSec ?? DefaultFadeSec, untrack: true);
            }
            finally
            {
                _transition.Release();
            }
        }


        /// <summary>
        /// Hides all ephemeral regions (Toast, Overlay, Tooltip, Context, Popover) and their contents.
        /// </summary>
        public void HideAllEphemeral()
        {
            HideAllEphemeralAsync().Forget();
        }

        private async UniTask HideAllEphemeralAsync()
        {
            await _transition.WaitAsync();
            try
            {
                foreach (var region in _ephemeralRegions)
                {
                    if (!_regionMap.TryGetValue(region, out var container)) continue;

                    if (_regionToPresenters.TryGetValue(region, out var list) && list.Count > 0)
                    {
                        // copy to avoid mutation during iteration
                        var snapshot = list.ToList();

                        foreach (var t in snapshot)
                        {
                            if (_presentersByType.TryGetValue(t, out var ip) && ip?.BaseView != null)
                            {
                                var root = ip.BaseView.RootOfThisView;
                                if (root != null)
                                {
                                    await FadeRegion(root, false, DefaultFadeSec); // surgical fade-out
                                    // remove only this presenter's subtree
                                    if (root.parent != null)
                                        root.RemoveFromHierarchy();
                                }
                            }
                        }
                        list.Clear();
                    }
                    // collapse container only if empty
                    if (container.childCount == 0)
                    {
                        container.style.display = DisplayStyle.None;
                        container.style.opacity = 0f;
                    }
                }
                HideGlassPane();
            }
            finally
            {
                _transition.Release();
            }
        }


        /// <summary>
        /// Show a toast notification in the Toast region.
        /// If multiple toasts are requested, they are queued and shown one after another.
        /// Simple usage: ShowNotification("Hello world", 2.5f);
        /// Usage with localized title and unlocalized detail: ShowNotification("ServerError", 3f, null, "ERR-503: timeout");
        /// Usage with args: ShowNotification("GiftReceived", 3f, null, null, "a new sword");
        /// with table entry: GiftReceived:  You have received {0}! in Unity localization table "Notifications"
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="seconds"></param>
        /// <param name="onClick"></param>
        /// <param name="unlocalized"></param>
        /// <param name="args"></param>
        public void ShowNotification(string entry, float seconds = 2.5f, Action onClick = null, string unlocalized = null, params object[] args)
        {
            _toastQueue.Enqueue(new ToastItem
            {
                Entry = entry,
                Args = args, // ShowNotification("GiftReceived", 3f, null, null, "a new sword"); with table entry: GiftReceived:  You have received {0}!
                Unlocalized = unlocalized, // raw message if no localization
                Seconds = seconds,
                OnClick = onClick
            });
            if (!_toastBusy) ProcessToastQueueAsync().Forget();
        }

        
        private UIProjectContext.UIRegion ResolveRegion<TPresenter>(WindowOptions options)
        {
            if (options.Region.HasValue) return options.Region.Value;

            // Default region mapping per presenter type can be customized here if desired.
            // For now, default to Content1 for general UI, Popover for anchored ephemeral with Anchor provided.
            if (options.Anchor != null)
                return UIProjectContext.UIRegion.Popover;

            return UIProjectContext.UIRegion.Content1;
        }

        private bool IsAllowed(RequiredPermissions req)
        {
            if (req == null || (req.Roles.Count == 0 && req.Permissions.Count == 0)) return true;

            var permSvc = ServiceLocator.GetService<PermissionService>();
            if (permSvc == null) return true; // permissive fallback

            // ANY role OR ANY permission
            bool anyRole = req.Roles.Count > 0 && req.Roles.Any(permSvc.HasRole);
            bool anyPerm = req.Permissions.Count > 0 && req.Permissions.Any(permSvc.HasPermission);
            return anyRole || anyPerm;
        }

        private async UniTask PrepareRegionForShowAsync(UIProjectContext.UIRegion region, WindowOptions options)
        {
            if (!_regionMap.TryGetValue(region, out var container)) return;
            await UniTask.Yield();

            container.style.display = DisplayStyle.Flex;
            container.style.opacity = 0f;

            if (options.CenterScreen)
            {
                container.style.justifyContent = Justify.Center;
                container.style.alignItems = Align.Center;
            }

            if (options.Size.HasValue)
            {
                container.style.width = options.Size.Value.x;
                container.style.height = options.Size.Value.y;
            }
            if (options.Position.HasValue)
            {
                container.style.position = Position.Absolute;
                container.style.left = options.Position.Value.x;
                container.style.top = options.Position.Value.y;
            }
        }

        private async UniTask FadeRegion(VisualElement container, bool show, float seconds)
        {
            if (show)
            {
                container.style.display = DisplayStyle.Flex;
                await container.FadeToAsync(1f, seconds);
            }
            else
            {
                await container.FadeToAsync(0f, seconds);
                container.style.display = DisplayStyle.None;
            }
        }

        private void BringRegionToFront(VisualElement container)
        {
            container?.BringToFront();
        }

        private bool AnyEphemeralVisible()
        {
            foreach (var r in _ephemeralRegions)
            {
                if (_regionMap.TryGetValue(r, out var ve) && ve.resolvedStyle.display == DisplayStyle.Flex)
                    return true;
            }
            return false;
        }

        private void ShowGlassPane()
        {
            if (_glassPane == null) return;
            _glassPane.style.display = DisplayStyle.Flex;
        }

        private void HideGlassPane()
        {
            if (_glassPane == null) return;
            _glassPane.style.display = DisplayStyle.None;
        }

        private void TrackPresenterRegion(Type t, UIProjectContext.UIRegion region)
        {
            if (!_regionToPresenters.TryGetValue(region, out var list))
            {
                list = new List<Type>();
                _regionToPresenters[region] = list;
            }
            if (!list.Contains(t)) list.Add(t);
        }

        private void UntrackPresenterRegion(Type t, UIProjectContext.UIRegion region)
        {
            if (_regionToPresenters.TryGetValue(region, out var list))
                list.Remove(t);
        }

        private UIProjectContext.UIRegion? FindRegionOfPresenter(Type t)
        {
            foreach (var kv in _regionToPresenters)
                if (kv.Value.Contains(t)) return kv.Key;
            return null;
        }

        private void ClearContainerOfPresenter(VisualElement container, Type presenterType)
        {
            var root = GetPresenterRoot(presenterType);
            if (root == null) return;

            if (root.parent != null)
                root.RemoveFromHierarchy(); // remove just this presenter’s root

            // optional: collapse the region only if it’s now empty
            //if (container != null && container.childCount == 0)
            //{
            //    container.style.opacity = 0f;
            //    container.style.display = DisplayStyle.None;
            //}
        }

        private void AlignToAnchor(
            VisualElement popupContainer,
            VisualElement anchor,
            AnchorCorner anchorCorner,
            AnchorCorner popupCorner,
            Vector2 offset)
        {
            if (anchor == null || popupContainer == null) return;

            // Ensure layout ready
            popupContainer.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                var ab = anchor.worldBound;
                var pb = popupContainer.worldBound;

                Vector2 anchorPoint = anchorCorner switch
                {
                    AnchorCorner.TopLeft     => new Vector2(ab.xMin, ab.yMin),
                    AnchorCorner.TopRight    => new Vector2(ab.xMax, ab.yMin),
                    AnchorCorner.BottomLeft  => new Vector2(ab.xMin, ab.yMax),
                    AnchorCorner.BottomRight => new Vector2(ab.xMax, ab.yMax),
                    _ => new Vector2(ab.xMax, ab.yMax)
                };

                Vector2 popupOffset = popupCorner switch
                {
                    AnchorCorner.TopLeft     => new Vector2(0, 0),
                    AnchorCorner.TopRight    => new Vector2(pb.width, 0),
                    AnchorCorner.BottomLeft  => new Vector2(0, pb.height),
                    AnchorCorner.BottomRight => new Vector2(pb.width, pb.height),
                    _ => new Vector2(pb.width, pb.height)
                };

                // Convert world -> panel space
                var root = _uiDocument.rootVisualElement;
                var localAnchor = root.WorldToLocal(anchorPoint);
                var targetPos = localAnchor - popupOffset + offset;

                popupContainer.style.position = Position.Absolute;
                popupContainer.style.left = targetPos.x;
                popupContainer.style.top = targetPos.y;
            }, TrickleDown.NoTrickleDown);
        }

        

        private async UniTaskVoid ProcessToastQueueAsync()
        {
            _toastBusy = true;
            try
            {
                if (!_regionMap.TryGetValue(UIProjectContext.UIRegion.Toast, out var toastRegion))
                    return;

                while (_toastQueue.Count > 0)
                {
                    var item = _toastQueue.Dequeue();

                    var card = new VisualElement { name = "ToastItem" };
                    //card.style.paddingLeft = card.style.paddingRight = 12;
                    //card.style.paddingTop = card.style.paddingBottom = 8;
                    //card.style.borderTopLeftRadius = card.style.borderTopRightRadius =
                    //card.style.borderBottomLeftRadius = card.style.borderBottomRightRadius = 6;
                    //card.style.backgroundColor = new Color(0, 0, 0, 0.7f);
                    //card.style.backgroundColor = new Color(0, 0, 0, 0f);
                    //card.style.unityTextAlign = TextAnchor.MiddleCenter;

                    const string table = "Notifications";

                    // localized or raw
                    string primaryText;
                    if (!string.IsNullOrEmpty(item.Entry))
                    {
                        var handle = LocalizationSettings.StringDatabase
                            .GetLocalizedStringAsync(table, item.Entry, item.Args);
                        await handle.Task;
                        primaryText = string.IsNullOrEmpty(handle.Result)
                            ? $"[{table}.{item.Entry}]"
                            : handle.Result;
                    }
                    else
                    {
                        primaryText = string.IsNullOrEmpty(item.Message) ? "[missing text]" : item.Message;
                    }

                    var labelMain = new Label(primaryText);
                    labelMain.style.color = Color.black;
                    labelMain.style.unityFontStyleAndWeight = FontStyle.Bold;
                    card.Add(labelMain);

                    // optional second line (unlocalized detail)
                    if (!string.IsNullOrEmpty(item.Unlocalized))
                    {
                        var labelSub = new Label(item.Unlocalized);
                        labelSub.style.color = Color.black;
                        labelSub.style.unityFontStyleAndWeight = FontStyle.Normal;
                        labelSub.style.opacity = 1f;
                        labelSub.style.marginTop = 4;
                        card.Add(labelSub);
                    }

                    if (item.OnClick != null)
                    {
                        card.RegisterCallback<PointerDownEvent>(_ => item.OnClick());
                    }

                    card.style.opacity = 0f;
                    toastRegion.Add(card);

                    toastRegion.style.display = DisplayStyle.Flex;
                    
                    await card.FadeToAsync(1f, DefaultFadeSec);
                    await UniTask.Delay(TimeSpan.FromSeconds(item.Seconds));
                    await card.FadeToAsync(0f, DefaultFadeSec);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f)); //buffer

                    toastRegion.Clear();
                    toastRegion.style.display = DisplayStyle.None;
                }
            }
            finally
            {
                _toastBusy = false;
            }
        }
    }

    public sealed class WindowHandle
    {
        public IPresenter Presenter { get; private set; }
        public WindowOptions Options { get; private set; }
        private readonly UIManager _uiManager;

        public WindowHandle(IPresenter presenter, WindowOptions options, UIManager mgr)
        {
            Presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _uiManager = mgr;
        }

        public void Close(float? fadeOutSec = null)
        {
            if (Presenter == null) return;
            _uiManager.HideAsync(Presenter.GetType(), fadeOutSec).Forget();
        }
    }

    public class WindowOptions
    {
        public GameObject TargetGameObject = null;
        public UIDocument UIDoc = null;
        public StyleSheet StyleSheet = null;
        public VisualElement TargetContainer = null; // optional hard override (normally we use Region)
        public RequiredPermissions Permissions = null;

        public bool Modal = false; //modal is ephemeral but does not auto-hide on glass click 
        public bool CenterScreen = true;

        public Vector2? Size = null;
        public Vector2? Position = null;
        
        public UIProjectContext.UIRegion? Region = null;
        public VisualElement Anchor = null;
        public AnchorCorner AnchorCorner = AnchorCorner.BottomRight;
        public AnchorCorner PopupCorner = AnchorCorner.TopRight;
        public Vector2 AnchorOffset = Vector2.zero;

        public float? FadeInSec = null;
        public float? FadeOutSec = null;

        /// <summary>
        /// if true, rebuild UI even if presenter exists
        /// </summary>
        public bool ForceRebuild = false;
        /// <summary>
        /// clear previous and target containers before rebuild.
        /// Keep false if you want to preserve existing content in target container,
        /// or animate out old content before showing new.
        /// </summary>
        public bool ClearOldAndNewContainers = true;
        public bool CloseOthersInRegion = false;
    }
}