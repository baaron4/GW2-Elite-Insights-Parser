using Newtonsoft.Json;

namespace GW2EIGW2API.GW2API
{
    public abstract class GW2APIBaseItem
    {
        [JsonProperty]
        public long Id { get; internal set; }
    }
}

