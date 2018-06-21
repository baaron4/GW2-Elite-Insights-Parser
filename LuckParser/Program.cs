using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LuckParser
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Application.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            if (args.Length > 0)
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                /*foreach (String arg in args)
                {
                    Console.WriteLine(arg);
                }*/
                // Use the application through console 
                MainForm myConsoleParser = new MainForm(args);
                
                return 0;
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            return 0;
        }
    }
}
