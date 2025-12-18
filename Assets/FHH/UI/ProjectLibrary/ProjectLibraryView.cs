using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FHH.Logic;
using Foxbyte.Core.Localization.Utilities;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.ProjectLibrary
{
    public sealed class ProjectLibraryView : ViewBase<ProjectLibraryPresenter>
    {
        public event Action<ProjectLibraryTab> OnTabRequested;
        public event Action<ProjectLibraryItem> OnDownloadRequested;
        public event Action<ProjectLibraryItem> OnLoadRequested;

        private VisualElement _root; // ProjectLibraryRoot
        private Button _mineBtn; // "Meine Projekte"
        private Button _sharedBtn; // "Mit mir geteilte"
        private VisualElement _grid;// card grid container
        private ScrollView _scroll;
        private readonly HashSet<string> _loaded = new();
        private readonly Dictionary<string, Button> _loadButtonsById = new();
        private readonly Dictionary<string, Button> _downloadButtonsById = new();
        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var gen = ViewGeneratorBase<ProjectLibraryPresenter, ProjectLibraryView>.Create<ProjectLibraryViewGenerator>(this);
            _root = await gen.GenerateViewAsync();

            _mineBtn   = _root.Q<Button>("ProjectLibrary_TabMine");
            _sharedBtn = _root.Q<Button>("ProjectLibrary_TabShared");
            _grid      = _root.Q<VisualElement>("ProjectLibrary_Grid");
            _scroll = _root.Q<ScrollView>("ProjectLibrary_Scroll");

            _mineBtn.clicked   += () => OnTabRequested?.Invoke(ProjectLibraryTab.Mine);
            _sharedBtn.clicked += () => OnTabRequested?.Invoke(ProjectLibraryTab.SharedWithMe);

            return _root;
        }

        public void RefreshGrid(
            IReadOnlyList<ProjectLibraryItem> items,
            HashSet<string> locallyAvailableProjectIds,
            bool isGerman)
        {
            if (_grid == null) return;

            _grid.Clear();
            _loadButtonsById.Clear();
            _downloadButtonsById.Clear();

            // build first, then localize, then apply dynamic download state
            if (items != null)
            {
                foreach (var item in items)
                {
                    var card = BuildCard(item);
                    _grid.Add(card);
                }
            }
            BindLocalizationFor(_grid);
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (locallyAvailableProjectIds != null &&
                        locallyAvailableProjectIds.Contains(item.Id))
                    {
                        SetDownloadButtonLocal(item.Id, isGerman);
                    }
                    else
                    {
                        SetDownloadButtonIdle(item.Id);
                    }
                }
            }
            if (_scroll != null) _scroll.scrollOffset = Vector2.zero; // back to top on refresh
        }


        private VisualElement BuildCard(ProjectLibraryItem item)
        {
            var card = new VisualElement { name = "ProjectLibrary_Card" };
            card.AddToClassList("pl-card");

            var title = new Label(item.Name) { name = "ProjectLibrary_Card_Title" };
            title.AddToClassList("pl-card-title");
            title.AddToClassList("i18n-skip");  // do not localize
            card.Add(title);

            var desc = new Label(item.Description) { name = "ProjectLibrary_Card_Description" };
            desc.AddToClassList("pl-card-desc");
            desc.AddToClassList("i18n-skip");
            desc.style.whiteSpace = WhiteSpace.Normal;
            card.Add(desc);

            var actions = new VisualElement { name = "ProjectLibrary_Card_Actions" };
            actions.AddToClassList("pl-card-actions");
            actions.AddToClassList("i18n-skip");
            card.Add(actions);

            var downloadBtn = new Button { name = "ProjectLibrary_Card_DownloadBtn", text = "Herunterladen" };
            downloadBtn.AddToClassList("btn");
            downloadBtn.AddToClassList("btn-download");
            downloadBtn.AddToClassList("i18n-skip");
            downloadBtn.text = ServiceLocator.GetService<LocaleSwitcher>().IsGerman() ? "Herunterladen" : "Download";
            downloadBtn.clicked += () => OnDownloadRequested?.Invoke(item);
            actions.Add(downloadBtn);
            _downloadButtonsById[item.Id] = downloadBtn;

            var loadBtn = new Button { name = "ProjectLibrary_Card_LoadBtn", text = "Laden" };
            loadBtn.AddToClassList("btn");
            loadBtn.AddToClassList("pl-btn-load");
            //ApplyArrowIcon(loadBtn);
            ApplyIcon(loadBtn, "Icons/arrow_forward");
            loadBtn.clicked += () => OnLoadRequested?.Invoke(item);
            actions.Add(loadBtn);
            
            _loadButtonsById[item.Id] = loadBtn;

            return card;
        }

        private void ApplyIcon(Button btn, string resourcesPath)
        {
            var tex = Resources.Load<Texture2D>(resourcesPath);
            var icon = btn.Q<VisualElement>("ProjectLibrary_Card_LoadBtn_Icon");
            if (icon == null)
            {
                icon = new VisualElement { name = "ProjectLibrary_Card_LoadBtn_Icon" };
                icon.AddToClassList("pl-icon");
                btn.Add(icon);
            }
            if (tex != null)
            {
                icon.style.backgroundImage = new StyleBackground(tex);
            }
        }

        private void ApplyArrowIcon(Button btn)
        {
            var tex = Resources.Load<Texture2D>("Icons/arrow_forward");
            if (tex == null) return;

            var icon = new VisualElement { name = "ProjectLibrary_Card_LoadBtn_Icon" };
            icon.AddToClassList("pl-icon");
            icon.style.backgroundImage = new StyleBackground(tex);
            btn.Add(icon);
        }

        /// <summary>
        /// Flip the specified card's button to Loaded/Unloaded state (text + style).
        /// </summary>
        public void SetLoadedStateFor(string projectId, bool isLoaded)
        {
            if (!_loadButtonsById.TryGetValue(projectId, out var btn)) return;
            
            if (isLoaded)
            {
                btn.AddToClassList("btn-danger");
                btn.AddToClassList("icon-only");
                ApplyIcon(btn, "Icons/close");       
                _loaded.Add(projectId);
            }
            else
            {
                btn.RemoveFromClassList("btn-danger");
                btn.RemoveFromClassList("icon-only");
                ApplyIcon(btn, "Icons/arrow_forward");
                _loaded.Remove(projectId); 
            }
        }

        /// <summary>
        /// Mark only the given project as loaded; reset all others.
        /// </summary>
        public void MarkOnlyThisAsLoaded(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                foreach (var kv in _loadButtonsById)
                {
                    SetLoadedStateFor(kv.Key, false);
                }
                return;
            }
            foreach (var kv in _loadButtonsById)
            {
                SetLoadedStateFor(kv.Key, kv.Key == projectId);
            }
        }

        public bool IsLoaded(string projectId)
        {
            return !string.IsNullOrWhiteSpace(projectId) && _loaded.Contains(projectId);
        }

        public void SetDownloadButtonInteractable(string projectId, bool interactable)
        {
            if (!_downloadButtonsById.TryGetValue(projectId, out var btn))
            {
                return;
            }
            btn.SetEnabled(interactable);
        }

        public void SetDownloadButtonIdle(string projectId)
        {
            if (!_downloadButtonsById.TryGetValue(projectId, out var btn))
            {
                return;
            }
            btn.text = "Herunterladen";
            btn.SetEnabled(true);
        }

        public void SetDownloadButtonLocal(string projectId, bool isGerman)
        {
            if (!_downloadButtonsById.TryGetValue(projectId, out var btn))
            {
                return;
            }
            btn.text = isGerman ? "Lokal entfernen" : "Remove locally";
            btn.SetEnabled(true);
        }

        public void SetDownloadButtonText(string projectId, string text)
        {
            if (!_downloadButtonsById.TryGetValue(projectId, out var btn))
            {
                return;
            }
            btn.text = text;
        }

        protected override void OnDestroy()
        {
            Presenter.OnViewDestroyed();
            base.OnDestroy();
            OnTabRequested -= OnTabRequested;
            OnDownloadRequested -= OnDownloadRequested;
            OnLoadRequested -= OnLoadRequested;
            _loadButtonsById.Clear();
            _loaded.Clear();
            _downloadButtonsById.Clear();
        }
    }
}