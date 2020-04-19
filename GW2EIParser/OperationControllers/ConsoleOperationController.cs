using System;
using System.ComponentModel;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public class ConsoleOperationController : OperationController
    {

        public ConsoleOperationController(string location, string status) : base(location, status)
        {
        }

        public override void ThrowIfCanceled()
        {
        }

        public override void UpdateProgress(string status, int percent)
        {
            Status = status;
            Console.WriteLine($"{Location}: {status}" + Environment.NewLine);;
        }
    }
}
