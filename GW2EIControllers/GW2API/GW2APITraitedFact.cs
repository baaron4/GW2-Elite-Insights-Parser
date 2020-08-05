using Newtonsoft.Json;

namespace GW2EIControllers.GW2API
{
    public class GW2APITraitedFact : GW2APIFact
    {
        [JsonProperty(PropertyName = "requires_trait")]
        public int RequiresTrait { get; internal set; }
        [JsonProperty]
        public int Overrides { get; internal set; }
    }
}
