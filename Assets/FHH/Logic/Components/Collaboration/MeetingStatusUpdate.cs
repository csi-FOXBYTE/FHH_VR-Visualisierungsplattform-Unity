using Newtonsoft.Json;

namespace FHH.Logic.Components.Collaboration
{
    internal sealed class MeetingStatusUpdate
    {
        [JsonProperty("id")] public string id { get; set; }
        [JsonProperty("joinCode")] public string joinCode { get; set; }
        [JsonProperty("status")] public string status { get; set; } // PLANNED, ACTIVE, MISSING_HOST, END, CANCELED
        [JsonProperty("endTime")] public string endTime { get; set; }
        [JsonProperty("startTime")] public string startTime { get; set; }
        [JsonProperty("title")] public string title { get; set; }
        [JsonProperty("role")] public string role { get; set; } // "attendee" or "moderator"
        [JsonProperty("projectId")] public string projectId { get; set; }
        [JsonProperty("owner")] public MeetingOwnerDto owner { get; set; }
    }
}