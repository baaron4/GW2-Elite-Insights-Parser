using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class MissingKeyActorsException : EIFatalException
    {
        internal MissingKeyActorsException(string message) : base(message)
        {
        }

    }
}
