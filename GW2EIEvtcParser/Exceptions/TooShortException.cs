using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class TooShortException : Exception
    {
        internal TooShortException() : base("Fight is too short")
        {
        }

    }
}
