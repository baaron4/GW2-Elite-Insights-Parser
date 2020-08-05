using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIControllers.GW2API
{
    public class GW2APITrait
    {
        [JsonProperty]
        public long Id { get; internal set; }
        [JsonProperty]
        public string Name { get; internal set; }
        [JsonProperty]
        public string Icon { get; internal set; }
        [JsonProperty]
        public string Description { get; internal set; }
        [JsonProperty]
        public int Specialization { get; internal set; }
        [JsonProperty]
        public int Tier { get; internal set; }
        [JsonProperty]
        public string Slot { get; internal set; }
        [JsonProperty]
        public List<GW2APIFact> Facts { get; internal set; }
        [JsonProperty(PropertyName = "traited_facts")]
        public List<GW2APITraitedFact> TraitedFacts { get; internal set; }
        [JsonProperty]
        public List<GW2APISkill> Skills { get; internal set; }
    }

}

