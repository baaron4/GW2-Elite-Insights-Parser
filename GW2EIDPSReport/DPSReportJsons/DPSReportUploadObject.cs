using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GW2EIDPSReport.DPSReportJsons;

public class DPSReportUploadObject
{
    
    public string Id;
    
    public string Permalink;
    
    public string Identifier;
    
    public long UploadTime;
    
    public long EncounterTime;
    
    public string Generator;
    
    public int GeneratorId;
    
    public int GeneratorVersion;
    
    public string Language;
    
    public int LanguageId;
    
    public string UserToken;
    
    public string Error;
    
    public DPSReportUploadEncounterObject Encounter;
    
    public DPSReportUploadEvtcObject Evtc;

    [JsonIgnore]
    public Dictionary<string, DPSReportUploadPlayerObject> Players
    {
        get
        {
            var json = PlayersJson.ToString();
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, DPSReportUploadPlayerObject>>(json);
            }
            catch
            {
                return new Dictionary<string, DPSReportUploadPlayerObject>();
            }
        }
    }

    [JsonPropertyName("players")]
    internal object PlayersJson { get; set; }
    
    public DPSReportReportObject Report;
    
    public string TempApiId;
}
