using System.Text.Json;
using System.Text.Json.Serialization;

namespace GW2EIDPSReport.DPSReportJsons;

#pragma warning disable CS0649 // field never assigned to
public class DPSReportUploadObject
{
    
    public string? Id;
    
    public string? Permalink;
    
    public string? Identifier;
    
    public long? UploadTime;
    
    public long? EncounterTime;
    
    public string? Generator;
    
    public int? GeneratorId;
    
    public int? GeneratorVersion;
    
    public string? Language;
    
    public int? LanguageId;
    
    public string? UserToken;
    
    public string? Error;
    
    public DPSReportUploadEncounterObject? Encounter;
    
    public DPSReportUploadEvtcObject? Evtc;

    [JsonIgnore]
    public Dictionary<string, DPSReportUploadPlayerObject>? Players
    {
        get
        {
            var json = PlayersJson?.ToString()!;
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, DPSReportUploadPlayerObject>>(json)!;
            }
            catch
            {
                return null;
            }
        }
    }

    [JsonPropertyName("players")]
    internal object? PlayersJson;
    
    public DPSReportReportObject? Report;
    
    public string? TempApiId;
}
#pragma warning restore CS0649 // field never assigned to
