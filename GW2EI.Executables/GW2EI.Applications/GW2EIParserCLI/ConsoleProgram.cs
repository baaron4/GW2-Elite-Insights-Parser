using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using GW2EIEvtcParser;
using GW2EIParserCommons;
using GW2EIParserCommons.Exceptions;
using Tracing;

namespace GW2EIParserWinForms;

public class ConsoleProgram
{
    private readonly List<ulong> DiscordMessageIDs = [];

    private readonly List<ConsoleOperationController> Operations = [];
    private bool Executing = false;

    private readonly ProgramHelper ProgramHelper;
    private readonly bool BatchToDiscord;

    private readonly FileSystemWatcher FileWatcher = new();

    public ConsoleProgram(ProgramHelper programHelper, bool batchToDiscord, string pathToWatch)
    {
        ProgramHelper = programHelper;
        BatchToDiscord = batchToDiscord;
        ProgramHelper.ExecuteMemoryCheckTask();

        if (pathToWatch != null && Directory.Exists(pathToWatch))
        {
            Console.WriteLine("File Watcher: watching " + pathToWatch + Environment.NewLine);
            FileWatcher.Path = pathToWatch;

            FileWatcher.IncludeSubdirectories = true;
            FileWatcher.Created += LogFileWatcher_Created;
            FileWatcher.Renamed += LogFileWatcher_Renamed;
            FileWatcher.EnableRaisingEvents = true;
        }
    }


    /// <returns>0 on success, other value on error</returns>
    public int ParseAll(List<string> logFiles)
    {
        using var _t = new AutoTrace("ParseAll");
        logFiles.ForEach(logFile => Operations.Add(new ConsoleOperationController(logFile)));
        if (FileWatcher.EnableRaisingEvents)
        {
            var exit = new AutoResetEvent(false);

            HandleFiles();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                exit.Set();
            };
            exit.WaitOne();
        }
        else
        {
            HandleFiles();
        }
        return 0;
    }

    #region FILE WATCHER

    /// <summary>
    /// Waits 3 seconds, checks if the file still exists and then adds it to the queue.
    /// This is neccessary because:
    /// 1.) Arc needs some time to complete writing the log file. The watcher gets triggered as soon as the writing starts.
    /// 2.) When Arc is configured to use ZIP compression, the log file is still created as usual, but after the file is written
    ///     it is then zipped and deleted again. Therefore the watcher gets triggered twice, first for the .evtc and then for the .zip.
    /// 3.) Zipping the file also needs time, so we have to wait a bit there too.
    /// </summary>
    /// <param name="path"></param>
    private async void AddDelayed(string path)
    {
        await Task.Delay(3000).ConfigureAwait(false);
        if (File.Exists(path))
        {
            if (Executing)
            {
                AddDelayed(path);
            }
            else
            {
                Console.WriteLine("File Watcher: adding " + path);
                Operations.Add(new ConsoleOperationController(path));
                HandleFiles();
            }
        }
    }

    private void LogFileWatcher_Created(object sender, FileSystemEventArgs e)
    {
        if (ProgramHelper.IsSupportedFormat(e.FullPath))
        {
            Console.WriteLine("File Watcher: created " + e.FullPath + Environment.NewLine);
            AddDelayed(e.FullPath);
        }
    }

    private void LogFileWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        if (ProgramHelper.IsTemporaryCompressedFormat(e.OldFullPath) && ProgramHelper.IsCompressedFormat(e.FullPath))
        {
            Console.WriteLine("File Watcher: renamed " + e.OldFullPath + " to " + e.FullPath + Environment.NewLine);
            AddDelayed(e.FullPath);
        }
        else if (ProgramHelper.IsTemporaryFormat(e.OldFullPath) && ProgramHelper.IsSupportedFormat(e.FullPath))
        {
            Console.WriteLine("File Watcher: renamed " + e.OldFullPath + " to " + e.FullPath + Environment.NewLine);
            AddDelayed(e.FullPath);
        }
    }
    #endregion FILE WATCHER

    private void HandleFiles()
    {
        if (Executing)
        {
            return;
        }
        Executing = true;
        var operationsToExecute = Operations.Where(x => !x.Executed).ToList();
        if (operationsToExecute.Count == 0)
        {
            Executing = false;
            return;
        }
        if (ProgramHelper.ParseMultipleLogs())
        {
            Console.WriteLine("Parsing: Multi-threaded" + Environment.NewLine);
            var state = new ThreadingState()
            {
                ProgramHelper = ProgramHelper,
                NoMoreFiles = false,
                FileQueue = new(),
            };

            var parallelism = ProgramHelper.GetMaxParallelRunning();
            for (int i = 0; i < parallelism - 1; i++)
            {
                var t = new Thread(EnterParserThread);
                t.Start(state);
            }

            foreach (var operation in operationsToExecute)
            {
                state.FileQueue.Enqueue(operation);
            }

            state.NoMoreFiles = true;
            EnterParserThread(state); // we take the last thread
        }
        else
        {
            Console.WriteLine("Parsing: Mono-threaded" + Environment.NewLine);
            foreach (var operation in operationsToExecute)
            {
                ParseLog(operation, ProgramHelper);
            }
        }
        if (BatchToDiscord)
        {
            Console.WriteLine("Discord: Preparing batch for discord" + Environment.NewLine);
            ProgramHelper.HandleBatchedDiscordEmbed(DiscordMessageIDs, Operations, (message) =>
            {
                Console.WriteLine(message);
            });
        }
        Executing = false;
    }

    private class ThreadingState
    {
        public ProgramHelper ProgramHelper;
        public volatile bool NoMoreFiles;
        public ConcurrentQueue<ConsoleOperationController> FileQueue;
    }

    static void EnterParserThread(object state_)
    {
        var state = (ThreadingState)state_;
        while (true)
        {
            ConsoleOperationController operation;
            while (!state.FileQueue.TryDequeue(out operation))
            {
                if (state.NoMoreFiles && state.FileQueue.IsEmpty) { return; }
                //NOTE(Rennorb): Don't even bother with synchronizing. Just wait a bit.
                Thread.Sleep(10);
            }

            if (string.IsNullOrWhiteSpace(operation.InputFile)) { Debugger.Break(); }

            ParseLog(operation, state.ProgramHelper);
        }
    }

    private static void ParseLog(ConsoleOperationController operation, ProgramHelper programHelper)
    {
        using var _t = new AutoTrace("Parse One");
        try
        {
            programHelper.DoWork(operation);
            operation.FinalizeStatus(true, OperationController.FailureReason.NotApplicable);
        }
        catch (ProgramException prgEx)
        {
            var finalException = ParserHelper.GetFinalException(prgEx);
            OperationController.FailureReason reason = OperationController.GetReasonFromException(finalException);
            operation.UpdateProgress("Program: " + finalException.Source);
            operation.UpdateProgress("Program: " + finalException.StackTrace);
            operation.UpdateProgress("Program: " + finalException.TargetSite);
            operation.UpdateProgress("Program: " + finalException.Message);
            operation.FinalizeStatus(false, reason);
        }
        catch (Exception ex)
        {
            operation.UpdateProgress("Program: something terrible has happened");
            operation.FinalizeStatus(false, OperationController.GetReasonFromException(ex));
        }
        finally
        {
            programHelper.GenerateTraceFile(operation);
        }
        Console.WriteLine("Processed - " + JsonSerializer.Serialize(new ConsoleResultObject(operation), ConsoleResultObject.Serializer) + Environment.NewLine);
    }
}
