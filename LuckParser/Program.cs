using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LuckParser
{
    static class Program
    {
#if !DEBUG
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool FreeConsole();
#endif
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            // Migrate previous settings if version changed
            if (Properties.Settings.Default.Outdated)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Outdated = false;
            }

            List<string> logFiles = new List<string>();
            bool uiMode = true;
            Application.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            if (args.Length > 0)
            {
                List<string> argsList = args.ToList();
                HashSet<string> argsSet = new HashSet<string>(argsList);
                HashSet<int> usedIds = new HashSet<int>();
                uiMode = false;
                if (argsSet.Contains("-h"))
                {
                    Console.WriteLine("GuildWars2EliteInsights " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                    Console.WriteLine("");
                    Console.WriteLine("-c [config path]         : uses specified config file");
                    Console.WriteLine("-ui                      : runs the application with user interface");
                    Console.WriteLine("-h                       : displays this screen");
                    Console.WriteLine("-v                       : displays the version");
                    Console.WriteLine("-f                       : files to parse. Giving files without -f is deprecated");
                    return 0;
                }

                if (argsSet.Contains("-v"))
                {
                    Console.WriteLine("GuildWars2EliteInsights " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                    return 0;
                }

                if (argsSet.Contains("-ui"))
                {
                    uiMode = true;
                    usedIds.Add(argsList.IndexOf("-ui"));
                }

                if (argsSet.Contains("-c"))
                {
                    int id = argsList.IndexOf("-c");
                    if (id == argsList.Count - 1 || argsList[id + 1].StartsWith("-"))
                    {
                        Console.WriteLine("A path to a config file must be specified after -c");
                        return 0;
                    } else
                    {
                        CustomSettingsManager.ReadConfig(argsList[id + 1]);
                        usedIds.Add(id);
                        usedIds.Add(id + 1);
                    }
                }

                if (argsSet.Contains("-f"))
                {
                    int id = argsList.IndexOf("-f");
                    for (int i = id + 1; i < argsList.Count; i++)
                    {
                        string file = argsList[i];
                        if (file.StartsWith("-"))
                        {
                            break;
                        }
                        logFiles.Add(file);
                    }
                }
                else
                {
                    for (int i = 0; i < argsList.Count; i++)
                    {
                        if (!usedIds.Contains(i) && !argsList[i].StartsWith("-"))
                            logFiles.Add(argsList[i]);
                    }
                }

            }
            if (uiMode)
            {
#if !DEBUG
                FreeConsole();
#endif
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(logFiles));
            }
            else
            {
                if (logFiles.Count > 0)
                {
                    // Use the application through console 
                    new ConsoleProgram(logFiles);
                    return 0;
                }
            }
            return 0;
        }
    }
}
