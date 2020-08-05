using Newtonsoft.Json;

namespace GW2EIControllers.GW2API
{
    public class GW2APIFact
    {
        [JsonProperty]
        public string Text { get; internal set; }
        [JsonProperty]
        public string Icon { get; internal set; }
        [JsonProperty]
        public string Type { get; internal set; }
        [JsonProperty]
        public string Target { get; internal set; }
        [JsonProperty]
        public object Value { get; internal set; }
        [JsonProperty]
        public string Status { get; internal set; }
        [JsonProperty]
        public string Description { get; internal set; }
        [JsonProperty(PropertyName = "apply_count")]
        public int ApplyCount { get; internal set; }
        [JsonProperty]
        public float Duration { get; internal set; }
        [JsonProperty(PropertyName = "field_type")]
        public string FieldType { get; internal set; }
        [JsonProperty(PropertyName = "finisher_type")]
        public string FinisherType { get; internal set; }
        [JsonProperty]
        public float Percent { get; internal set; }
        [JsonProperty(PropertyName = "hit_count")]
        public int HitCount { get; internal set; }
        [JsonProperty(PropertyName = "dmg_multiplier")]
        public float DmgMultiplier { get; internal set; }
        [JsonProperty]
        public int Distance { get; internal set; }
        [JsonProperty]
        public GW2APIFact Prefix { get; internal set; }
    }
}

