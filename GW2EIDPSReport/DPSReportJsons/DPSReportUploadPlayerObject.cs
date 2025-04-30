using System.Text.Json.Serialization;

namespace GW2EIDPSReport.DPSReportJsons;

public class DPSReportUploadPlayerObject
{
    //NOTE(Rennorb): This apparently diverges from the usual camel case used elsewhere for dps.report.

    [JsonPropertyName("display_name")]
    public string? DisplayName;
    [JsonPropertyName("character_name")]
    public string? CharacterName;
    [JsonPropertyName("profession")]
    public int? Profession;
    [JsonPropertyName("elite_spec")]
    public int? EliteSpec;
}
