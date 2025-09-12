using System.Text.Json.Serialization;

namespace GW2EIUpdater;

/// <summary>
/// See https://api.github.com/repos/baaron4/GW2-Elite-Insights-Parser/releases/latest for available properties.
/// </summary>
public sealed class GitHubRelease
{
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("assets")]
    public GitHubReleaseAsset[] Assets { get; set; } = [];
}
