using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.MeetingBar
{
    public sealed class MeetingBarViewGenerator : ViewGeneratorBase<MeetingBarPresenter, MeetingBarView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            var root = new VisualElement { name = "MeetingBarRoot" };
            root.AddToClassList("meetingbar-root");
            root.AddToClassList("row");
            root.AddToClassList("center");

            var title = new Label { name = "ProjectTitle" };
            title.AddToClassList("meetingbar-title");
            title.AddToClassList("i18n-skip");
            root.Add(title);

            var spacer = new VisualElement { name = "MeetingBarSpacer" };
            spacer.AddToClassList("meetingbar-spacer");
            root.Add(spacer);

            var timer = new Label { name = "SessionTimer" };
            timer.AddToClassList("meetingbar-timer");
            timer.AddToClassList("i18n-skip");
            root.Add(timer);

            //var status = new Label { name = "MeetingStatus" };
            //status.AddToClassList("meetingbar-status");
            //status.AddToClassList("i18n-skip");
            //root.Add(status);

            root.Add(MakeIconButton("GuidedAccessButton", "Icons/MeetingBar/guided_access"));
            root.Add(MakeIconButton("ChatButton", "Icons/MeetingBar/chat"));
            root.Add(MakeIconButton("AdaptiveMicButton", "Icons/MeetingBar/adaptive_audio_mic"));
            root.Add(MakeIconButton("MicButton", "Icons/MeetingBar/mic"));

            var leave = new Button { name = "MeetingBar_LeaveButton" };
            leave.AddToClassList("btn");
            leave.AddToClassList("btn-primary");
            leave.AddToClassList("meetingbar-leave");
            root.Add(leave);

            await UniTask.CompletedTask;
            return root;
        }

        private Button MakeIconButton(string name, string resourcePathNoExt)
        {
            var b = new Button { name = name };
            b.AddToClassList("meetingbar-icon-btn");
            b.focusable = false;
            b.AddToClassList("i18n-skip");

            var img = new Image { name = $"{name}_Img" };
            img.scaleMode = ScaleMode.ScaleToFit;
            img.AddToClassList("meetingbar-icon");
            var tex = Resources.Load<Texture2D>(resourcePathNoExt);
            if (tex != null) img.image = tex;

            b.Add(img);
            return b;
        }
    }
}
