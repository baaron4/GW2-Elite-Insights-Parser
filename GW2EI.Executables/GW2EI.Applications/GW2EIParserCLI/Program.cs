using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using GW2EIParserCommons;
using GW2EIParserCommons.Properties;
using GW2EIUpdater;

[assembly: CLSCompliant(false)]
namespace GW2EIParserWinForms;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        Console.WriteLine($"{Process.GetCurrentProcess().ProcessName} {Assembly.GetEntryAssembly().GetName().Version}" + Environment.NewLine);

        // Migrate previous settings if version changed
        if (Settings.Default.Outdated)
        {
            Settings.Default.Upgrade();
            Settings.Default.Outdated = false;
        }

        // Clean up temp folder on startup
        Updater.CleanTemp();

        var logFiles = new List<string>();
        CultureInfo.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        bool batchDiscord = false;
        string pathToWatch = null;
        if (args.Length > 0)
        {
            int parserArgOffset = 0;

            if (args.Contains("-update"))
            {
                Console.WriteLine("Checking for a new update");
                List<string> traces = [];
                Updater.UpdateInfo? _info = Updater.CheckForUpdate("GW2EICLI.zip", traces).GetAwaiter().GetResult();
                if (_info == null)
                {
                    Console.WriteLine("Update tentative has failed, please try again later or update manually.");
                    traces.ForEach(x => Console.WriteLine(x));
                    return 0;
                }
                traces.Clear();
                Updater.UpdateInfo info = _info.Value;
                if (info.UpdateAvailable)
                {
                    Console.WriteLine("New release has been found");
                    Console.WriteLine($"Current Elite Insights version: {info.CurrentVersion}");
                    Console.WriteLine($"Latest Elite Insights version: {info.LatestVersion}");
                    Console.WriteLine($"Download Size: {info.DownloadSize}");
                    Console.WriteLine("Installing");
                    if (!Updater.DownloadAndUpdate(info, traces).GetAwaiter().GetResult())
                    {
                        traces.ForEach(x => Console.WriteLine(x));
                    }
                }
                else
                {
                    Console.WriteLine("Elite Insights is up to date.");
                }
                return 0;
            }

            if (args.Contains("-h"))
            {
                PrintHelp();
                return 0;
            }

            if (args.Contains("-cache"))
            {
                ProgramHelper.APIController.WriteAPISkillsToFile(ProgramHelper.SkillAPICacheLocation);
                ProgramHelper.APIController.WriteAPIMapsToFile(ProgramHelper.MapAPICacheLocation);
                ProgramHelper.APIController.WriteAPITraitsToFile(ProgramHelper.TraitAPICacheLocation);
                ProgramHelper.APIController.WriteAPISpecsToFile(ProgramHelper.SpecAPICacheLocation);
                parserArgOffset += 1;
            }

            if (args.Contains("-populate_from"))
            {
                int argPos = Array.IndexOf(args, "-populate_from");
                var addPath = args[argPos + 1];
                try
                {
                    var duration = long.Parse(args[argPos + 2]);
                    logFiles.AddRange(ProgramHelper.FetchSupportedFormatsFrom(addPath, duration, DateTime.Now));
                } 
                catch (Exception)
                {
                    PrintHelp();
                    return 1;
                }
                parserArgOffset += 3;
            }

            if (args.Contains("-discord_batch"))
            {
                batchDiscord = true;
                parserArgOffset += 1;
            }

            if (args.Contains("-watch"))
            {
                int argPos = Array.IndexOf(args, "-watch");
                pathToWatch = args[argPos + 1];
                parserArgOffset += 2;
            }

            if (args.Contains("-c"))
            {
                if (args.Length - parserArgOffset >= 2)
                {
                    // Do not access settings before this, else this will not work
                    int argPos = Array.IndexOf(args, "-c");

                    CustomSettingsManager.ReadConfig(args[argPos + 1]);

                    parserArgOffset += 2;
                }
                else
                {
                    Console.WriteLine("More arguments required for option -c:");
                    Console.WriteLine("GuildWars2EliteInsights.exe -c [config path] [logs]");
                    return 0;
                }
            }

            for (int i = parserArgOffset; i < args.Length; i++)
            {
                logFiles.Add(args[i]);
            }

        }
        var thisAssembly = Assembly.GetExecutingAssembly();
        var settings = CustomSettingsManager.GetProgramSettings();
        using var programHelper = new ProgramHelper(thisAssembly.GetName().Version, settings);
        if (logFiles.Count > 0 || pathToWatch != null)
        {
            var consoleProgram = new ConsoleProgram(programHelper, batchDiscord, pathToWatch);
            return consoleProgram.ParseAll(logFiles);
        }
        else
        {
            PrintHelp();
            return 1;
        }
    }

    static void PrintHelp()
    {
        Console.WriteLine($"{Path.GetFileName(Environment.ProcessPath)} [arguments] [logs...]");
        Console.WriteLine("");
        Console.WriteLine("-c [config path] : use another config file");
        Console.WriteLine("-h : help");
        Console.WriteLine("-watch [path to watch] : will put the application in watch mode where evtc files added to the path will be automatically parsed");
        Console.WriteLine("-populate_from [path] [duration] : evtcs inside and under path will be added to the list of logs to be parsed. Duration, in hours, is used to limit the age of the logs, 0 for infinite");
        Console.WriteLine("-discord_batch : will upload logs uploaded to dps.report to the provided discord webhook. Will do nothing without a webhook or no dps.report urls");
        Console.WriteLine("-cache : will update the API caches");
        Console.WriteLine("-update : will check for the latest update available on GitHub and restart the application");
    }
}
