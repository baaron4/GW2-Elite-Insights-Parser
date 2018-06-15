using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuckParser
{
    public class GridRow
    {
        public const string STATUS_PARSE = "Parse";
        public const string STATUS_CANCEL = "Cancel";
        public const string STATUS_OPEN = "Open";

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
        public string ButtonState { get; set; }
        /// <summary>
        /// BackgroundWorker processing the log
        /// </summary>
        public BackgroundWorker BgWorker { get; set; }

        public GridRow(string location, string status)
        {
            Location = location;
            Status = status;
            ButtonState = "Parse";
        }

        /// <summary>
        /// Begins processing the log
        /// </summary>
        public void Run()
        {
            ButtonState = STATUS_CANCEL;
            BgWorker.RunWorkerAsync(this);
        }

        /// <summary>
        /// Cancels the log's processing
        /// </summary>
        public void Cancel()
        {
            Status = "Cancelling...";
            BgWorker.CancelAsync();
        }
    }
}
