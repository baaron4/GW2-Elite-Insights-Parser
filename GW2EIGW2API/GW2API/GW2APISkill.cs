using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIGW2API.GW2API
{
    public class GW2APISkill : GW2APIBaseItem
    {
        [JsonProperty]
        public string Name { get; internal set; }
        [JsonProperty]
        public string Description { get; internal set; }
        [JsonProperty]
        public string Icon { get; internal set; }
        [JsonProperty(PropertyName = "chat_link")]
        public string ChatLink { get; internal set; }
        [JsonProperty]
        public string Type { get; internal set; }
        [JsonProperty(PropertyName = "weapon_type")]
        public string WeaponType { get; internal set; }
        [JsonProperty]
        public IReadOnlyList<string> Professions { get; internal set; }
        [JsonProperty]
        public string Slot { get; internal set; }
        [JsonProperty]
        public IReadOnlyList<GW2APIFact> Facts { get; internal set; }
        [JsonProperty(PropertyName = "traited_facts")]
        public IReadOnlyList<GW2APITraitedFact> TraitedFacts { get; internal set; }
        [JsonProperty]
        public IReadOnlyList<string> Categories { get; internal set; }
        [JsonProperty]
        public string Attunement { get; internal set; }
        [JsonProperty]
        public int Cost { get; internal set; }
        [JsonProperty(PropertyName = "dual_wield")]
        public string DualWield { get; internal set; }
        [JsonProperty(PropertyName = "flip_skill")]
        public int FlipSkill { get; internal set; }
        [JsonProperty]
        public int Initiative { get; internal set; }
        [JsonProperty(PropertyName = "next_chain")]
        public int NextChain { get; internal set; }
        [JsonProperty(PropertyName = "prev_chain")]
        public int PrevChain { get; internal set; }
        [JsonProperty(PropertyName = "transform_skills")]
        public IReadOnlyList<long> TransformSkills { get; internal set; }
        [JsonProperty(PropertyName = "bundle_skills")]
        public IReadOnlyList<long> BundleSkills { get; internal set; }

        [JsonProperty(PropertyName = "toolbelt_skill")]
        public int ToolbeltSkill { get; internal set; }
    }

}

