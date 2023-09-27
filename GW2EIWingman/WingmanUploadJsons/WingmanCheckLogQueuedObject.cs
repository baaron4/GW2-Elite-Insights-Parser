using Newtonsoft.Json;

namespace GW2EIWingman.WingmanUploadJsons
{
    public class WingmanCheckLogQueuedObject
    {
        [JsonProperty]
        public string Link { get; internal set; }
        [JsonProperty]
        public string TargetURL { get; internal set; }
        [JsonProperty]
        public bool InQueue { get; internal set; }
    }
}
