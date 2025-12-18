using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using FHH.Logic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Topics
{
    public sealed class TopicsPresenter 
        : PresenterBase<TopicsPresenter, TopicsView, TopicsModel>
    {

        private bool _isReloadingTopics;

        public TopicsPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            TopicsModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model ?? new TopicsModel(), perms)
        { }

        public override async UniTask InitializeBeforeUiAsync()
        {
            await Model.InitializeAsync();
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await UniTask.CompletedTask;
            await Model.ReloadFromManagerAsync();
            RefreshLists();
            WireTabButtons();
            LayerManager.Instance.OnCombinedLayersChanged += OnCombinedLayersChangedHandler;
        }

        protected override async UniTask OnViewUnmountedAsync()
        {
            LayerManager.Instance.OnCombinedLayersChanged -= OnCombinedLayersChangedHandler;
            await UniTask.Yield();
        }

        private void WireTabButtons()
        {
            var basisBtn = View.RootOfThisView.Q<Button>("TopicsBasis");
            //var moreBtn  = View.RootOfThisView.Q<Button>("TopicsMore");

            basisBtn.clicked += () =>
            {
                if (!Allow(basisBtn)) return;
                Model.SwitchGroup(TopicsModel.TopicGroup.Basis);
                UpdateTabs();
                RefreshLists();
            };

            //moreBtn.clicked += () =>
            //{
            //    if (!Allow(moreBtn)) return;
            //    Model.SwitchGroup(TopicsModel.TopicGroup.More);
            //    UpdateTabs();
            //    RefreshLists();
            //};

            UpdateTabs();
        }

        private void UpdateTabs()
        {
            var basisBtn = View.RootOfThisView.Q<Button>("TopicsBasis");
            //var moreBtn  = View.RootOfThisView.Q<Button>("TopicsMore");

            basisBtn.RemoveFromClassList("topics-tab--active");
            //moreBtn.RemoveFromClassList("topics-tab--active");
            if (Model.CurrentGroup == TopicsModel.TopicGroup.Basis)
                basisBtn.AddToClassList("topics-tab--active");
            //else
            //    moreBtn.AddToClassList("topics-tab--active");
        }

        private void RefreshLists()
        {
            var lm = LayerManager.Instance;
            if (lm != null)
            {
                var combined = lm.GetCombinedBaseLayers();
                var restrictedCount = 0;
                foreach (var l in combined) if (l.IsRestricted) restrictedCount++;
                //Debug.Log($"Topics: combined={combined.Count}, restricted-in-combined={restrictedCount}, hasRestrictedLoaded={lm.HasRestrictedLayersLoaded}");
            }

            var (downloaded, available) = Model.GetActiveLists();
            //View.BindDownload(downloaded, OnInfoClicked, OnDeleteClicked, OnToggleChanged);
            //View.BindAvailable(available, OnInfoClicked, OnDownloadClicked, OnToggleChanged);
            View.BindDownload(downloaded, OnDeleteClicked, OnToggleChanged);
            View.BindAvailable(available, OnDownloadClicked, OnToggleChanged);
        }

        private async void OnToggleChanged(TopicsModel.TopicItem item, bool isVisible)
        {
            var lm = LayerManager.Instance;
            if (lm == null) return;

            var ok = await lm.ToggleBaseLayerVisibilityAsync(item.Id, isVisible);
            if (!ok) return;

            // Keep UI/model in sync with manager state
            await TopicsModelReloadAndRefreshAsync();
        }

        private async UniTask TopicsModelReloadAndRefreshAsync()
        {
            var lm = LayerManager.Instance;
            if (lm == null) return;

            if (_isReloadingTopics)
                return;

            _isReloadingTopics = true;
            try
            {
                await lm.RefreshDownloadedStatusAsync();
                await Model.ReloadFromManagerAsync();
                RefreshLists();
            }
            finally
            {
                _isReloadingTopics = false;
            }
        }

        private void OnInfoClicked(TopicsModel.TopicItem item)
        {
            Debug.Log($"Info: {item.Title} ({FormatSize(item.Bytes)})");
        }

        private void OnDownloadClicked(TopicsModel.TopicItem item, VisualElement container)
        {
            if (item.IsDownloading) return;
            OnDownloadAsync(item, container).Forget();
        }

        private async UniTask OnDownloadAsync(TopicsModel.TopicItem item, VisualElement container)
        {
            if (item.IsDownloading) return;

            var lm = LayerManager.Instance;
            if (lm == null)
            {
                Debug.LogWarning("OnDownloadClicked: LayerManager.Instance is null.");
                return;
            }

            item.IsDownloading = true;
            item.DownloadProgress = 0f;

            // show progress bar (looping animation)
            var bar = View.ShowProgressFor(container);
            
            // Link to presenter’s lifetime
            using var dlCts = CancellationTokenSource.CreateLinkedTokenSource(Token);

            // progress loop token (separate, just for UI animation)
            using var progressCts = new CancellationTokenSource();
            
            
            _ = UniTask.Create(async () =>
            {
                const float loopTime = 2f;
                float t = 0f;
                while (!progressCts.IsCancellationRequested && !Token.IsCancellationRequested)
                {
                    t += Time.deltaTime;
                    var phase = (t % loopTime) / loopTime;
                    item.DownloadProgress = phase;
                    View.UpdateProgress(bar, item.DownloadProgress);
                    await UniTask.Yield(PlayerLoopTiming.Update, Token);
                }
            });

            var success = await lm.DownloadLayerAsync(item.Id, dlCts.Token);

            progressCts.Cancel();
            item.IsDownloading = false;

            await lm.RefreshDownloadedStatusAsync();
            await Model.ReloadFromManagerAsync();
            RefreshLists();

            if (!success)
            {
                Debug.LogWarning($"Download failed for {item.Title} (id: {item.Id}).");
            }
        }

        private void OnDeleteClicked(TopicsModel.TopicItem item)
        {
            if (item.IsDownloading) return;
            DeleteAsync(item).Forget();
        }

        private async UniTask DeleteAsync(TopicsModel.TopicItem item)
        {
            var lm = LayerManager.Instance;
            if (lm == null) return;

            var ok = await lm.DeleteLayerAsync(item.Id);
            if (!ok) return;

            await lm.RefreshDownloadedStatusAsync();
            await Model.ReloadFromManagerAsync();
            RefreshLists();
        }
        

        internal static string FormatSize(long bytes)
        {
            double gb = bytes / (1024.0 * 1024.0 * 1024.0);
            return $"{gb:0.0} GB";
        }

        private void OnCombinedLayersChangedHandler(IReadOnlyList<BaseLayerCombined> _)
        {
            TopicsModelReloadAndRefreshAsync().Forget();
        }


        public override void Dispose()
        {
            LayerManager.Instance.OnCombinedLayersChanged -= OnCombinedLayersChangedHandler;
            base.Dispose();
        }
    }
}