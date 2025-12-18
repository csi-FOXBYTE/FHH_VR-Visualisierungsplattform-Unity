using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.UI;
using FHH.UI.ProjectLibrary;
using Foxbyte.Presentation;
using Foxbyte.Presentation.Extensions;
using System;
using FHH.UI.Collaboration;
using FHH.UI.Help;
using FHH.UI.Settings;
using FHH.UI.Topics;
using Foxbyte.Core.Services.Permission;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.MainMenu
{
    public sealed class MainMenuPresenter
        : PresenterBase<MainMenuPresenter, MainMenuView, MainMenuModel>
    {
        private VisualElement _sidebar; // "MainMenuSidebar"
        private VisualElement _content; // "MainMenuContent"
        private Button _currentSelection;
        private UIManager _uiManager;
        private IPresenter _lastPresenter;
        private WindowHandle _lastHandle;
        private bool _isAnonymous;

        public MainMenuPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            MainMenuModel model = null,
            Foxbyte.Core.Services.Permission.RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model, perms)
        {
        }

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            if (Model == null) Model = new MainMenuModel();
            await Model.InitializeAsync();
            _uiManager = ServiceLocator.GetService<UIManager>();
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            _isAnonymous = ServiceLocator.GetService<PermissionService>().IsAnonymous();
            if (_isAnonymous)
            {
                Model.InitialSection = Model.InitialSection == MainMenuSection.Help ? MainMenuSection.Help : MainMenuSection.Topics;
                //View.RootOfThisView.
                // query the "MainMenu_Button_Library") and hide it
                var libBtn = _sidebar?.Q<Button>("MainMenu_Button_Library");
                if (libBtn != null) libBtn.style.display = DisplayStyle.None;
                var meetingBtn = _sidebar?.Q<Button>("MainMenu_Button_Meetings");
                if (meetingBtn != null) meetingBtn.style.display = DisplayStyle.None;
            }
            await UniTask.Yield();
            //var first = _sidebar?.Q<Button>("MainMenu_Button_Library");
            //SelectButton(first);
            //OnSelectProjectLibrary();
            SelectSectionAsync(Model.InitialSection, forceRecreate: true).Forget();
        }

        // Called by ViewGenerator once containers exist
        public void AttachContainers(VisualElement sidebar, VisualElement content)
        {
            _sidebar  = sidebar ?? throw new ArgumentNullException(nameof(sidebar));
            _content  = content ?? throw new ArgumentNullException(nameof(content));
        }

        public async UniTask SelectSectionAsync(MainMenuSection section, bool forceRecreate = false)
        {
            switch (section)
            {
                case MainMenuSection.Library:   
                    OnSelectProjectLibrary(forceRecreate); 
                    break;
                case MainMenuSection.Meetings:  
                    OnSelectMeetings(forceRecreate); 
                    break;
                case MainMenuSection.Topics:    
                    OnSelectTopics(forceRecreate); 
                    break;
                case MainMenuSection.Settings:  
                    OnSelectSettings(forceRecreate); 
                    break;
                case MainMenuSection.Help:      
                    OnSelectHelp(forceRecreate); 
                    break;
                //case MainMenuSection.Search:    
                //    OnSelectSearch(forceRecreate); 
                //    break;
                default:
                    OnSelectProjectLibrary(forceRecreate);
                    break;
            }

            await UniTask.CompletedTask;
        }

        private bool ShouldRebuild<TPresenter>(bool forceRecreate)
        {
            if (forceRecreate) return true;
            if (_content == null) return true;
            if (_content.childCount == 0) return true;
            if (_lastPresenter is not TPresenter) return true;
            return false;
        }

        private void SelectButton(Button btn)
        {
            _currentSelection?.RemoveFromClassList("menu-item--active");
            _currentSelection = btn;
            _currentSelection?.AddToClassList("menu-item--active");
        }

        
        // Sidebar actions (left)


        // Project Library
        public void OnSelectProjectLibrary(bool forceRecreate = false)
        {
            if (!ShouldRebuild<ProjectLibraryPresenter>(forceRecreate))
                return;

            if (_lastPresenter != null && _lastPresenter is not ProjectLibraryPresenter)
                _lastHandle.Close();

            ShowProjectLibraryAsync().Forget();
            //if (_lastPresenter is ProjectLibraryPresenter) return;
            //if (_lastPresenter != null && _lastPresenter is not ProjectLibraryPresenter)
            //    _lastHandle.Close();
            //ShowProjectLibraryAsync().Forget();
        }

        private async UniTask ShowProjectLibraryAsync()
        {
            
            var btn = _sidebar.Q<Button>("MainMenu_Button_Library");
            SelectButton(btn);

            var opts = new WindowOptions
            {
                TargetContainer = _content,
                CenterScreen = false,
                FadeInSec = 0.15f,
                FadeOutSec = 0.15f,
                StyleSheet = Resources.Load<StyleSheet>("ProjectLibrary"),
                ForceRebuild = true,
                ClearOldAndNewContainers = true
            };
            // model will be created on initialization of the project library presenter
            _lastHandle = await _uiManager.ShowWindowAsync<ProjectLibraryPresenter, ProjectLibraryView, ProjectLibraryModel>(
                null, opts, null, null, this.Token);

            _lastPresenter = _lastHandle.Presenter;
        }
        
        // Meetings / Collaboration

        public void OnSelectMeetings(bool forceRecreate = false)
        {
            if (!ShouldRebuild<CollaborationPresenter>(forceRecreate)) return;
            if (_lastPresenter != null && _lastPresenter is not CollaborationPresenter) _lastHandle.Close();
            ShowMeetingsAsync().Forget();
        }

        private async UniTask ShowMeetingsAsync()
        {
            var btn = _sidebar.Q<Button>("MainMenu_Button_Meetings");
            SelectButton(btn);

            var opts = new WindowOptions
            {
                TargetContainer = _content,
                CenterScreen = false,
                FadeInSec = 0.15f,
                FadeOutSec = 0.15f,
                StyleSheet = Resources.Load<StyleSheet>("Collaboration"), // stylesheet name is internal
                ForceRebuild = true,
                ClearOldAndNewContainers = true
            };

            _lastHandle = await _uiManager.ShowWindowAsync<CollaborationPresenter, CollaborationView, CollaborationModel>(
                null, opts, null, null, this.Token);

            _lastPresenter = _lastHandle.Presenter;
        }

        // Topics

        public void OnSelectTopics(bool forceRecreate = false)
        {
            if (!ShouldRebuild<TopicsPresenter>(forceRecreate)) return;
            if (_lastPresenter != null && _lastPresenter is not TopicsPresenter) _lastHandle.Close();
            ShowTopicsAsync().Forget();
        }

        private async UniTask ShowTopicsAsync()
        {
            var btn = _sidebar.Q<Button>("MainMenu_Button_Topics");
            SelectButton(btn);

            var opts = new WindowOptions
            {
                TargetContainer = _content,
                CenterScreen = false,
                FadeInSec = 0.15f,
                FadeOutSec = 0.15f,
                StyleSheet = Resources.Load<StyleSheet>("Topics"),
                ForceRebuild = true,
                ClearOldAndNewContainers = true
            };

            _lastHandle = await _uiManager.ShowWindowAsync<TopicsPresenter, TopicsView, TopicsModel>(
                null, opts, null, null, this.Token);

            _lastPresenter = _lastHandle.Presenter;
        }

        // Settings

        public void OnSelectSettings(bool forceRecreate = false)
        {
            if (!ShouldRebuild<SettingsPresenter>(forceRecreate)) return;
            if (_lastPresenter != null && _lastPresenter is not SettingsPresenter) _lastHandle.Close();
            ShowSettingsAsync().Forget();
        }

        private async UniTask ShowSettingsAsync()
        {
            var btn = _sidebar.Q<Button>("MainMenu_Button_Settings");
            SelectButton(btn);

            var opts = new WindowOptions
            {
                TargetContainer = _content,
                CenterScreen = false,
                FadeInSec = 0.15f,
                FadeOutSec = 0.15f,
                StyleSheet = Resources.Load<StyleSheet>("Settings"),
                ForceRebuild = true,
                ClearOldAndNewContainers = true
            };

            _lastHandle = await _uiManager.ShowWindowAsync<SettingsPresenter, SettingsView, SettingsModel>(
                null, opts, null, null, this.Token);

            _lastPresenter = _lastHandle.Presenter;
        }

        // Help

        public void OnSelectHelp(bool forceRecreate = false)
        {
            if (!ShouldRebuild<HelpPresenter>(forceRecreate)) return;
            if (_lastPresenter != null && _lastPresenter is not HelpPresenter) _lastHandle.Close();
            ShowHelpAsync().Forget();
        }

        private async UniTask ShowHelpAsync()
        {
            var btn = _sidebar.Q<Button>("MainMenu_Button_Help");
            SelectButton(btn);

            var opts = new WindowOptions
            {
                TargetContainer = _content,
                CenterScreen = false,
                FadeInSec = 0.15f,
                FadeOutSec = 0.15f,
                StyleSheet = Resources.Load<StyleSheet>("Help"),
                ForceRebuild = true,
                ClearOldAndNewContainers = true
            };

            _lastHandle = await _uiManager.ShowWindowAsync<HelpPresenter, HelpView, HelpModel>(
                null, opts, null, null, this.Token);

            _lastPresenter = _lastHandle.Presenter;
        }
        

        // Helpers

        private void ResetCachedContentState()
        {
            if (_lastHandle != null)
                try { _lastHandle.Close(); } catch { }

            _lastPresenter = null;
            _lastHandle = default;
        }

        // Called on view destroy
        public void OnMainMenuClosed()
        {
            ResetCachedContentState();
        }

        public void OnQuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
