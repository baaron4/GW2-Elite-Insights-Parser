using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIDPSReport.DPSReportJsons
{
    public class ReportObject
    {
        [JsonProperty]
        public bool Anonymous { get; internal set; }
        [JsonProperty]
        public bool Detailed { get; internal set; }
    }
}
