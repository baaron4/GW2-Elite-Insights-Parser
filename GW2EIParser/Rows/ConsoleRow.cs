using System;
using System.ComponentModel;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public class ConsoleRow : GridRow
    {

        public ConsoleRow(string location, string status) : base(location, status)
        {
            ButtonText = "";
        }

        /// <summary>
        /// Begins processing the log
        /// </summary>
        public override void Run()
        {
            State = RowState.Parsing;
        }

        /// <summary>
        /// Cancels the log's processing
        /// </summary>
        public override void Cancel()
        {
            State = RowState.Cancelling;
        }

        public override bool IsBusy()
        {
            return false;
        }

        public override void ThrowIfCanceled(string cancelStatus = "Canceled")
        {
        }

        public override void UpdateProgress(string status, int percent)
        {
            Status = status;
            Console.WriteLine($"{Location}: {status}" + Environment.NewLine);;
        }
    }
}
