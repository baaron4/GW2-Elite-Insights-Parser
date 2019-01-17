using Newtonsoft.Json;

namespace LuckParser.Controllers
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
        public string[] Professions { get; set; }
        public string Slot { get; set; }
        public string[] Categories { get; set; }
        public string[] Flags { get; set; }
        public GW2APIFacts[] Facts { get; set; }
        public long Specialization { get; set; }
        [JsonProperty(PropertyName = "dual_wield")]
        public string DualWield { get; set; }
    }

}