using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GW2EIParser
{
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
    }
    internal class FormOperationController : OperationController
    {

        private CancellationTokenSource _cancelTokenSource;

        private Task _task;

        private readonly DataGridView _dgv;
        /// <summary>
        /// State of the button
        /// </summary>
        public string ButtonText { get; protected set; }
        /// <summary>
        /// Operation state
        /// </summary>
        public OperationState State { get; protected set; }

        public FormOperationController(Version parserVersion, string location, string status, DataGridView dgv) : base(parserVersion, location, status)
        {
            ButtonText = "Parse";
            State = OperationState.Ready;
            _dgv = dgv;
        }

        public void SetContext(CancellationTokenSource cancelTokenSource, Task task)
        {
            _cancelTokenSource = cancelTokenSource;
            _task = task;
        }

        public bool IsBusy()
        {
            if (_task != null)
            {
                return !_task.IsCompleted;
            }
            return false;
        }

        protected override void ThrowIfCanceled()
        {
            if (_task != null && _cancelTokenSource.IsCancellationRequested)
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

        public void ToRunState()
        {
            ButtonText = "Cancel";
            State = OperationState.Parsing;
            Status = "Parsing";
            InvalidateDataView();
        }

        public void ToCancelState()
        {
            if (_task == null)
            {
                return;
            }
            State = OperationState.Cancelling;
            ButtonText = "Cancelling";
            _cancelTokenSource.Cancel();
            InvalidateDataView();
        }
        public void ToRemovalFromQueueState()
        {
            ToCancelState();
            Status = "Awaiting Removal from Queue";
            InvalidateDataView();
        }
        public void ToCancelAndClearState()
        {
            ToCancelState();
            State = OperationState.ClearOnCancel;
        }
        public void ToReadyState()
        {
            State = OperationState.Ready;
            ButtonText = "Parse";
            Status = "Ready To Parse";
            InvalidateDataView();
        }

        public void ToCompleteState()
        {
            State = OperationState.Ready;
            ButtonText = "Parse";
            FinalizeStatus("Parsing Successful - ");
            InvalidateDataView();
        }

        public void ToUnCompleteState()
        {
            State = OperationState.UnComplete;
            ButtonText = "Parse";
            FinalizeStatus("Parsing Failure - ");
            InvalidateDataView();
        }

        public void ToPendingState()
        {
            State = OperationState.Pending;
            ButtonText = "Cancel";
            Status = "Pending";
            InvalidateDataView();
        }

        public void ToQueuedState()
        {
            State = OperationState.Queued;
            ButtonText = "Cancel";
            Status = "Queued";
            InvalidateDataView();
        }
    }
}
