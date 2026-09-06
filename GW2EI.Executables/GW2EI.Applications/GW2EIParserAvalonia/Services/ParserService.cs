using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GW2EIEvtcParser;
using GW2EIParserCommons;
using static GW2EIParserCommons.ProgramHelper;

namespace GW2EIParserAvalonia.Services;

public sealed class ParserService : IDisposable
{
    private readonly ProgramHelper _programHelper;
    public ProgramSettings Settings { get; }

    public ParserService(ProgramSettings settings)
    {
        Settings = settings;
        var version = typeof(App).Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        _programHelper = new ProgramHelper(version, Settings);
    }

    public delegate void OnTaskRun();

    public async Task ParseAsync(AvaloniaOperationController operation, OnTaskRun onTaskRun)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Run(() => {
                operation.ToRunState(cancellationTokenSource);
                onTaskRun();
                _programHelper.DoWork(operation);
            }, cancellationTokenSource.Token);
        }
        finally
        {
            cancellationTokenSource.Dispose();
        }
    }

    public async Task InspectParseAsync(InspectorOperationController operation, OnTaskRun onTaskRun)
    {
        if (operation.InspectLog != null || operation.Errored)
        {
            return;
        }
        var cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Run(() => {
                onTaskRun();
                operation.InspectLog = _programHelper.ParseLogForInspection(operation);
            }, cancellationTokenSource.Token)
                .ContinueWith(t => 
                {
                    if (t.IsFaulted)
                    {
                        operation.InspectLog = null;
                        operation.Errored = true;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        finally
        {
            cancellationTokenSource.Dispose();
        }
    }

    public bool ParseMultipleLogs()
    {
        return _programHelper.ParseMultipleLogs();
    }

    public int GetMaxParallelRunning()
    {
        return _programHelper.GetMaxParallelRunning();
    }

    public void ExecuteMemoryCheckTask()
    {
        _programHelper.ExecuteMemoryCheckTask();
    }

    public void GenerateTraceFile(AvaloniaOperationController operation)
    {
        _programHelper.GenerateTraceFile(operation);
    }

    public string HandleBatchedDiscordEmbed(List<ulong> ids, List<OperationController> operations, BatchedDiscordTraceHandler traceHandler)
    {
        return _programHelper.HandleBatchedDiscordEmbed(ids, operations, traceHandler);
    }

    public void Dispose()
    {
        _programHelper.Dispose();
    }
}
