using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIUtils.GW2API
{
    public class GW2APISkill
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        [JsonProperty(PropertyName = "chat_link")]
        public string ChatLink { get; set; }
        public string Type { get; set; }
        [JsonProperty(PropertyName = "weapon_type")]
        public string WeaponType { get; set; }
        public List<string> Professions { get; set; }
        public string Slot { get; set; }
        public List<GW2APIFact> Facts { get; set; }
        [JsonProperty(PropertyName = "traited_facts")]
        public List<GW2APITraitedFact> TraitedFacts { get; set; }
        public List<string> Categories { get; set; }
        public string Attunement { get; set; }
        public int Cost { get; set; }
        [JsonProperty(PropertyName = "dual_wield")]
        public string DualWield { get; set; }
        [JsonProperty(PropertyName = "flip_skill")]
        public int FlipSkill { get; set; }
        public int Initiative { get; set; }
        [JsonProperty(PropertyName = "next_chain")]
        public int NextChain { get; set; }
        [JsonProperty(PropertyName = "prev_chain")]
        public int PrevChain { get; set; }
        [JsonProperty(PropertyName = "transform_skills")]
        public List<int> TransformSkills { get; set; }
        [JsonProperty(PropertyName = "bundle_skills")]
        public List<int> BundleSkills { get; set; }

        [JsonProperty(PropertyName = "toolbelt_skill")]
        public int ToolbeltSkill { get; set; }
    }

}

