using Newtonsoft.Json;

namespace GW2EIDPSReport.DPSReportJsons
{
    public class DPSReportUploadEncounterObject
    {
        //[JsonProperty]
        //public int UniqueId { get; internal set; }
        //[JsonProperty]
        //public bool Success { get; internal set; }
        //[JsonProperty]
        // long Duration { get; internal set; }
        //[JsonProperty]
        //public long CompDps { get; internal set; }
        [JsonProperty]
        public int NumberOfPlayers { get; internal set; }
        //[JsonProperty]
        //public int NumberOfGroups { get; internal set; }
        [JsonProperty]
        public long BossId { get; internal set; }
        //[JsonProperty]
        //public long Gw2Build { get; internal set; }
        [JsonProperty]
        public bool JsonAvailable { get; internal set; }
    }
}
