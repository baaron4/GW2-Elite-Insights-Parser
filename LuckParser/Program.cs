using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LuckParser
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool FreeConsole();
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
            HashSet<string> argsSet = new HashSet<string>(args.ToList());
            HashSet<int> usedIds = new HashSet<int>();
            if (args.Length > 0)
            {
                uiMode = false;
                if (argsSet.Contains("-h"))
                {
                    Console.WriteLine("GuildWars2EliteInsights " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                    Console.WriteLine("");
                    Console.WriteLine("-c [config path]         : uses specified config file");
                    Console.WriteLine("-ui                      : runs the application with user interface");
                    Console.WriteLine("-h                       : displays this screen");
                    Console.WriteLine("-v                       : displays the version");
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
                    usedIds.Add(Array.IndexOf(args, "-ui"));
                }

                if (argsSet.Contains("-c"))
                {
                    int id = Array.IndexOf(args, "-c");
                    if (id == args.Length - 1 || args[id + 1].StartsWith("-"))
                    {
                        Console.WriteLine("A path to a config file must be specified after -c");
                        return 0;
                    } else
                    {
                        CustomSettingsManager.ReadConfig(args[id + 1]);
                        usedIds.Add(id);
                        usedIds.Add(id + 1);
                    }
                }

                for (int i = 0; i < args.Length; i++)
                {
                    if (!usedIds.Contains(i))
                        logFiles.Add(args[i]);
                }
            }
            if (uiMode)
            {
                FreeConsole();
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
