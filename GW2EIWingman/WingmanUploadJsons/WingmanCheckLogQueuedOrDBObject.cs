using Newtonsoft.Json;

namespace GW2EIWingman.WingmanUploadJsons
{
    public class WingmanCheckLogQueuedOrDBObject : WingmanCheckLogQueuedObject
    {
        [JsonProperty]
        public bool InDB { get; internal set; }
    }
}
