using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace GW2EIUpdater;

public static class Updater
{
    /// <summary>
    /// Structure to store the update information.
    /// </summary>
    public struct UpdateInfo(GitHubRelease release, string current, string latest, string size, string file, bool update)
    {
        public readonly GitHubRelease Release = release;
        /// <summary>
        /// Current Elite Insights running version.
        /// </summary>
        public readonly string CurrentVersion = current;
        /// <summary>
        /// Latest Elite Insights version found.
        /// </summary>
        public readonly string LatestVersion = latest;
        /// <summary>
        /// Latest Elite Insights release page link.
        /// </summary>
        public string ReleasePageURL => Release.HtmlUrl;
        /// <summary>
        /// Size of the downloadable file.
        /// </summary>
        public readonly string DownloadSize = size;
        /// <summary>
        /// Name of the file to download.
        /// </summary>
        public readonly string FileName = file;
        /// <summary>
        /// Wether a new Elite Insights update has been found or not.
        /// </summary>
        public readonly bool UpdateAvailable = update;
    }

    private static readonly HttpClient _httpClient = new();

    /// <summary>
    /// Compares the current Elite Insights versions to the latest released version on GitHub.
    /// </summary>
    /// <returns>Returns <see cref="UpdateInfo"/> with the update information of the latest version if it has a higher number than the current.</returns>
    public static async Task<UpdateInfo?> CheckForUpdate(string fileName, List<string> traces)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var currentVersion = entryAssembly?.GetName().Version;

        // GitHub API Call & JSON Object creation
        try
        {
            // Uri & Client Headers
            var uri = new Uri("https://api.github.com/repos/baaron4/GW2-Elite-Insights-Parser/releases/latest");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GW2EI-Updater/1.0");
            _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

            // API Response
            using var responseMessage = await _httpClient.GetAsync(uri);
            responseMessage.EnsureSuccessStatusCode();

            // Response serialization
            var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
            var latestRelease = JsonSerializer.Deserialize<GitHubRelease>(jsonResponse) ?? throw new InvalidOperationException("Deserialize failed");

            // Release format is "v1.0.0.0"
            string version = latestRelease.Name.Substring(1); // Remove "v"
            var latestVersion = Version.Parse(version);

            // File download size
            var asset = latestRelease.Assets.FirstOrDefault(x => x.Name.Equals(fileName)) ?? throw new InvalidOperationException("Asset could not be found");
            long size = asset.Size;

            return new UpdateInfo(
                latestRelease,
                (currentVersion ?? new Version(0, 0 , 0 , 0)).ToString(),
                latestVersion.ToString(),
                $"{size / (1024.0 * 1024.0):F2} MB",
                fileName,
                latestVersion > currentVersion);
        }
        catch (Exception ex)
        {
            traces.Add($"Update check failed: {ex.Message}");
        }
        return null;
    }

    /// <summary>
    /// Downloads the latest released .zip and executes the update.
    /// </summary>
    public static async Task<bool> DownloadAndUpdate(UpdateInfo info, string dlFolderName, string fileName, List<string> traces)
    {
#if DEBUG
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (location == null)
        {
            traces.Add($"Download location does not exist");
            return false;
        }
        string tempPath = Path.Combine(location, "GW2EITemp");
#else

        // Windows: C:\Users\User\AppData\Local\Temp\
        // Linux: /tmp/
        string tempPath = Path.GetTempPath();
#endif
        // Windows: C:\Users\User\AppData\Local\Temp\GW2EIUpdateTemp\ or \GW2EICLIUpdateTemp\
        // Linux: /tmp/GW2EIUpdateTemp/ or /GW2EICLIUpdateTemp/
        string folderPath = Path.Combine(tempPath, dlFolderName);

        try
        {
            traces.Add($"Creating temp directory {folderPath}");
            Directory.CreateDirectory(folderPath);
            var downloadUrl = info.Release.Assets.FirstOrDefault(x => x.Name.Equals(info.FileName))?.BrowserDownloadUrl;
            if (downloadUrl == null)
            {
                traces.Add($"No download url");
                return false;
            }
            // Windows: C:\Users\User\AppData\Local\Temp\GW2EIUpdateTemp\GW2EI.zip or GW2EICLIUpdateTemp\GW2EICLI.zip
            // Linux: /tmp/GW2EIUpdateTemp/GW2EI.zip or GW2CLI.zip
            var filePath = Path.Combine(folderPath, info.FileName);
            traces.Add($"Downloading {downloadUrl} to {filePath}");

            // Get response message
            var uri = new Uri(downloadUrl);
            using HttpResponseMessage responseMessage = await _httpClient.GetAsync(uri);
            responseMessage.EnsureSuccessStatusCode();

            // Read Zip content
            using Stream response = await responseMessage.Content.ReadAsStreamAsync();
            using var ms = new MemoryStream();
            await response.CopyToAsync(ms);
            ms.Position = 0;

            // Unzip and save from memory stream
            try
            {

                using var archive = new ZipArchive(ms, ZipArchiveMode.Read);
                archive.ExtractToDirectory(folderPath, overwriteFiles: true);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Downloaded zip archive extraction has failed", ex);
            }

        }
        catch (Exception ex)
        {
            traces.Add($"Download failed with {ex.Message}");
            return false;
        }

        // Execute updater process
        try
        {
            traces.Add($"Executing GuildWars2EliteInsightsUpdater.exe with arguments ${AppContext.BaseDirectory} and {Process.GetCurrentProcess().ProcessName}");
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = Path.Combine(folderPath, "GuildWars2EliteInsightsUpdater.exe"),
            };
            psi.ArgumentList.Add(AppContext.BaseDirectory); // Let the library handle excaping spaces in the folder names
            psi.ArgumentList.Add(Process.GetCurrentProcess().ProcessName);
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            traces.Add($"GuildWars2EliteInsightsUpdater.exe could not be executed - {ex.Message}");
            return false;
        }
        return true;
    }

    public static void CleanTemp(string dlFolderName)
    {
#if DEBUG
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (location == null)
        {
            return;
        }
        string tempPath = Path.Combine(location, "GW2EITemp");
        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }
#else

        // Windows: C:\Users\User\AppData\Local\Temp\
        // Linux: /tmp/
        string tempPath = Path.GetTempPath();
        string folderPath = Path.Combine(tempPath, dlFolderName);
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }
#endif
    }
}
