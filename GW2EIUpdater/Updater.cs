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
    public static async Task<UpdateInfo> CheckForUpdate(string fileName)
    {
        Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;

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
            var latestRelease = JsonSerializer.Deserialize<GitHubRelease>(jsonResponse);

            // Release format is "v1.0.0.0"
            string version = latestRelease.Name.Substring(1); // Remove "v"
            var latestVersion = Version.Parse(version);

            // File download size
            long size = latestRelease.Assets.FirstOrDefault(x => x.Name.Equals(fileName)).Size;

            return new UpdateInfo(
                latestRelease,
                currentVersion.ToString(),
                latestVersion.ToString(),
                $"{size / (1024.0 * 1024.0):F2} MB",
                fileName,
                latestVersion > currentVersion);
        }
        catch
        {
            throw new HttpRequestException("API Request failed");
        }
    }

    /// <summary>
    /// Downloads the latest released .zip and executes the update.
    /// </summary>
    public static async Task DownloadAndUpdate(UpdateInfo info, string dlFolderName, string fileName)
    {
#if DEBUG
        string tempPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GW2EITemp");
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
            Directory.CreateDirectory(folderPath);
            var downloadUrl = info.Release.Assets.FirstOrDefault(x => x.Name.Equals(info.FileName)).BrowserDownloadUrl;

            // Windows: C:\Users\User\AppData\Local\Temp\GW2EIUpdateTemp\GW2EI.zip or GW2EICLIUpdateTemp\GW2EICLI.zip
            // Linux: /tmp/GW2EIUpdateTemp/GW2EI.zip or GW2CLI.zip
            var filePath = Path.Combine(folderPath, info.FileName);

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
            throw new HttpRequestException("Update failed", ex);
        }

        // Execute updater process
        try
        {
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
            throw new DllNotFoundException("GW2EIUpdater executable not found", ex);
        }
    }

    public static void CleanTemp(string dlFolderName)
    {
#if DEBUG
        string tempPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GW2EITemp");
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
