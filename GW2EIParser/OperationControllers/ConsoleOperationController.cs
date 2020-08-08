using System;

namespace GW2EIParser
{
    internal class ConsoleOperationController : OperationController
    {

        public ConsoleOperationController(string parserName, Version parserVersion, string location, string status) : base(parserName, parserVersion, location, status)
        {
        }
    }
}
