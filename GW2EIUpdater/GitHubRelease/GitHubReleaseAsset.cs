using Newtonsoft.Json;

namespace GW2EIUpdater;

internal class GitHubReleaseAsset
{
    [JsonProperty("name")]
    internal string Name { get; set; }

    [JsonProperty("size")]
    internal long Size { get; set; }

    [JsonProperty("browser_download_url")]
    internal string BrowserDownloadUrl { get; set; }
}
