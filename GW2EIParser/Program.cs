using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using GW2EIParserCommons;
using GW2EIUpdater;

[assembly: CLSCompliant(false)]
namespace GW2EIParser;

internal static class Program
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();
    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(StandardHandle nStdHandle);
    [DllImport("kernel32.dll")]
    private static extern FileType GetFileType(IntPtr handle);

    private const int AttachParentProcess = -1;

    private enum StandardHandle
    {
        Input = -10,
        Output = -11,
        Error = -12
    }

    private enum FileType : uint
    {
        Unknown = 0x0000,
        Disk = 0x0001,
        Char = 0x0002,
        Pipe = 0x0003
    }

    private static bool IsRedirected(IntPtr handle)
    {
        FileType fileType = GetFileType(handle);

        return (fileType == FileType.Disk) || (fileType == FileType.Pipe);
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static int Main(string[] args)
    {
        // Migrate previous settings if version changed
        if (Properties.Settings.Default.Outdated)
        {
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Outdated = false;
        }

        // Clean up temp folder on startup
        try
        {
            // TODO restore this comment before merging
            // Directory.Delete(Path.Combine(Path.GetTempPath(), Updater.EI_TempFolder), true);
        }
        catch
        {

        }

        var logFiles = new List<string>();
        Application.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        if (args.Length > 0)
        {
            int parserArgOffset = 0;

            if (args.Contains("-h"))
            {
                Console.WriteLine($"{args[0]} [arguments] [logs...]");
                Console.WriteLine("");
                Console.WriteLine("-c [config path] : use another config file");
                Console.WriteLine("-p : disable windows specific functions");
                Console.WriteLine("-h : help");
                return 0;
            }

            if (args.Contains("-p"))
            {
                parserArgOffset += 1;
            }
            else
            {
                /*
                 * Magic for windows:
                 * - opens a console window if used from a non-console with command line options
                 * - fixes output on windows cmd (other consoles tested behaved better)(otherwise no console output or piped file output)
                 *
                 * We need to do this, because the console output is lazy initialized
                 * and if we are redirecting to a file or pipe we want to make sure Console.out points to the correct handle
                 * and doesn't init with the console ignoring existing stdout
                 */
                if (IsRedirected(GetStdHandle(StandardHandle.Output)))
                {
                    _ = Console.Out;
                }

                if (!AttachConsole(AttachParentProcess))
                {
                    AllocConsole();
                }

                AttachConsole(AttachParentProcess);
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
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using var form = new MainForm(logFiles, programHelper);
        Application.Run(form);
        return 0;
    }
}
