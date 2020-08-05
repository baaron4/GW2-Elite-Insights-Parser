using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIControllers.GW2API
{
    public class GW2APITrait
    {
        public long Id { get; internal set; }
        public string Name { get; internal set; }
        public string Icon { get; internal set; }
        public string Description { get; internal set; }
        public int Specialization { get; internal set; }
        public int Tier { get; internal set; }
        public string Slot { get; internal set; }
        public List<GW2APIFact> Facts { get; internal set; }
        [JsonProperty(PropertyName = "traited_facts")]
        public List<GW2APITraitedFact> TraitedFacts { get; internal set; }
        public List<GW2APISkill> Skills { get; internal set; }
    }

}

