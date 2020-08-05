using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class TooLongException : Exception
    {
        internal TooLongException() : base("Fight is took longer than 24h - may be a broken evtc")
        {
        }

    }
}
