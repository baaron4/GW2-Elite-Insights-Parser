using System;
using System.ComponentModel;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public class FormRow : GridRow
    {
        public BackgroundWorker BgWorker { get; }

        public FormRow(string location, string status) : base(location, status)
        {
            BgWorker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        }

        /// <summary>
        /// Begins processing the log
        /// </summary>
        public override void Run()
        {
            ButtonText = "Cancel";
            State = RowState.Parsing;
            BgWorker.RunWorkerAsync(this);
        }

        /// <summary>
        /// Cancels the log's processing
        /// </summary>
        public override void Cancel()
        {
            State = RowState.Cancelling;
            BgWorker.CancelAsync();
        }

        public override bool IsBusy()
        {
            return BgWorker.IsBusy;
        }

        public override void ThrowIfCanceled(string cancelStatus = "Canceled")
        {
            if (BgWorker.CancellationPending)
            {
                Status = cancelStatus;
                throw new CancellationException(this);
            }
        }

        public override void UpdateProgress(string status, int percent)
        {
            Status = status;
            BgWorker.ReportProgress(percent, this);
            Console.WriteLine($"{Location}: {status}" + Environment.NewLine);;
        }
    }
}
