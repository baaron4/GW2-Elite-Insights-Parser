using System.Globalization;
using System.Reflection;
using GW2EIParserCommons;

[assembly: CLSCompliant(false)]
namespace GW2EIParser;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        // Migrate previous settings if version changed
        if (Properties.Settings.Default.Outdated)
        {
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Outdated = false;
        }

        var logFiles = new List<string>();
        CultureInfo.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        if (args.Length > 0)
        {
            int parserArgOffset = 0;

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
