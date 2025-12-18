using System;
using System.Collections.Generic;

namespace FHH.Logic.Components.Collaboration
{
    public sealed class MeetingEvent
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; } 
        public DateTime? EndTime { get; set; }
        public string Title { get; set; }
        //public string JoinCode { get; set; }
        //public string Role { get; set; }
        public string ProjectId { get; set; }
        public MeetingOwner Owner { get; set; }
        public List<MeetingModerator> Moderators { get; set; }
    }
}