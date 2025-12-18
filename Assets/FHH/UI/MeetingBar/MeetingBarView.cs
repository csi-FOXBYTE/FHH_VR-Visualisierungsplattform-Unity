using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine.UIElements;

namespace FHH.UI.MeetingBar
{
    public sealed class MeetingBarView : ViewBase<MeetingBarPresenter>
    {
        protected override string LocalizationTableName => "General";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        public Label TitleLabel { get; private set; }
        public Label SessionTimerLabel { get; private set; }
        public Button GuidedAccessButton { get; private set; }
        public Button ChatButton { get; private set; }
        public Button AdaptiveMicButton { get; private set; }
        public Button MicButton { get; private set; }
        public Button LeaveButton { get; private set; }

        //public Label StatusLabel { get; private set; }

        public VisualElement Root { get; private set; }

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var gen = ViewGeneratorBase<MeetingBarPresenter, MeetingBarView>.Create<MeetingBarViewGenerator>(this);
            Root = await gen.GenerateViewAsync();

            TitleLabel = Root.Q<Label>("ProjectTitle");
            SessionTimerLabel = Root.Q<Label>("SessionTimer");
            //StatusLabel = Root.Q<Label>("MeetingStatus");

            GuidedAccessButton = Root.Q<Button>("GuidedAccessButton");
            ChatButton = Root.Q<Button>("ChatButton");
            AdaptiveMicButton = Root.Q<Button>("AdaptiveMicButton");
            MicButton = Root.Q<Button>("MicButton");
            LeaveButton = Root.Q<Button>("MeetingBar_LeaveButton");

            return Root;
        }
    }
}
