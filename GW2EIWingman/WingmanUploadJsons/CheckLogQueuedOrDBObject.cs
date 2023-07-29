using Newtonsoft.Json;

namespace GW2EIWingman.WingmanUploadJsons
{
    public class CheckLogQueuedOrDBObject : CheckLogQueuedObject
    {
        [JsonProperty]
        public bool InDB { get; internal set; }
    }
}
