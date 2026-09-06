using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser;
using GW2EIParserAvalonia.Services;
using GW2EIParserCommons;
using GW2EIParserCommons.Exceptions;

namespace GW2EIParserAvalonia.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private ObservableCollection<LogFileViewModel> logFiles = [];
    [ObservableProperty]
    private string queueStatus = "Waiting";
    [ObservableProperty]
    private bool parseEnabled = false;
    [ObservableProperty]
    private bool cancelAllEnabled = false;
    [ObservableProperty]
    private bool clearAllEnabled = false;
    [ObservableProperty]
    private bool clearUncompletedEnabled = false;
    [ObservableProperty]
    private bool discordBatchEnabled = true;
    [ObservableProperty]
    private bool autoDiscordBatchEnabled = true;
    [ObservableProperty]
    private bool checkUpdatesEnabled = true;
    [ObservableProperty]
    private bool settingsEnabled = true;
    [ObservableProperty]
    private bool autoDiscordBatch;
    [ObservableProperty]
    private string watchingDirectory = string.Empty;
    [ObservableProperty]
    private bool watchingDirectoryVisible;
    [ObservableProperty]
    private bool logTracesVisible;
    [ObservableProperty]
    private string version = string.Empty;
    public readonly ParserService _parserService;
    private readonly Queue<LogFileViewModel> _logQueue = new();
    private readonly IApplicationTrace _trace;
    private int _runningCount;
    public ProgramSettings Settings { get; }
    public SettingsViewModel SettingsViewModel { get; }
    public bool AnyRunning => _runningCount > 0;

    public MainWindowViewModel(IApplicationTrace trace, CommandLineOptions? commandLineOptions)
    {
        _trace = trace;

        Settings = CustomSettingsManager.GetProgramSettings();
        _parserService = new ParserService(Settings);

        SettingsViewModel = new SettingsViewModel(Settings);
        SettingsViewModel.LoadFromSettings();
        SettingsViewModel.PropertyChanged += SettingsViewModel_SettingsApplied;

        AutoDiscordBatch = SettingsViewModel.AutoDiscordBatch;
        LogTracesVisible = SettingsViewModel.SaveOutTrace;

        Version = $"v{Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty}";

        if (commandLineOptions != null && commandLineOptions.LogFiles.Count > 0)
        {
            foreach (var file in commandLineOptions.LogFiles)
            {
                AddFile(file);
            }
        }

        UpdateWatchDirectory();
        UpdateButtonStates();
    }

    private void SettingsViewModel_SettingsApplied(object? sender, EventArgs e)
    {
        SettingsViewModel.ApplyToSettings();
        AutoDiscordBatch = SettingsViewModel.AutoDiscordBatch;
        LogTracesVisible = SettingsViewModel.SaveOutTrace;

        UpdateWatchDirectory();
    }

    public void AddFile(string path)
    {
        if (LogFiles.Any(x => string.Equals(x.InputFilePath, path, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var logFileViewModel = new LogFileViewModel(path);

        logFileViewModel.ParseRequested += LogFile_ParseRequested;
        logFileViewModel.ReParseRequested += LogFile_ParseRequested;
        logFileViewModel.PendingCancellationRequested += LogFile_PendingCancellationRequested;
        logFileViewModel.RemoveRequested += LogFile_RemoveRequested;
        logFileViewModel.InspectLogRequested += LogFile_InspectLogRequested;

        LogFiles.Add(logFileViewModel);
        UpdateQueueStatus();
        _trace.Add("UI: Added " + logFileViewModel.InputFilePath);

        ParseEnabled = !AnyRunning;
        ClearAllEnabled = true;
        ClearUncompletedEnabled = LogFiles.Any(x => x.State == OperationState.UnComplete);

        if (SettingsViewModel.AutoParse)
        {
            QueueOrRunOperation(logFileViewModel);
        }
        UpdateButtonStates();
    }

    private void LogFile_ParseRequested(object? sender, EventArgs e)
    {
        if (sender is LogFileViewModel logFile)
        {
            QueueOrRunOperation(logFile);
        }
    }

    public void ParseAll()
    {
        _logQueue.Clear();

        if (LogFiles.Count == 0)
        {
            return;
        }

        UpdateButtonStates();

        foreach (var logFile in LogFiles)
        {
            if (logFile.Operation.IsIdle)
            {
                QueueOrRunOperation(logFile);
            }
        }
    }

    public void CancelAll()
    {
        var queued = new HashSet<LogFileViewModel>(_logQueue);

        _logQueue.Clear();

        foreach (var logFile in LogFiles)
        {
            if (logFile.IsRunning)
            {
                logFile.Operation.ToCancelState();
                UpdateQueueStatus();
            }
            else if (logFile.IsIdleOrPending && queued.Contains(logFile))
            {
                logFile.Operation.ToReadyState();
                UpdateQueueStatus();
            }
        }

        UpdateButtonStates();
    }

    public void ClearAll()
    {
        _logQueue.Clear();

        for (var i = LogFiles.Count - 1; i >= 0; i--)
        {
            var logFile = LogFiles[i];

            if (logFile.IsRunning)
            {
                logFile.Operation.ToCancelAndClearState();
                UpdateQueueStatus();
            }
            else if (logFile.IsIdleOrPending)
            {
                LogFiles.RemoveAt(i);
                UpdateQueueStatus();
            }
        }

        UpdateButtonStates();
    }

    public void ClearUncompleted()
    {
        for (var i = LogFiles.Count - 1; i >= 0; i--)
        {
            var logFile = LogFiles[i];

            if (logFile.State == OperationState.UnComplete)
            {
                LogFiles.RemoveAt(i);
            }
        }

        UpdateButtonStates();
    }

    public void AddFilesFromDirectory(string path)
    {
        var files = ProgramHelper.FetchSupportedFormatsFrom(path, SettingsViewModel.PopulateHourLimit, DateTime.Now);

        foreach (var file in files)
        {
            AddFile(file);
        }
    }

    public void UpdateWatchDirectory()
    {
        if (SettingsViewModel.AutoAdd && Directory.Exists(SettingsViewModel.AutoAddPath))
        {
            WatchingDirectory = $"Watching for log files in '{SettingsViewModel.AutoAddPath}'";
            WatchingDirectoryVisible = true;

            _trace.Add("Settings: Updated watch directory to " + SettingsViewModel.AutoAddPath);
        }
        else
        {
            WatchingDirectory = string.Empty;
            WatchingDirectoryVisible = false;
        }
    }

    public void HandleCreatedFile(string path)
    {
        if (!ProgramHelper.IsSupportedFormat(path))
        {
            return;
        }

        _ = AddDelayed(path);
    }

    public void HandleRenamedFile(string oldPath, string newPath)
    {
        if (ProgramHelper.IsTemporaryCompressedFormat(oldPath) && ProgramHelper.IsCompressedFormat(newPath))
        {
            _trace.Add("File Watcher: renamed " + oldPath + " to " + newPath);
            _ = AddDelayed(newPath);
        }
        else if (ProgramHelper.IsTemporaryFormat(oldPath) && ProgramHelper.IsSupportedFormat(newPath))
        {
            _trace.Add("File Watcher: adding " + newPath);
            _ = AddDelayed(newPath);
        }
    }

    private async Task AddDelayed(string path)
    {
        await Task.Delay(3000);

        if (File.Exists(path))
        {
            _trace.Add("File Watcher: adding " + path);
            AddFile(path);
        }
    }

    private void QueueOrRunOperation(LogFileViewModel logFile)
    {
        if (!AnyRunning || (_parserService.ParseMultipleLogs() && _runningCount < _parserService.GetMaxParallelRunning()))
        {
            _ = RunOperationAsync(logFile);
        }
        else
        {
            _logQueue.Enqueue(logFile);
            logFile.Operation.ToPendingState();
            UpdateQueueStatus();
        }
        UpdateButtonStates();
    }

    private async Task RunOperationAsync(LogFileViewModel logFile)
    {
        _parserService.ExecuteMemoryCheckTask();
        _runningCount++;

        logFile.Operation.ToQueuedState();
        UpdateQueueStatus();
        _trace.Add("Operation: Queued " + logFile.InputFilePath);

        try
        {
            await _parserService.ParseAsync(logFile.Operation, () =>
            {
                UpdateQueueStatus();
                _trace.Add("Operation: Parsing " + logFile.InputFilePath);
            });

            if (logFile.State != OperationState.ClearOnCancel)
            {
                logFile.Operation.ToCompleteState();
                _trace.Add("Operation: Parsed " + logFile.InputFilePath);
            }
        }
        catch (OperationCanceledException)
        {
            logFile.Operation.UpdateProgress("Program: Operation Aborted");
            if (logFile.State != OperationState.ClearOnCancel)
            {
                logFile.Operation.ToUnCompleteState(OperationController.FailureReason.User);
            }
        }
        catch (Exception ex)
        {
            OperationController.FailureReason reason = OperationController.GetReasonFromException(ex);
            HandleOperationException(logFile.Operation, ex);
            if (logFile.State != OperationState.ClearOnCancel)
            {
                logFile.Operation.ToUnCompleteState(reason);
            }
        }
        finally
        {
            _runningCount--;

            if (logFile.State == OperationState.ClearOnCancel)
            {
                LogFiles.Remove(logFile);
                UpdateQueueStatus();
            }
            else
            {
                UpdateQueueStatus();
            }

            _parserService.GenerateTraceFile(logFile.Operation);

            if (logFile.State != OperationState.Complete)
            {
                logFile.Operation.ResetContent();
                UpdateQueueStatus();
            }

            RunNextOperation();
        }
    }

    private void RunNextOperation()
    {
        if (_logQueue.Count > 0 && (_parserService.ParseMultipleLogs() || !AnyRunning))
        {
            _ = RunOperationAsync(_logQueue.Dequeue());
            return;
        }

        UpdateButtonStates();
    }

    private void LogFile_PendingCancellationRequested(object? sender, EventArgs e)
    {
        if (sender is not LogFileViewModel logFile)
        {
            return;
        }

        var remainingOperations = _logQueue.Where(x => !ReferenceEquals(x, logFile)).ToList();
        _logQueue.Clear();

        foreach (var operation in remainingOperations)
        {
            _logQueue.Enqueue(operation);
        }

        logFile.Operation.ToReadyState();
        UpdateQueueStatus();
    }

    private void LogFile_RemoveRequested(object? sender, EventArgs e)
    {
        if (sender is not LogFileViewModel logFile || !logFile.RemoveEnabled)
        {
            return;
        }

        LogFiles.Remove(logFile);
        UpdateQueueStatus();
        _trace.Add("UI: Removed " + logFile.InputFilePath);
        ClearAllEnabled = LogFiles.Count > 0;
        ClearUncompletedEnabled = LogFiles.Any(x => x.State == OperationState.UnComplete);
        ParseEnabled = !AnyRunning && LogFiles.Count > 0;
    }

    private void LogFile_InspectLogRequested(object? sender, EventArgs e)
    {
        if (sender is LogFileViewModel logFile)
        {
            InspectorParse(logFile);
        }
    }

    private async void InspectorParse(LogFileViewModel logFile)
    {
        try
        {
            await _parserService.InspectAsync(logFile.InspectorOperation, () =>
            {
                _trace.Add("Operation: Inspecting " + logFile.InputFilePath);
            });
        }
        finally
        {
            var inspectorWindow = new InspectorWindow(_parserService.ParsedLog!, _trace);
            inspectorWindow.Show();
        }
        
    }

    public async Task<string> SendAllToDiscordAsync()
    {
        _trace.Add("UI: Manual Discord Batch");

        DiscordBatchEnabled = false;

        try
        {
            var ids = new List<ulong>();
            var operations = LogFiles.Select(x => (OperationController)x.Operation).ToList();

            return await Task.Run(() =>
                _parserService.HandleBatchedDiscordEmbed(ids, operations, _trace.Add));
        }
        finally
        {
            DiscordBatchEnabled = !AnyRunning;
        }
    }

    public void UpdateVersionLabel(bool isAvailable)
    {
        Version = isAvailable ? Version + " (Update Available)" : Version;
    }

    private void UpdateQueueStatus()
    {
        var status = new List<string>();

        var queued = LogFiles.Count(x => x.State is OperationState.Pending or OperationState.Queued);
        var parsing = LogFiles.Count(x => x.State == OperationState.Parsing);
        var completed = LogFiles.Count(x => x.State == OperationState.Complete);

        if (queued > 0)
        {
            status.Add($"{queued} log(s) queued");
        }
        if (parsing > 0)
        {
            status.Add($"{parsing} log(s) parsing");
        }
        if (completed > 0)
        {
            status.Add($"{completed} log(s) completed");
        }

        QueueStatus = status.Count > 0 ? string.Join(", ", status) : "Waiting";
    }

    private void UpdateButtonStates()
    {
        bool hasLogs = LogFiles.Count > 0;
        bool hasUncompleted = LogFiles.Any(x => x.State == OperationState.UnComplete);
        bool running = AnyRunning;
        bool queued = _logQueue.Count > 0;

        ParseEnabled = hasLogs && !running;
        CancelAllEnabled = running || queued;
        ClearAllEnabled = hasLogs;
        ClearUncompletedEnabled = hasUncompleted;

        DiscordBatchEnabled = !running;
        AutoDiscordBatchEnabled = !running;
        CheckUpdatesEnabled = !running;
    }

    private static void HandleOperationException(OperationController operation, Exception exception)
    {
        if (exception is not ProgramException)
        {
            operation.UpdateProgress("Program: something terrible has happened");
        }

        if (exception is OperationCanceledException)
        {
            operation.UpdateProgress("Program: operation Aborted");
            return;
        }

        var finalException = ParserHelper.GetFinalException(exception);

        operation.UpdateProgress("Program: " + finalException.Source);
        operation.UpdateProgress("Program: " + finalException.StackTrace);
        operation.UpdateProgress("Program: " + finalException.TargetSite);
        operation.UpdateProgress("Program: " + finalException.Message);
    }

    public void Dispose()
    {
        _parserService.Dispose();
    }
}
