using Newtonsoft.Json;

namespace GW2EIParser.Controllers.GW2API
{
    public class GW2APIFact
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public string Target { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        [JsonProperty(PropertyName = "apply_count")]
        public int ApplyCount { get; set; }
        public int Duration { get; set; }
        [JsonProperty(PropertyName = "field_type")]
        public string FieldType { get; set; }
        [JsonProperty(PropertyName = "finisher_type")]
        public string FinisherType { get; set; }
        public float Percent { get; set; }
        [JsonProperty(PropertyName = "hit_count")]
        public int HitCount { get; set; }
        [JsonProperty(PropertyName = "dmg_multiplier")]
        public float DmgMultiplier { get; set; }
        public int Distance { get; set; }
        public GW2APIFact Prefix { get; set; }

        public object Value { get; set; }
    }
}