using System.ComponentModel;

namespace LuckParser
{
    public enum RowState
    {
        Ready = 0,
        Parsing = 1,
        Cancelling = 2,
        Complete = 3,
        Pending = 4,
        ClearOnComplete = 5
    }

    public class RowData
    {
        /// <summary>
        /// Whether we're running in the form, or via console
        /// </summary>
        public bool FromConsole { get; set; }
        public RowState State { get; set; }
    }

    public class GridRow
    {
        /// <summary>
        /// Location of the file being parsed
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Location of the generated log
        /// </summary>
        public string LogLocation { get; set; }
        /// <summary>
        /// Status of the parse operation
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// State of the button
        /// </summary>
        public string ButtonText { get; set; }
        /// <summary>
        /// BackgroundWorker processing the log
        /// </summary>
        public BackgroundWorker BgWorker { get; set; }
        /// <summary>
        /// Contains row state and other data
        /// </summary>
        public RowData Metadata { get; set; } = new RowData();

        public GridRow(string location, string status)
        {
            Location = location;
            Status = status;
            ButtonText = "Parse";
            Metadata.State = RowState.Ready;
        }

        /// <summary>
        /// Begins processing the log
        /// </summary>
        public void Run()
        {
            ButtonText = "Cancel";
            Metadata.State = RowState.Parsing;
            BgWorker.RunWorkerAsync(this);
        }

        /// <summary>
        /// Cancels the log's processing
        /// </summary>
        public void Cancel()
        {
            Status = "Cancelling...";
            Metadata.State = RowState.Cancelling;
            BgWorker.CancelAsync();
        }

        public void UpdateProgress(string status, int percent)
        {
            BgWorker.UpdateProgress(this, status, percent);
        }
    }
}
