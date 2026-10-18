using System;
using System.Collections.Generic;
using System.Threading;
using GW2EIParserAvalonia.ViewModels;
using GW2EIParserCommons;

namespace GW2EIParserAvalonia.Services;

#pragma warning disable CA1001
internal sealed class AvaloniaOperationController : OperationController
{
    public string ButtonText { get; private set; } = "Parse";
    public string ReParseText { get; private set; } = "N/A";
    public bool ReParseEnabled { get; private set; } = false;
    public bool LogTracesEnabled { get; private set; } = false;
    public OperationState State { get; private set; } = OperationState.Ready;

    public bool IsRunning => State == OperationState.Parsing || State == OperationState.Queued || State == OperationState.Cancelling;
    public bool IsIdle => State == OperationState.UnComplete || State == OperationState.Complete || State == OperationState.Ready;
    public bool IsIdleOrPending => IsIdle || State == OperationState.Pending;

    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly LogFileViewModel _logFileViewModel;
    public event EventHandler? ProgressUpdated;

    private static readonly Dictionary<OperationState, (string ActionButton, string ReParseButton, bool ReParseEnabled, bool LogTraces)> _states = new()
    {
        [OperationState.Ready] = ("Parse", "N/A", false, false),
        [OperationState.Queued] = ("Cancel", "N/A", false, false),
        [OperationState.Pending] = ("Cancel", "N/A", false, false),
        [OperationState.Parsing] = ("Cancel", "N/A", false, false),
        [OperationState.Cancelling] = ("Cancelling", "N/A", false, false),
        [OperationState.ClearOnCancel] = ("Cancelling", "N/A", false, false),
        [OperationState.Complete] = ("Open", "Re-Parse", true, true),
        [OperationState.UnComplete] = ("Parse", "N/A", false, false),
        [OperationState.RemoveFromQueue] = ("Removing From Queue", "N/A", false, false),
        [OperationState.RemoveFromQueueAndClear] = ("Removing From Queue", "N/A", false, false),
    };

    public AvaloniaOperationController(string location, LogFileViewModel logFileViewModel) : base(location, "Ready to parse")
    {
        _logFileViewModel = logFileViewModel;
    }

    private void SetState(OperationState operationState)
    {
        State = operationState;
        var dictState = _states[operationState];

        ButtonText = dictState.ActionButton;
        ReParseText = dictState.ReParseButton;
        ReParseEnabled = dictState.ReParseEnabled;
        LogTracesEnabled = dictState.LogTraces;

        _logFileViewModel.UpdateFromOperation();
    }

    public void ToReadyState()
    {
        SetState(OperationState.Ready);
    }

    public void ToQueuedState()
    {
        SetState(OperationState.Queued);
    }

    public void ToPendingState()
    {
        SetState(OperationState.Pending);
    }

    public void ToRunState(CancellationTokenSource cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
        // If removal requested, immediately cancel execution
        if (State == OperationState.RemoveFromQueue)
        {
            State = OperationState.Parsing;
            ToCancelState();
            return;
        }
        else if (State == OperationState.RemoveFromQueueAndClear)
        {
            State = OperationState.Parsing;
            ToCancelAndClearState();
            return;
        }

        SetState(OperationState.Parsing);
        UpdateProgress("Parsing");
    }

    public void ToCancelState()
    {
        if (State == OperationState.Parsing)
        {
            SetState(OperationState.Cancelling);
            _cancellationTokenSource.Cancel();
        }
        else if (State == OperationState.Queued)
        {
            SetState(OperationState.RemoveFromQueue);
        }
    }

    public void ToCancelAndClearState()
    {
        if (State == OperationState.Parsing)
        {
            ToCancelState();
            SetState(OperationState.ClearOnCancel);
        }
        else if (State == OperationState.Queued)
        {
            ToCancelState();
            SetState(OperationState.RemoveFromQueueAndClear);
        }
        else if (State == OperationState.Cancelling)
        {
            SetState(OperationState.ClearOnCancel);
        }
    }

    public void ToCompleteState()
    {
        SetState(OperationState.Complete);
        FinalizeStatus(true, FailureReason.NotApplicable);
    }

    public void ToUnCompleteState(FailureReason reason)
    {
        SetState(OperationState.UnComplete);
        FinalizeStatus(false, reason);
    }

    public override void FinalizeStatus(bool parsed, FailureReason reason)
    {
        base.FinalizeStatus(parsed, reason);
        _logFileViewModel.UpdateFromOperation();
    }

    protected override void ThrowIfCanceled()
    {
        if (_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
        }
    }

    public override void ResetContent()
    {
        base.ResetContent();
        _logFileViewModel.UpdateFromOperation();
    }

    public override void ResetState()
    {
        base.ResetState();
        _logFileViewModel.UpdateFromOperation();
    }

    public override void UpdateProgress(string status)
    {
        base.UpdateProgress(status);
        Status = status;
        ProgressUpdated?.Invoke(this, EventArgs.Empty);
    }
}

#pragma warning restore CA1001
