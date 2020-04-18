using System;
using System.ComponentModel;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public enum OperationState
    {
        Ready = 0,
        Parsing = 1,
        Cancelling = 2,
        Complete = 3,
        Pending = 4,
        ClearOnComplete = 5
    }

    public abstract class Operation
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
        /// Operation state
        /// </summary>
        public OperationState State { get; set; }

        public Operation(string location, string status)
        {
            Location = location;
            Status = status;
            ButtonText = "Parse";
            State = OperationState.Ready;
        }

        /// <summary>
        /// Begins processing the log
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Cancels the log's processing
        /// </summary>
        public abstract void Cancel();

        public abstract bool IsBusy();

        public abstract void ThrowIfCanceled(string cancelStatus = "Canceled");

        public abstract void UpdateProgress(string status, int percent);
    }
}
