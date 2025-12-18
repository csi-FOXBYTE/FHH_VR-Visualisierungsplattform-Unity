using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Components.Collaboration;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;
using FHH.Logic.Models;
using FHH.UI.MeetingBar;

namespace FHH.UI.Collaboration
{
    public sealed class CollaborationPresenter
        : PresenterBase<CollaborationPresenter, CollaborationView, CollaborationModel>
    {
        public CollaborationPresenter(
            GameObject targetGameobjectForView,
            UIDocument uidoc,
            StyleSheet styleSheet,
            VisualElement target,
            CollaborationModel model = null,
            Foxbyte.Core.Services.Permission.RequiredPermissions perms = null)
            : base(targetGameobjectForView, uidoc, styleSheet, target, model ?? new CollaborationModel(), perms)
        { }

        private CollaborationService _collaborationService;

        public override async UniTask InitializeBeforeUiAsync()
        {
            await base.InitializeBeforeUiAsync();
            _collaborationService = ServiceLocator.GetService<CollaborationService>();
            await Model.InitializeAsync();
        }

        public override async UniTask InitializeAfterUiAsync()
        {
            await Model.LoadDataAsync(); // fetch events
            View.RebuildList(Model.Meetings);
        }

        public void OnStartMeeting(MeetingEvent meeting)
        {
            Debug.Log($"Start meeting: {meeting?.Title} [{meeting?.StartTime} - {meeting?.EndTime}]");
            StartMeetingAsync(meeting).Forget();
        }

        private async UniTask StartMeetingAsync(MeetingEvent meeting)
        {
            ShowMeetingBarAsync(meeting).Forget();
            await UniTask.Yield();
            //await _collaborationService.CollaborateAsync(meeting, MountToken); // pass View's token for cancellation on close
        }

        // Optional: public API to refresh from outside
        public async UniTask RefreshAsync()
        {
            await Model.LoadDataAsync();
            View.RebuildList(Model.Meetings);
        }

        private async UniTask ShowMeetingBarAsync(MeetingEvent meeting)
        {
            var uiManager = ServiceLocator.GetService<UIManager>();

            // check if the MeetingBar is already open
            if (uiManager.IsShown<MeetingBarPresenter>())
            {
                Debug.Log("MeetingBar is already open.");
                return;
            }

            var uiDoc = View.UIDocOfThisView;
            //var leftSlot = uiDoc.rootVisualElement.Q<VisualElement>("MenuBar_Left");
            uiManager.ShowWindowAsync<MeetingBarPresenter, MeetingBarView, MeetingBarModel>(
                model: new MeetingBarModel(),
                options: new WindowOptions
                {
                    //TargetContainer = leftSlot,
                    //TargetContainer = UiDocument.rootVisualElement,
                    //Position = new Vector2(350, 10),
                    Region = UIProjectContext.UIRegion.Toolbar1,
                    CenterScreen = false,
                    StyleSheet = Resources.Load<StyleSheet>("MeetingBar"),
                },
                initData:meeting
            ).Forget();
            await UniTask.Yield();
        }
    }
}