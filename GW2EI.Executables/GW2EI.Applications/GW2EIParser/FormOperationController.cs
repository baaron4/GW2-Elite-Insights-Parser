using GW2EIParserCommons;
using static GW2EIParserWinForms.MainForm;

namespace GW2EIParserWinForms;

internal enum OperationState
{
    Ready = 0,
    Parsing = 1,
    Cancelling = 2,
    Complete = 3,
    Pending = 4,
    ClearOnCancel = 5,
    Queued = 6,
    UnComplete = 7,
    RemoveFromQueue = 8,
    RemoveFromQueueAndClear = 9,
}
internal sealed class FormOperationController : OperationController
{
    private CancellationTokenSource _cancelTokenSource;

    private readonly DataGridView _dgv;
    private readonly BindingSource _bdSrc;
    /// <summary>
    /// State of the button
    /// </summary>
    public string ButtonText { get; private set; }
    /// <summary>
    /// State of the reparse button
    /// </summary>
    public string ReParseText { get; private set; }
    /// <summary>
    /// Operation state
    /// </summary>
    public OperationState State { get; private set; }

    public bool IsRunning => State == OperationState.Parsing || State == OperationState.Queued || State == OperationState.Cancelling;
    public bool IsIdle => State == OperationState.UnComplete || State == OperationState.Complete || State == OperationState.Ready;
    public bool IsIdleOrPending => IsIdle || State == OperationState.Pending;

    public FormOperationController(string location, string status, DataGridView dgv, BindingSource bindingSource) : base(location, status)
    {
        ButtonText = "Parse";
        State = OperationState.Ready;
        _dgv = dgv;
        bindingSource.Add(this);
        _bdSrc = bindingSource;
        SetReparseButtonState(false);
    }

    protected override void ThrowIfCanceled()
    {
        if (_cancelTokenSource.IsCancellationRequested)
        {
            _cancelTokenSource.Token.ThrowIfCancellationRequested();
        }
    }

    private void InvalidateDataView()
    {
        if (_dgv.InvokeRequired)
        {
            _dgv.Invoke(new Action(() => _dgv.Invalidate()));
        }
        else
        {
            _dgv.Invalidate();
        }
    }

    private void SetReparseButtonState(bool onOff)
    {
        int rowIndex = _bdSrc.IndexOf(this);
        ReParseText = onOff ? "Re-Parse" : "N/A";
        if (rowIndex >= 0)
        {
            var reparseButton = (DataGridViewDisableButtonCell)_dgv.Rows[rowIndex].Cells["ReParseButtonState"];
            reparseButton.Enabled = onOff;
        }
    }

    public void ToRunState(CancellationTokenSource cancelTokenSource)
    {
        _cancelTokenSource = cancelTokenSource;
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
        State = OperationState.Parsing;
        ButtonText = "Cancel";
        SetReparseButtonState(false);
        Status = "Parsing";
        InvalidateDataView();
    }

    public void ToCancelState()
    {
        if (State == OperationState.Parsing)
        {
            State = OperationState.Cancelling;
            ButtonText = "Cancelling";
            _cancelTokenSource.Cancel();
        }
        else if (State == OperationState.Queued)
        {
            State = OperationState.RemoveFromQueue;
            Status = "Awaiting Removal from Queue";
        }
        SetReparseButtonState(false);
        InvalidateDataView();
    }
    public void ToCancelAndClearState()
    {
        if (State == OperationState.Parsing)
        {
            ToCancelState();
            State = OperationState.ClearOnCancel;
        }
        else if (State == OperationState.Queued)
        {
            ToCancelState();
            State = OperationState.RemoveFromQueueAndClear;
        }
        else if (State == OperationState.Cancelling)
        {
            State = OperationState.ClearOnCancel;
        }
    }
    public void ToReadyState()
    {
        State = OperationState.Ready;
        ButtonText = "Parse";
        SetReparseButtonState(false);
        Status = "Ready To Parse";
        InvalidateDataView();
    }

    public void ToCompleteState()
    {
        State = OperationState.Complete;
        ButtonText = "Open";
        SetReparseButtonState(true);
        FinalizeStatus(true, FailureReason.NotApplicable);
        InvalidateDataView();
    }

    public void ToUnCompleteState(FailureReason reason)
    {
        State = OperationState.UnComplete;
        ButtonText = "Parse";
        SetReparseButtonState(false);
        FinalizeStatus(false, reason);
        InvalidateDataView();
    }

    public void ToPendingState()
    {
        State = OperationState.Pending;
        ButtonText = "Cancel";
        SetReparseButtonState(false);
        Status = "Pending";
        InvalidateDataView();
    }

    public void ToQueuedState()
    {
        State = OperationState.Queued;
        ButtonText = "Cancel";
        SetReparseButtonState(false);
        Status = "Queued";
        InvalidateDataView();
    }
}
