using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using GW2EIParserCommons;
using GW2EIUpdater;

[assembly: CLSCompliant(false)]
namespace GW2EIParser;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        Console.WriteLine($"{Process.GetCurrentProcess().ProcessName} {Assembly.GetEntryAssembly().GetName().Version}");

        // Migrate previous settings if version changed
        if (Properties.Settings.Default.Outdated)
        {
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Outdated = false;
        }

        // Clean up temp folder on startup
        Updater.CleanTemp("GW2EICLIUpdateTemp");

        var logFiles = new List<string>();
        CultureInfo.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
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
                    Console.WriteLine("Update check has failed");
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
                    if (!Updater.DownloadAndUpdate(info, "GW2EICLIUpdateTemp", "GW2EICLI.zip", traces).GetAwaiter().GetResult())
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
                ProgramHelper.APIController.WriteAPITraitsToFile(ProgramHelper.TraitAPICacheLocation);
                ProgramHelper.APIController.WriteAPISpecsToFile(ProgramHelper.SpecAPICacheLocation);
                parserArgOffset += 1;
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
        if (logFiles.Count > 0)
        {
            return ConsoleProgram.ParseAll(logFiles, programHelper);
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
        Console.WriteLine("-cache : will update the API caches");
    }
}
