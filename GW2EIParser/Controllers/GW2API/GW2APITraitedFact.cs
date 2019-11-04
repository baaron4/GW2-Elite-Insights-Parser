using Newtonsoft.Json;

namespace GW2EIParser.Controllers.GW2API
{
    public class GW2APITraitedFact : GW2APIFact
    {
        [JsonProperty(PropertyName = "requires_trait")]
        public int RequiresTrait { get; set; }
        public int Overrides { get; set; }
    }
}
