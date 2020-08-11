using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GW2EIDPSReport.DPSReportJsons
{
    public class DPSReportUploadObject
    {
        [JsonProperty]
        public string Id { get; internal set; }
        [JsonProperty]
        public string Permalink { get; internal set; }
        [JsonProperty]
        public string Identifier { get; internal set; }
        [JsonProperty]
        public long UploadTime { get; internal set; }
        [JsonProperty]
        public long EncounterTime { get; internal set; }
        [JsonProperty]
        public string Generator { get; internal set; }
        [JsonProperty]
        public string Language { get; internal set; }
        [JsonProperty]
        public string UserToken { get; internal set; }
        [JsonProperty]
        public string TempApiId { get; internal set; }
        [JsonProperty]
        public string Error { get; internal set; }
        [JsonProperty]
        public DPSReportUploadEncounterObject Encounter { get; internal set; }
        [JsonProperty]
        public DPSReportUploadEvtcObject Evtc { get; internal set; }
        [JsonProperty]
        public Dictionary<string, DPSReportUploadPlayerObject> Players { get; internal set; }
    }
}
