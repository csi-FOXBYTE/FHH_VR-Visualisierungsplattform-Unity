using Cysharp.Threading.Tasks;
using FHH.Logic.Models;
using Foxbyte.Presentation;
using System.Collections.Generic;
using FHH.Logic;
using FHH.Logic.Components.Collaboration;

namespace FHH.UI.Collaboration
{
    public sealed class CollaborationModel : PresenterModelBase
    {
        public List<MeetingEvent> Meetings { get; private set; } = new();

        private CollaborationService _collaborationService;

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            _collaborationService = ServiceLocator.GetService<CollaborationService>();
        }

        public async UniTask<List<MeetingEvent>> LoadEventsAsync()
        {
            var events = await _collaborationService.GetMeetingEventsAsync(CancellationToken);
            return events;

            // mock
            //await UniTask.Delay(50, cancellationToken: CancellationToken);
            //var demo = new List<EventsDto>
            //{
            //    new EventsDto
            //    {
            //        title = "Bürgerbeteiligung",
            //        startTime = "2025-03-03T12:15:00Z",
            //        endTime = "2025-03-03T13:45:00Z",
            //        project = "Projekt Alsterufer",
            //        attendees = new [] {"alice@example.com","bob@example.com"},
            //        moderators = new [] {"moderator@example.com"}
            //    },
            //    new EventsDto
            //    {
            //        title = "Entwurfsrunde",
            //        startTime = "2025-03-05T09:00:00Z",
            //        endTime = "2025-03-05T10:00:00Z",
            //        project = "Rathausplatz",
            //        attendees = new [] {"you@example.com"},
            //        moderators = new [] {"lead@example.com"}
            //    }
            //};
            //return demo.Select(Map).ToList();
        }

        public override async UniTask LoadDataAsync()
        {
            Meetings = await LoadEventsAsync();
        }
    }
}
