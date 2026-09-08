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
                        process.WaitForExit();
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
            try
            {
                var maxTries = 10;
                if (checker.Length == 0)
                {
                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(localDirectory, "*", SearchOption.AllDirectories))
                    {
                        for (int i = 0; i < maxTries; i++)
                        {
                            try
                            {
                                Directory.CreateDirectory(dirPath.Replace(localDirectory, originDirectory));
                            }
                            catch (IOException) when (i < maxTries - 1)
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }

                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(localDirectory, "*.*", SearchOption.AllDirectories))
                    {
                        for (int i = 0; i < maxTries; i++)
                        {
                            try
                            {
                                File.Copy(newPath, newPath.Replace(localDirectory, originDirectory), true);
                            }
                            catch (IOException) when (i < maxTries - 1)
                            {
                                Thread.Sleep(100);
                            }
                        }
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
            catch
            {
                Console.WriteLine("Automatic update failed, please update the application manually.");
                Console.WriteLine("Press any key to continue.");
                _ = Console.ReadKey();
            }
        }

        return 0;
    }
}
