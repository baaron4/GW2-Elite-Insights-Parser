using System.Globalization;
using GW2EIParser;
using GW2EIParserCommons;

// We set this culture to match the one used in the WinForms program.
var cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
Thread.CurrentThread.CurrentCulture = cultureInfo;
Thread.CurrentThread.CurrentUICulture = cultureInfo;

// get args from command line
if (args.Length == 0)
{
    //Console.WriteLine($"Usage: ./GW2EIConsole -c [config path] [logs]");
    // TODO: Get program name
    Console.WriteLine($"Usage: ./GW2EIConsole [logs]");
    return 1;
}

// TODO: Config path (-c [config path])
var logFiles = args;

// TODO: Get settings (the WinForms version relies on WinForms-specific property APIs)
var settings = new ProgramSettings();
// TODO: Get version
var programHelper = new ProgramHelper(new Version(), settings);

// Note that the ConsoleProgram does all its logic in the constructor
_ = new ConsoleProgram(logFiles, programHelper);

return 0;
