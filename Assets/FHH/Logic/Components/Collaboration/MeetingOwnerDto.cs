using Newtonsoft.Json;

namespace FHH.Logic.Components.Collaboration
{
    internal sealed class MeetingOwnerDto
    {
        [JsonProperty("name")] public string name { get; set; }
        [JsonProperty("email")] public string email { get; set; }
        [JsonProperty("id")] public string id { get; set; }
    }
}