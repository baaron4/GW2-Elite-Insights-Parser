using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class IncompleteLogException : EIException
    {
        internal IncompleteLogException(string message) : base("Log incomplete - " + message)
        {
        }

    }
}
