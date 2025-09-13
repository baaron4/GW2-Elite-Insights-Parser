using System.Diagnostics;

[assembly: CLSCompliant(false)]
namespace GW2EIUpdater;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            string originDirectory = args[0];
            string fullProcessName = args[1];
            string processName = args[1].Replace(".exe", "");
            var eiProcesses = Process.GetProcessesByName(processName);
            var localDirectory = AppContext.BaseDirectory;

            // Close EI Process
            foreach (Process process in eiProcesses)
            {
                try
                {
                    if (!process.WaitForExit(500))
                    {
                        process.Kill();
                    }
                }
                catch
                {
                    // do nothing
                }
            }

            // The Updater can run faster than the process unloads the DLL
            var checker = Process.GetProcessesByName(processName);
            while (checker.Length > 0)
            {
                checker = Process.GetProcessesByName(processName);
            }
            if (checker.Length == 0)
            {
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(localDirectory, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(localDirectory, originDirectory));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(localDirectory, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(localDirectory, originDirectory), true);
                }
            }

            // Start Elite Insights to finish the update
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = Path.Combine(originDirectory, fullProcessName),
                Arguments = "",
            });
        }

        return 0;
    }
}
