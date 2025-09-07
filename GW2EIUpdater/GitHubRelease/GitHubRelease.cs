using Newtonsoft.Json;

namespace GW2EIUpdater;

/// <summary>
/// See https://api.github.com/repos/baaron4/GW2-Elite-Insights-Parser/releases/latest for available properties.
/// </summary>
internal sealed class GitHubRelease
{
    [JsonProperty("html_url")]
    internal string Html_Url { get; set; }

    [JsonProperty("name")]
    internal string Name { get; set; }

    [JsonProperty("assets")]
    internal GitHubReleaseAsset[] Assets { get; set; }
}
