using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class IncompleteLogException : Exception
    {
        internal IncompleteLogException() : base("Log incomplete")
        {
        }

    }
}
