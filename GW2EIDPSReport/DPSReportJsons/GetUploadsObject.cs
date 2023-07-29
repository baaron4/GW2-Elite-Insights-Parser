using Newtonsoft.Json;

namespace GW2EIDPSReport.DPSReportJsons
{
    public class GetUploadsObject
    {
        [JsonProperty]
        public int Pages { get; internal set; }
        [JsonProperty]
        public uint CurrentTime { get; internal set; }
        [JsonProperty]
        public int FoundUploads { get; internal set; }
        [JsonProperty]
        public int TotalUploads { get; internal set; }
        [JsonProperty]
        public string UserToken { get; internal set; }
        [JsonProperty]
        public UploadObject[] Uploads { get; internal set; }
    }
}
