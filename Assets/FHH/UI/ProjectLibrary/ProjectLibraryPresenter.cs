using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Models;
using Foxbyte.Core;
using Foxbyte.Core.Localization.Utilities;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.ProjectLibrary
{
    public sealed class ProjectLibraryPresenter
        : PresenterBase<ProjectLibraryPresenter, ProjectLibraryView, ProjectLibraryModel>
    {
        public ProjectLibraryPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            ProjectLibraryModel model = null,
            RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model ?? new ProjectLibraryModel(), perms)
        {
        }

        private bool _eventsHooked = false;
        private bool _isToggling = false; 
        private readonly Dictionary<string, CancellationTokenSource> _downloadAnimationTokens = new();
        private bool _isGerman;
        private string _shownLoadedProjectId;

        public override async UniTask InitializeBeforeUiAsync()
        {
            // create model here for convenience instead of passing it in
            if (Model == null)
            {
                Model = new ProjectLibraryModel();
                Model.AttachCancellationToken(Token);
            }
            await Model.InitializeAsync();
            await Model.LoadDataAsync();

            LayerManager.Instance.OnProjectUnloaded -= OnProjectUnloaded;
            LayerManager.Instance.OnProjectUnloaded += OnProjectUnloaded;

            LayerManager.Instance.OnProjectChanged -= OnProjectChanged;
            LayerManager.Instance.OnProjectChanged += OnProjectChanged;
        }

        private void OnProjectChanged(Project project)
        {
            SyncLoadedButtonFromLayerAsync(project?.Id).Forget();
        }

        private void OnProjectUnloaded()
        {
            SyncLoadedButtonFromLayerAsync(null).Forget();
        }
        
        private async UniTaskVoid SyncLoadedButtonFromLayerAsync(string newActiveProjectId)
        {
            await UniTask.SwitchToMainThread();
            await UniTask.WaitForEndOfFrame();

            if (View?.RootOfThisView == null) return;

            if (!string.IsNullOrWhiteSpace(_shownLoadedProjectId))
            {
                View.SetLoadedStateFor(_shownLoadedProjectId, isLoaded: false);
            }

            _shownLoadedProjectId = string.IsNullOrWhiteSpace(newActiveProjectId) ? null : newActiveProjectId;

            if (!string.IsNullOrWhiteSpace(_shownLoadedProjectId))
            {
                View.SetLoadedStateFor(_shownLoadedProjectId, isLoaded: true);
            }
        }

        private async UniTaskVoid ApplyActiveProjectToViewAsync(string activeProjectId)
        {
            await UniTask.SwitchToMainThread();
            await UniTask.WaitForEndOfFrame();

            if (View?.RootOfThisView == null) return;
            View.MarkOnlyThisAsLoaded(activeProjectId);
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await UniTask.Yield();
            var localeSwitcher = ServiceLocator.GetService<LocaleSwitcher>();
            _isGerman = localeSwitcher != null && localeSwitcher.IsTwoLetterLanguage("de");

            _shownLoadedProjectId = LayerManager.Instance.CurrentProject?.Id;
            await RefreshViewFromModelAsync();
            await UniTask.SwitchToMainThread();
            View.MarkOnlyThisAsLoaded(_shownLoadedProjectId);

            //if (!string.IsNullOrEmpty(LayerManager.Instance.CurrentProject?.Id))
            //{
            //    Model.SetActiveProject(LayerManager.Instance.CurrentProject?.Id);
            //}
            //else
            //{
            //    Model.ClearActiveProject();
            //}
            //await RefreshViewFromModelAsync();
            
            if (!_eventsHooked)
            {
                View.OnTabRequested += HandleTabRequested;
                View.OnDownloadRequested += OnDownloadRequested;
                View.OnLoadRequested += OnLoadRequested;
                _eventsHooked = true;
            }
        }

        private void HandleTabRequested(ProjectLibraryTab tab) 
        {
            RunGuardedAsync(View.RootOfThisView, async (_ct) =>
            {
                await Model.LoadForTabAsync(tab);
                await RefreshViewFromModelAsync(); // re-applies active state
            }, minCooldown: .15, external: CancellationToken.None).Forget();
        }

        private async UniTask RefreshViewFromModelAsync()
        {
            HashSet<string> locallyAvailable = null;

            if (Model.Current != null && Model.Downloader != null)
            {
                locallyAvailable = new HashSet<string>();
                foreach (var item in Model.Current)
                {
                    if (Model.Downloader.IsProjectCached(item.Id))
                    {
                        locallyAvailable.Add(item.Id);
                    }
                }
            }

            View.RefreshGrid(Model.Current, locallyAvailable, _isGerman);
            View.MarkOnlyThisAsLoaded(Model.ActiveProjectId);

            await UniTask.Yield();
        }

        public override async UniTask OnModelReplacedAsync(object initData)
        {
            await base.OnModelReplacedAsync(initData); // runs Initialize/Load
            await RefreshViewFromModelAsync();  // update UI
        }

        private void OnDownloadRequested(ProjectLibraryItem item)
        {
            DownloadOrRemoveProjectAsync(item).Forget();
        }

        private void OnLoadRequested(ProjectLibraryItem item)
        {
            if (_isToggling) return;
            _isToggling = true; 
            ToggleWrapperAsync(item).Forget();
        }

        private async UniTaskVoid ToggleWrapperAsync(ProjectLibraryItem item)
        {
            try
            {
                await ToggleProjectAsync(item);
            }
            finally
            {
                _isToggling = false;
            }
        }

        private async UniTask ToggleProjectAsync(ProjectLibraryItem item)
        {
            // If this card's project is currently active => unload
            if (!string.IsNullOrWhiteSpace(Model.ActiveProjectId) && Model.ActiveProjectId == item.Id)
            {
                await LayerManager.Instance.UnloadProjectAsync();
                Model.ClearActiveProject();
                View.SetLoadedStateFor(item.Id, isLoaded: false);
                return;
            }

            // Otherwise load the selected project (preferring local if available)
            await GetProjectAsync(item.Id);
            if (Model.ActiveProjectId == item.Id)
            {
                View.MarkOnlyThisAsLoaded(item.Id);
            }
        }

        private async UniTask GetProjectAsync(string id)
        {
            Project project = null;

            if (Model.Downloader != null && Model.Downloader.IsProjectCached(id))
            {
                project = await Model.Downloader.LoadProjectAsync(id, Token);
            }
            else
            {
                project = await Model.GetProjectAsync(id);
            }

            if (project != null)
            {
                await LayerManager.Instance.SetProjectAsync(project);
                Model.SetActiveProject(id);
                View.SetLoadedStateFor(id, isLoaded: true);
            }
            else
            {
                if (Model.ActiveProjectId == id)
                {
                    Model.ClearActiveProject();
                    View.SetLoadedStateFor(id, isLoaded: false);
                }
            }
        }

        public void OnViewDestroyed()
        {
            if (!_eventsHooked)
            {
                return;
            }
            LayerManager.Instance.OnProjectUnloaded -= OnProjectUnloaded;
            LayerManager.Instance.OnProjectChanged -= OnProjectChanged;
            if (View != null)
            {
                View.OnTabRequested -= HandleTabRequested;
                View.OnDownloadRequested -= OnDownloadRequested;
                View.OnLoadRequested -= OnLoadRequested;
            }
            _eventsHooked = false;
        }

        private async UniTaskVoid DownloadOrRemoveProjectAsync(ProjectLibraryItem item)
        {
            if (Model.Downloader == null)
            {
                ULog.Warning("ProjectDownloader is not initialized.");
                return;
            }

            var projectId = item.Id;

            // If already cached, this acts as "Remove locally"
            if (Model.Downloader.IsProjectCached(projectId))
            {
                View.SetDownloadButtonInteractable(projectId, false);
                try
                {
                    await Model.Downloader.DeleteProjectAsync(projectId, Token);
                    View.SetDownloadButtonIdle(projectId);
                }
                catch (Exception ex)
                {
                    ULog.Error($"Failed to delete local project {projectId}: {ex.Message}");
                    View.SetDownloadButtonIdle(projectId);
                }
                finally
                {
                    View.SetDownloadButtonInteractable(projectId, true);
                }

                return;
            }

            // Avoid double-starting for same project
            if (_downloadAnimationTokens.ContainsKey(projectId))
            {
                return;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
            _downloadAnimationTokens[projectId] = cts;

            View.SetDownloadButtonInteractable(projectId, false);

            RunDownloadAnimationAsync(projectId, cts.Token).Forget();

            try
            {
                var (project, json) = await Model.GetProjectWithJsonAsync(projectId);
                if (project == null || string.IsNullOrWhiteSpace(json))
                {
                    ULog.Warning($"Project {projectId} could not be downloaded (null or empty).");
                    View.SetDownloadButtonIdle(projectId);
                    return;
                }

                var success = await Model.Downloader.SaveProjectAsync(project, json, cts.Token);
                if (!success)
                {
                    View.SetDownloadButtonIdle(projectId);
                    return;
                }

                View.SetDownloadButtonLocal(projectId, _isGerman);
            }
            catch (OperationCanceledException)
            {
                View.SetDownloadButtonIdle(projectId);
            }
            catch (Exception ex)
            {
                ULog.Error($"Downloading project {projectId} failed: {ex.Message}");
                View.SetDownloadButtonIdle(projectId);
            }
            finally
            {
                if (_downloadAnimationTokens.TryGetValue(projectId, out var existingCts))
                {
                    existingCts.Cancel();
                    existingCts.Dispose();
                    _downloadAnimationTokens.Remove(projectId);
                }

                View.SetDownloadButtonInteractable(projectId, true);
            }
        }

        private async UniTask RunDownloadAnimationAsync(string projectId, CancellationToken token)
        {
            var count = 0;
            while (!token.IsCancellationRequested)
            {
                count++;
                if (count > 20)
                {
                    count = 1;
                }

                var text = new string('I', count);
                View.SetDownloadButtonText(projectId, text);

                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}