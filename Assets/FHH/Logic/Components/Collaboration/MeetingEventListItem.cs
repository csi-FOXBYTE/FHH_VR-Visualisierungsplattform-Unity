using System.Collections.Generic;
using Newtonsoft.Json;

namespace FHH.Logic.Components.Collaboration
{
    /// <summary>
    /// DTO class for event list items. N
    /// No join code or project ID in response.
    /// </summary>
    internal sealed class MeetingEventListItem
    {
        [JsonProperty("id")] public string id { get; set; }
        [JsonProperty("status")] public string status { get; set; }
        [JsonProperty("endTime")] public string endTime { get; set; }
        [JsonProperty("startTime")] public string startTime { get; set; }
        [JsonProperty("title")] public string title { get; set; }
        [JsonProperty("owner")] public MeetingOwnerDto owner { get; set; }
        [JsonProperty("projectId")] public string projectId { get; set; }
        [JsonProperty("moderators")] public List<MeetingModeratorDto> moderators { get; set; }
    }
}