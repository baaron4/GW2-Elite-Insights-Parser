using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using GW2EIParserCommons;
using GW2EIParserCommons.Properties;

namespace GW2EIParserAvalonia.Services;

internal sealed class ApplicationTrace : IApplicationTrace
{
    private readonly string _traceFileName;
    private readonly BlockingCollection<string> _messages = [];

    public ApplicationTrace()
    {
        _traceFileName = $"{ProgramHelper.EILogPath}EILogs-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
        _ = Task.Run(WriteToFile);
    }

    public void Add(string message)
    {
        if (Settings.Default.ApplicationTraces)
        {
            _messages.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
        }
    }

    private void WriteToFile()
    {
        foreach (string message in _messages.GetConsumingEnumerable())
        {
            Directory.CreateDirectory(ProgramHelper.EILogPath);
            File.AppendAllText(_traceFileName, message + Environment.NewLine);
        }
    }
}
