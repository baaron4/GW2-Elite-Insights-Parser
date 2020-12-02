using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class IncompleteLogException : Exception
    {
        internal IncompleteLogException(string message) : base("Log incomplete - " + message)
        {
        }

    }
}
