using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public class FormOperation : Operation
    {

        private CancellationTokenSource _cancelTokenSource;

        private Task _task;

        private readonly DataGridView _dgv;

        public FormOperation(string location, string status, DataGridView dgv) : base(location, status)
        {
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

        public override void ThrowIfCanceled()
        {
            if (_task != null && _cancelTokenSource.IsCancellationRequested)
            {
                Status = "Operation Aborted";
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

        public override void UpdateProgress(string status, int percent)
        {
            Status = status;
            Console.WriteLine($"{Location}: {status}" + Environment.NewLine);
            InvalidateDataView();
        }
        public void ToRunState()
        {
            ButtonText = "Cancel";
            State = OperationState.Parsing;
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
            State = OperationState.Complete;
            ButtonText = "Open";
            InvalidateDataView();
        }

        public void ToUnCompleteState()
        {
            State = OperationState.Ready;
            ButtonText = "Parse";
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
