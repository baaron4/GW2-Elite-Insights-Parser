using System.Runtime.InteropServices;

namespace GW2EIEvtcParser;

public static class MemoryWatchdog
{
    public enum MemoryPressure
    {
        Low,
        Medium,
        High
    }

    static long s_availableMemory = 0;

    /// <remarks>Not thread safe</remarks> TODO(Rennorb)
    public static MemoryPressure GetMemoryPressure(long memoryLimit = 0)
    {
        const double HighPressureThreshold = .90;       // Percent of GC memory pressure threshold we consider "high"
        const double MediumPressureThreshold = .70;     // Percent of GC memory pressure threshold we consider "medium"

        if(memoryLimit <= 0)
        {
            if(s_availableMemory == 0)
            {
                // inspired by https://github.com/NickStrupat/ComputerInfo, but only the part we care about
                switch(Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT: {
                       s_availableMemory = Windows.GetAvailableMemory();
                    } break;

                    case PlatformID.Unix: {
                        var memTotal = "MemTotal:";
                        var memInfoLines = File.ReadAllLines("/proc/meminfo");
                        var memTotalLine = memInfoLines.FirstOrDefault(x => x.StartsWith(memTotal, StringComparison.Ordinal))?[memTotal.Length..];
                        if (memTotalLine != null && memTotalLine.EndsWith("kB", StringComparison.Ordinal) && long.TryParse(memTotalLine[..^2], out var memKb))
                        {
                            s_availableMemory = memKb * 1024;
                        }
                        else
                        {
                            throw new Exception("Failed to parse total memory fro mmeminfo file.");
                        }
                    } break;
                }
            }

            memoryLimit = s_availableMemory;
        }

        var currentUsage = GC.GetTotalMemory(false);

        if (currentUsage >= memoryLimit * HighPressureThreshold)
        {
            return MemoryPressure.High;
        }

        if (currentUsage >= memoryLimit * MediumPressureThreshold)
        {
            return MemoryPressure.Medium;
        }

        return MemoryPressure.Low;
    }

    //NOTE(Rennorb): Wrapped in its own class so it hopefully only gets instantiated on windows (bacause it isnt used anywhere else).
    //TODO(Rennorb) @cleanup: I really don't like this "praying that it works" but i haven't figured out how to work with native dlls in netstandard in a dynamic fassion.
    static class Windows
    {
        struct MEMORYSTATUSEX
        {
            public UInt32 dwLength;
            public UInt32 dwMemoryLoad;
            public UInt64 ullTotalPhys;
            public UInt64 ullAvailPhys;
            public UInt64 ullTotalPageFile;
            public UInt64 ullAvailPageFile;
            public UInt64 ullTotalVirtual;
            public UInt64 ullAvailVirtual;
            public UInt64 ullAvailExtendedVirtual;

            public void Init()
            {
                dwLength = checked((UInt32)Marshal.SizeOf(typeof(MEMORYSTATUSEX)));
            }
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern Boolean GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        public static long GetAvailableMemory()
        {
            var info = new MEMORYSTATUSEX();
            info.Init();
            if(!GlobalMemoryStatusEx(ref info)) {
                throw new Exception("Failed to call GlobalMemoryStatusEx.");
            }

            return (long)info.ullAvailVirtual;
        }
    }
}
