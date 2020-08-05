using Newtonsoft.Json;

namespace GW2EIControllers.GW2API
{
    public class GW2APISpec
    {
        [JsonProperty]
        public int Id { get; internal set; }
        [JsonProperty]
        public string Name { get; internal set; }
        [JsonProperty]
        public string Profession { get; internal set; }
        [JsonProperty]
        public bool Elite { get; internal set; }
        //minor_traits
        //major_traits
        [JsonProperty]
        public string Icon { get; internal set; }
        [JsonProperty]
        public string Background { get; internal set; }
    }
}
