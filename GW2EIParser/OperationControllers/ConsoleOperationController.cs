using System;
using System.ComponentModel;
using System.Linq;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public class ConsoleOperationController : OperationController
    {

        public ConsoleOperationController(string location, string status) : base(location, status)
        {
        }
    }
}
