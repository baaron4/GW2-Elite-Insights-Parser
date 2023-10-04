using Newtonsoft.Json;

namespace GW2EIWingman.WingmanUploadJsons
{
    public class WingmanImportLogQueuedObject
    {
        [JsonProperty]
        public string Link { get; internal set; }
        [JsonProperty]
        public string Note { get; internal set; }
        [JsonProperty]
        public int Success { get; internal set; }
    }
}
