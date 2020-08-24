using Newtonsoft.Json;

namespace GW2EIDPSReport.DPSReportJsons
{
    public class DPSReportGetUploadsObject
    {
        [JsonProperty]
        public int Pages { get; internal set; }
        [JsonProperty]
        public string UserToken { get; internal set; }
        [JsonProperty]
        public DPSReportUploadObject[] Uploads { get; internal set; }
    }
}
