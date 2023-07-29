using Newtonsoft.Json;

namespace GW2EIDPSReport.DPSReportJsons
{
    public class DPSReportUploadEvtcObject
    {
        [JsonProperty]
        public string Type { get; internal set; }
        [JsonProperty]
        public string Version { get; internal set; }
        [JsonProperty]
        public long BossId { get; internal set; }
    }
}
