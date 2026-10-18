using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2EIParserAvalonia.Services;

namespace GW2EIParserAvalonia.ViewModels;

internal partial class LogFileViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string InputFilePath { get; set; }

    [ObservableProperty]
    public partial string LogStatus { get; set; }

    [ObservableProperty]
    public partial string ButtonText { get; set; }

    [ObservableProperty]
    public partial string ReParseText { get; set; }

    [ObservableProperty]
    public partial bool RemoveEnabled { get; set; }

    [ObservableProperty]
    public partial bool ReParseEnabled { get; set; }

    [ObservableProperty]
    public partial bool LogTracesEnabled { get; set; }

    [ObservableProperty]
    public partial long Elapsed { get; set; }

    [ObservableProperty]
    public partial OperationState State { get; set; }
    public AvaloniaOperationController Operation { get; }
    public InspectorOperationController InspectorOperation { get; }
    public event EventHandler? ParseRequested;
    public event EventHandler? ReParseRequested;
    public event EventHandler? PendingCancellationRequested;
    public event EventHandler? RemoveRequested;
    public event EventHandler? InspectLogRequested;

    public bool IsRunning => Operation.IsRunning;
    public bool IsIdle => Operation.IsIdle;
    public bool IsIdleOrPending => Operation.IsIdleOrPending;

    public LogFileViewModel(string fullPath)
    {
        InputFilePath = fullPath;
        Operation = new AvaloniaOperationController(InputFilePath, this);
        InspectorOperation = new InspectorOperationController(InputFilePath);
        LogStatus = Operation.Status;
        ButtonText = Operation.ButtonText;
        ReParseText = Operation.ReParseText;
        Elapsed = Operation.Elapsed;
        State = Operation.State;
        RemoveEnabled = true;
        ReParseEnabled = Operation.ReParseEnabled;
        LogTracesEnabled = Operation.LogTracesEnabled;
        Operation.ProgressUpdated += Operation_ProgressUpdated;
    }

    [RelayCommand]
    private void Action()
    {
        switch (State)
        {
            case OperationState.Ready:
            case OperationState.UnComplete:
                ParseRequested?.Invoke(this, EventArgs.Empty);
                break;
            case OperationState.Parsing:
            case OperationState.Queued:
                Operation.ToCancelState();
                break;
            case OperationState.Pending:
                PendingCancellationRequested?.Invoke(this, EventArgs.Empty);
                break;
            case OperationState.Complete:
                OpenGeneratedFiles(Operation.OpenableFiles);
                break;
        }
    }

    [RelayCommand]
    private void ReParse()
    {
        if (State != OperationState.Complete)
        {
            return;
        }

        ReParseRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Remove()
    {
        if (!RemoveEnabled)
        {
            return;
        }

        RemoveRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    public void OpenTracesCommand()
    {
        OpenGeneratedFiles(Operation.OpenableLogTracesFiles);
    }

    [RelayCommand]
    public void InspectLogCommand()
    {
        InspectLogRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OpenGeneratedFiles(IReadOnlyList<string> paths)
    {
        foreach (string path in paths)
        {
            if (File.Exists(path))
            {
                OpenWithDefaultApplication(path);
            }
        }

        if (paths.Count < Operation.GeneratedFiles.Count && Operation.OutLocation != null && Directory.Exists(Operation.OutLocation))
        {
            OpenWithDefaultApplication(Operation.OutLocation);
        }
    }

    private static void OpenWithDefaultApplication(string path)
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
    }

    public void UpdateFromOperation()
    {
        LogStatus = Operation.Status;
        ButtonText = Operation.ButtonText;
        ReParseText = Operation.ReParseText;
        State = Operation.State;
        ReParseEnabled = Operation.ReParseEnabled;
        LogTracesEnabled = Operation.LogTracesEnabled;
        Elapsed = Operation.Elapsed;
        RemoveEnabled = Operation.IsIdle;
    }

    private void Operation_ProgressUpdated(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            LogStatus = Operation.Status;
        });
    }
}
