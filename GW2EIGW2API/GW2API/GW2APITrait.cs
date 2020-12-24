using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIGW2API.GW2API
{
    public class GW2APITrait : GW2APIBaseItem
    {
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
        public IReadOnlyList<GW2APIFact> Facts { get; internal set; }
        [JsonProperty(PropertyName = "traited_facts")]
        public IReadOnlyList<GW2APITraitedFact> TraitedFacts { get; internal set; }
        [JsonProperty]
        public IReadOnlyList<GW2APISkill> Skills { get; internal set; }
    }

}

