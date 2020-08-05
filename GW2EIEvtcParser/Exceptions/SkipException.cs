using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class SkipException : Exception
    {
        internal SkipException() : base("Option enabled - Failed logs are skipped")
        {
        }

    }
}
