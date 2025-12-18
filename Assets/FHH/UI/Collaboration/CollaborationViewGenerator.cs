using Cysharp.Threading.Tasks;
using FHH.Logic.Models;
using Foxbyte.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using FHH.Logic.Components.Collaboration;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Collaboration
{
    public sealed class CollaborationViewGenerator : ViewGeneratorBase<CollaborationPresenter, CollaborationView>
    {
        private VisualElement _root;
        private ScrollView _list;

        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            _root = new VisualElement { name = "CollaborationRoot" };
            _root.AddToClassList("collab-root");
            _root.AddToClassList("col");

            var titleRow = new VisualElement { name = "CollaborationHeader" };
            titleRow.AddToClassList("row");
            titleRow.AddToClassList("collab-header");

            //var title = new Label("Zusammenarbeit") { name = "CollaborationTitle" }; // i18n key
            //title.AddToClassList("collab-title");
            //titleRow.Add(title);

            _list = new ScrollView(ScrollViewMode.Vertical) { name = "CollaborationList" };
            _list.AddToClassList("collab-scroll");

            _root.Add(titleRow);
            _root.Add(_list);

            await UniTask.CompletedTask;
            return _root;
        }

        public void BuildCards(List<MeetingEvent> meetings)
        {
            _list.Clear();

            if (meetings == null || meetings.Count == 0)
            {
                var empty = new Label("Keine Termine") { name = "CollaborationEmpty" };
                empty.AddToClassList("collab-empty");
                _list.Add(empty);
                return;
            }

            var sortedMeetings = meetings
                .OrderBy(m => m.StartTime ?? DateTime.MaxValue)
                .ThenBy(m => m.Title)
                .ToList();

            for (int i = 0; i < sortedMeetings.Count; i++)
                _list.Add(CreateCard(sortedMeetings[i], i));
        }

        private VisualElement CreateCard(MeetingEvent ev, int index)
        {
            // Container
            var card = new VisualElement { name = $"MeetingCard_{index}" };
            card.AddToClassList("collab-card");
            card.AddToClassList("col");

            // Header row (date/time)
            var metaRow = new VisualElement { name = $"MeetingCard_Meta_{index}" };
            metaRow.AddToClassList("row");
            metaRow.AddToClassList("collab-meta");

            metaRow.Add(IconWithText("calendar", FormatDate(ev.StartTime), $"MeetingCard_Date_{index}"));
            //metaRow.Add(Spacer(12));
            metaRow.Add(IconWithText("time", $"{FormatTime(ev.StartTime)} - {FormatTime(ev.EndTime)}", $"MeetingCard_Time_{index}"));

            // Title
            var title = new Label(ev.Title ?? string.Empty) { name = $"MeetingCard_Title_{index}" };
            title.AddToClassList("collab-title2");

            // Project
            //if (!string.IsNullOrWhiteSpace(ev.ProjectId))
            //{
            //    var project = new Label(ev.ProjectId) { name = $"MeetingCard_Project_{index}" };
            //    project.AddToClassList("collab-project");
            //    card.Add(project);
            //}

            // CTA row
            var ctaRow = new VisualElement { name = $"MeetingCard_CtaRow_{index}" };
            ctaRow.AddToClassList("row");
            ctaRow.AddToClassList("collab-cta");

            var button = new Button { name = $"MeetingCard_StartButton_{index}", text = "Teilnehmen" };
            button.AddToClassList("btn");
            button.AddToClassList("btn-primary");
            button.AddToClassList("collab-start");

            var arrow = new VisualElement { name = $"MeetingCard_Arrow_{index}" };
            arrow.AddToClassList("collab-icon-arrow");
            arrow.style.backgroundImage = LoadIcon("arrow_forward");
            
            button.Add(arrow);
            metaRow.Add(button);

            //var btnWrap = new VisualElement { name = $"MeetingCard_StartWrap_{index}" };
            //btnWrap.AddToClassList("row");
            //btnWrap.AddToClassList("center");
            //btnWrap.AddToClassList("collab-startwrap");
            //btnWrap.Add(button);
            //btnWrap.Add(arrow);

            // Wire-up
            button.clicked += () => View.Presenter?.OnStartMeeting(ev);

            // Build
            card.Add(metaRow);
            card.Add(title);
            //card.Add(btnWrap);

            return card;
        }

        private VisualElement IconWithText(string iconFileNoExt, string text, string name)
        {
            var row = new VisualElement { name = name };
            row.AddToClassList("row");
            row.AddToClassList("center");

            var icon = new VisualElement { name = $"{name}_Icon" };
            icon.AddToClassList("collab-icon16");
            icon.style.backgroundImage = LoadIcon(iconFileNoExt);

            var label = new Label(text) { name = $"{name}_Label" };
            label.AddToClassList("collab-meta-text");

            row.Add(icon);
            row.Add(Spacer(6));
            row.Add(label);
            return row;
        }

        private StyleBackground LoadIcon(string fileNoExt)
        {
            // Resources/Icons/<file>.png
            var tex = Resources.Load<Texture2D>($"Icons/{fileNoExt}");
            return tex ? new StyleBackground(tex) : new StyleBackground();
        }

        private VisualElement Spacer(float width)
        {
            var s = new VisualElement();
            s.style.width = width;
            s.style.flexShrink = 0;
            return s;
        }

        private static string FormatDate(DateTime? dt)
        {
            if (dt == null) return string.Empty;
            // Monday, 03.03.2025 -> localized by table name if you prefer
            var d = dt.Value;
            return $"{d:dddd}, {d:dd.MM.yyyy}";
        }

        private static string FormatTime(DateTime? dt)
        {
            if (dt == null) return string.Empty;
            return dt.Value.ToString("HH:mm");
        }
    }
}
