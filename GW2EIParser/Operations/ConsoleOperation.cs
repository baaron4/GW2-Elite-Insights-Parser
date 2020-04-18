using System;
using System.ComponentModel;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public class ConsoleOperation : Operation
    {

        public ConsoleOperation(string location, string status) : base(location, status)
        {
            ButtonText = "";
        }

        /// <summary>
        /// Begins processing the log
        /// </summary>
        public override void Run()
        {
            State = OperationState.Parsing;
            ProgramHelper.DoWork(this);
        }

        /// <summary>
        /// Cancels the log's processing
        /// </summary>
        public override void Cancel()
        {
            State = OperationState.Cancelling;
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
