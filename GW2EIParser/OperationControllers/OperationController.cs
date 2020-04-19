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
        ClearOnCancel = 5,
        Queued = 6,
    }

    public abstract class OperationController
    {
        /// <summary>
        /// Location of the file being parsed
        /// </summary>
        public string Location { get; }
        /// <summary>
        /// Location of the generated log
        /// </summary>
        public string LogLocation { get; set; }
        /// <summary>
        /// Status of the parse operation
        /// </summary>
        public string Status { get; protected set; }
        /// <summary>
        /// State of the button
        /// </summary>
        public string ButtonText { get; protected set; }
        /// <summary>
        /// Operation state
        /// </summary>
        public OperationState State { get; protected set; }

        public OperationController(string location, string status)
        {
            Location = location;
            Status = status;
            ButtonText = "Parse";
            State = OperationState.Ready;
        }

        public abstract void ThrowIfCanceled();

        public abstract void UpdateProgress(string status, int percent);
    }
}
