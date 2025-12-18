//using System;
//using System.Collections.Generic;

//namespace FHH.Logic.Models
//{
//    public class Events
//    {
//        public DateTime? EndTime { get; set; }
//        public DateTime? StartTime { get; set; }
//        public string Title { get; set; }
//        public HashSet<string> Attendees { get; set; } = new();
//        public HashSet<string> Moderators { get; set; } = new();
//        public string Project { get; set; }

//        public bool IsAttendee(string email) => !string.IsNullOrWhiteSpace(email) && Attendees.Contains(email);
//        public bool IsModerator(string email) => !string.IsNullOrWhiteSpace(email) && Moderators.Contains(email);
//        public bool IsParticipant(string email) => IsAttendee(email) || IsModerator(email);
//    }
//}