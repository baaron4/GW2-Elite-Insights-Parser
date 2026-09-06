using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2EIParserAvalonia.Services;

namespace GW2EIParserAvalonia.ViewModels;

public partial class LogFileViewModel : ObservableObject
{
    [ObservableProperty]
    private string inputFilePath;
    [ObservableProperty]
    private string logStatus;
    [ObservableProperty]
    private string buttonText;
    [ObservableProperty]
    private string reParseText;
    [ObservableProperty]
    private bool removeEnabled;
    [ObservableProperty]
    private bool reParseEnabled;
    [ObservableProperty]
    private bool logTracesEnabled;
    [ObservableProperty]
    private long elapsed;
    [ObservableProperty]
    private OperationState state;
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
        inputFilePath = fullPath;
        Operation = new AvaloniaOperationController(inputFilePath, this);
        InspectorOperation = new InspectorOperationController(inputFilePath);
        logStatus = Operation.Status;
        buttonText = Operation.ButtonText;
        reParseText = Operation.ReParseText;
        elapsed = Operation.Elapsed;
        state = Operation.State;
        removeEnabled = true;
        reParseEnabled = Operation.ReParseEnabled;
        logTracesEnabled = Operation.LogTracesEnabled;
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
