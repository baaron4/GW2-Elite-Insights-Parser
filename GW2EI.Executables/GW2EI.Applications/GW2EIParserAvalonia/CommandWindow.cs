using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GW2EIParserCommons;

namespace GW2EIParserAvalonia;

internal class CommandWindow
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

        return fileType == FileType.Disk || fileType == FileType.Pipe;
    }

    internal static readonly CommandLineOptions TextOutputCommandLineOptions = new();

    private readonly string[] _args;

    public CommandWindow(string[]? args)
    {
        _args = args ?? [];
    }

    public CommandLineOptions? Initialize()
    {
        var logFiles = new List<string>();
        string configPath = "";

        if (_args.Length == 0)
        {
            return new CommandLineOptions
            {
                LogFiles = logFiles,
                ConfigPath = configPath
            };
        }

        int parserArgOffset = 0;

        if (_args.Contains("-p"))
        {
            parserArgOffset += 1;
        }
        else if (OperatingSystem.IsWindows())
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

        if (_args.Contains("-h"))
        {
            Console.WriteLine($"{_args[0]} [arguments] [logs...]");
            Console.WriteLine("");
            Console.WriteLine("-c [config path] : use another config file");
            Console.WriteLine("-h : help");
            Console.WriteLine("-v : show current version");
            Console.WriteLine("-p : disable windows specific functions");
            Console.WriteLine("-inspect_raw : will launch an inspector window on the first given log file, content will be the raw data as present in the evtc. If no log files are provided, the default app will launch instead. If the parsing is faulty, application will close. Log files after the first are ignored.");
            Console.WriteLine("-inspect_ei_partial : will launch an inspector window on the first given log file, content will be pre-processed by ei (gadget identifications, buff formula deduction, etc). If no log files are provided, the default app will launch instead. If the parsing is faulty, application will close. Log files after the first are ignored.");
            Console.WriteLine("-inspect_ei_full : will launch an inspector window on the first given log file, content will be fully processed by ei (partial + custom events and agents). If no log files are provided, the default app will launch instead. If the parsing is faulty, application will close. Log files after the first are ignored.");
            return TextOutputCommandLineOptions;
        }

        if (_args.Contains("-v"))
        {
            Console.WriteLine($"v{Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty}");
            return TextOutputCommandLineOptions;
        }

        if (_args.Contains("-c"))
        {
            if (_args.Length - parserArgOffset >= 2)
            {
                // Do not access settings before this, else this will not work
                int argPos = Array.IndexOf(_args, "-c");

                configPath = _args[argPos + 1];
                CustomSettingsManager.ReadConfig(configPath);

                parserArgOffset += 2;
            }
            else
            {
                Console.WriteLine("More arguments required for option -c:");
                Console.WriteLine("GuildWars2EliteInsights.exe -c [config path] [logs]");
                return null;
            }
        }

        var inspectMode = ProgramHelper.InspectionMode.NotApplicable;
        if (_args.Contains("-inspect_raw"))
        {
            parserArgOffset += 1;
            inspectMode = ProgramHelper.InspectionMode.Raw;
        }
        else if (_args.Contains("-inspect_ei_partial"))
        {
            parserArgOffset += 1;
            inspectMode = ProgramHelper.InspectionMode.RawWithEIPreProcess;
        }
        else if (_args.Contains("-inspect_ei_full"))
        {
            parserArgOffset += 1;
            inspectMode = ProgramHelper.InspectionMode.EI;
        }

        for (int i = parserArgOffset; i < _args.Length; i++)
        {
            logFiles.Add(_args[i]);
            if (inspectMode != ProgramHelper.InspectionMode.NotApplicable)
            {
                break;
            }
        }

        if (inspectMode != ProgramHelper.InspectionMode.NotApplicable && logFiles.Count == 0)
        {
            inspectMode = ProgramHelper.InspectionMode.NotApplicable;
        }

        return new CommandLineOptions
        {
            LogFiles = logFiles,
            ConfigPath = configPath,
            InspectMode = inspectMode
        };
    }
}
