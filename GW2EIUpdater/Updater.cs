using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using Newtonsoft.Json;

namespace GW2EIUpdater;

public class Updater
{
    /// <summary>
    /// Current Elite Insights running version.
    /// </summary>
    public string CurrentVersion { get; private set; } = string.Empty;
    /// <summary>
    /// Latest Elite Insights version found.
    /// </summary>
    public string LatestVersion { get; private set; } = string.Empty;
    /// <summary>
    /// Latest Elite Insights release page link.
    /// </summary>
    public string ReleasePageURL { get; private set; } = string.Empty;
    /// <summary>
    /// Wether a new Elite Insights update has been found or not.
    /// </summary>
    public bool UpdateFound { get; private set; } = false;

    public const string EI_DownloadName = "GW2EI.zip";
    public const string EICLI_DownloadName = "GW2EICLI.zip";
    public const string EI_TempFolder = "GW2EIUpdateTemp";
    public const string EIUpdater_Executable = "GuildWars2EliteInsightsUpdater.exe";
    private readonly string Executable = Process.GetCurrentProcess().ProcessName;
    private const string LatestReleaseURL = "https://api.github.com/repos/baaron4/GW2-Elite-Insights-Parser/releases/latest";
    private readonly HttpClient httpClient = new();
    private readonly bool DownloadCLI = false;
    private GitHubRelease LatestRelease;

    public Updater()
    {
    }

    public Updater(bool downloadCLI)
    {
        DownloadCLI = downloadCLI;
    }

    /// <summary>
    /// Checks the version of the latest GitHub release.
    /// </summary>
    /// <returns>Returns true if the latest version has a higher number than current.</returns>
    public async Task<bool> NewReleaseCheckerAsync()
    {
        var assembly = Assembly.GetEntryAssembly();

        if (assembly != null)
        {
            Version currentVersion = assembly.GetName().Version!;

            // GitHub API Call & JSON Object creation
            try
            {
                var uri = new Uri(LatestReleaseURL);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GW2EI-Updater/1.0");
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
                using var responseMessage = await httpClient.GetAsync(uri);
                responseMessage.EnsureSuccessStatusCode();
                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                LatestRelease = JsonConvert.DeserializeObject<GitHubRelease>(jsonResponse);
            }
            catch
            {
                LatestRelease = null;
            }

            if (LatestRelease != null && currentVersion != null)
            {
                // Release format is "v1.0.0.0"
                string version = LatestRelease.Name.Substring(1); // Remove "v"
                var latestVersion = Version.Parse(version);

                // Store release information
                CurrentVersion = currentVersion.ToString();
                LatestVersion = latestVersion.ToString();
                ReleasePageURL = LatestRelease.Html_Url;
                UpdateFound = latestVersion > currentVersion;
            }
            return UpdateFound;
        }
        
        return UpdateFound;
    }

    public async Task DownloadFileAsync()
    {
        string downloadUrl = string.Empty;
        
        // Windows: C:\Users\User\AppData\Local\Temp\
        // Linux: /tmp/
        string tempPath = Path.GetTempPath();

        // Windows: C:\Users\User\AppData\Local\Temp\GW2EIUpdateTemp\
        // Linux: /tmp/GW2EIUpdateTemp/
        string folderPath = Path.Combine(tempPath, EI_TempFolder);

        // Create folder if it doesn't exist
        Directory.CreateDirectory(folderPath);

        try
        {
            if (LatestRelease == null)
            {
                throw new HttpRequestException("Latest Release not found");
            }

            if (DownloadCLI)
            {
                downloadUrl = Array.Find(LatestRelease.Assets, x => x.Name.Equals(EICLI_DownloadName, StringComparison.Ordinal))?.BrowserDownloadUrl!;

                // Windows: C:\Users\User\AppData\Local\Temp\GW2EIUpdateTemp\GW2EI.zip
                // Linux: /tmp/GW2EIUpdateTemp/GW2EI.zip
                string filePath = Path.Combine(folderPath, EICLI_DownloadName);
            }
            else
            {
                downloadUrl = Array.Find(LatestRelease.Assets, x => x.Name.Equals(EI_DownloadName, StringComparison.Ordinal))?.BrowserDownloadUrl!;

                // Windows: C:\Users\User\AppData\Local\Temp\GW2EIUpdateTemp\GW2EI.zip
                // Linux: /tmp/GW2EIUpdateTemp/GW2EI.zip
                string filePath = Path.Combine(folderPath, EI_DownloadName);
            }

            // Get Zip response message
            var uri = new Uri(downloadUrl);
            using HttpResponseMessage responseMessage = await httpClient.GetAsync(uri);
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Download failed");
            }

            // Read Zip content
            using Stream response = await responseMessage.Content.ReadAsStreamAsync();
            using var ms = new MemoryStream();
            await response.CopyToAsync(ms);
            ms.Position = 0;

            // Unzip memory stream
            try
            {
                using var archive = new ZipArchive(ms, ZipArchiveMode.Read);
                archive.ExtractToDirectory(folderPath, overwriteFiles: true);
            }
            catch
            {
                throw new InvalidDataException("Unzip failed");
            }

        }
        catch
        {
            throw new HttpRequestException("Update failed");
        }

        // Execute updater process
        try
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = $"{folderPath}\\{EIUpdater_Executable}",
                Arguments = $"{AppContext.BaseDirectory} {Executable}",
            });
        }
        catch
        {
            throw new DllNotFoundException("Executable not found");
        }
    }
}
