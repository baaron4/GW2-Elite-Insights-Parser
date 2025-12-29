using System.Collections.Concurrent;
using System.Diagnostics;
using GW2EIEvtcParser;
using GW2EIParserCommons;
using GW2EIParserCommons.Exceptions;
using Tracing;

namespace GW2EIParser;

static class ConsoleProgram
{

    /// <returns>0 on success, other value on error</returns>
    public static int ParseAll(List<string> logFiles, ProgramHelper programHelper)
    {
        using var _t = new AutoTrace("ParseAll");
        if (programHelper.ParseMultipleLogs())
        {
            var state = new ThreadingState()
            {
                ProgramHelper = programHelper,
                NoMoreFiles = false,
                FileQueue = new(),
            };

            var parallelism = programHelper.GetMaxParallelRunning();
            for(int i = 0; i < parallelism - 1; i++)
            {
                var t = new Thread(EnterParserThread);
                t.Start(state);
            }

            foreach(var file in logFiles)
            {
                state.FileQueue.Enqueue(file);
            }

            state.NoMoreFiles = true;
            EnterParserThread(state); // we take the last thread
        }
        else
        {
            foreach (string file in logFiles)
            {
                ParseLog(file, programHelper);
            }
        }

        return 0;
    }

    public class ThreadingState
    {
        public ProgramHelper ProgramHelper;
        public volatile bool NoMoreFiles;
        public ConcurrentQueue<string> FileQueue;
    }

    static void EnterParserThread(object state_)
    {
        var state = (ThreadingState)state_;
        while (true)
        {
            string logFile;
            while(!state.FileQueue.TryDequeue(out logFile)) {
                if(state.NoMoreFiles && state.FileQueue.IsEmpty) { return; }
                //NOTE(Rennorb): Don't even bother with synchronizing. Just wait a bit.
                Thread.Sleep(10);
            }

            if(string.IsNullOrWhiteSpace(logFile)) { Debugger.Break(); }

            ParseLog(logFile, state.ProgramHelper);
        }
    }

    private static void ParseLog(string logFile, ProgramHelper programHelper)
    {
        using var _t = new AutoTrace("Parse One");
        programHelper.ExecuteMemoryCheckTask();
        var operation = new ConsoleOperationController(logFile);
        try
        {
            programHelper.DoWork(operation);
            operation.FinalizeStatus("Parsing Successful - ");
        }
        catch (ProgramException ex)
        {
            var finalException = ParserHelper.GetFinalException(ex);
            operation.UpdateProgress("Program: " + finalException.Source);
            operation.UpdateProgress("Program: " + finalException.StackTrace);
            operation.UpdateProgress("Program: " + finalException.TargetSite);
            operation.UpdateProgress("Program: " + finalException.Message);
            operation.FinalizeStatus("Parsing Failure - ");
        }
        catch (Exception)
        {
            operation.UpdateProgress("Program: something terrible has happened");
            operation.FinalizeStatus("Parsing Failure - ");
        }
        finally
        {
            programHelper.GenerateTraceFile(operation);
        }
        GC.Collect();
    }
}
