using Newtonsoft.Json;

namespace GW2EIDPSReport.DPSReportJsons
{
    public class DPSReportUploadPlayerObject
    {
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; internal set; }
        [JsonProperty(PropertyName = "character_name")]
        public string CharacterName { get; internal set; }
        [JsonProperty]
        public int Profession { get; internal set; }
        [JsonProperty(PropertyName = "elite_spec")]
        public int EliteSpec { get; internal set; }
    }
}
