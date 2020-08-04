using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIUtils.GW2API
{
    public class GW2APITrait
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public int Specialization { get; set; }
        public int Tier { get; set; }
        public string Slot { get; set; }
        public List<GW2APIFact> Facts { get; set; }
        [JsonProperty(PropertyName = "traited_facts")]
        public List<GW2APITraitedFact> TraitedFacts { get; set; }
        public List<GW2APISkill> Skills { get; set; }
    }

}

