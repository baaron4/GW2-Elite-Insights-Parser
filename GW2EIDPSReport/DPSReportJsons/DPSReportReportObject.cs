using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIDPSReport.DPSReportJsons
{
    public class DPSReportReportObject
    {
        [JsonProperty]
        public string Anonymous { get; internal set; }
        [JsonProperty]
        public bool Detailed { get; internal set; }
    }
}
