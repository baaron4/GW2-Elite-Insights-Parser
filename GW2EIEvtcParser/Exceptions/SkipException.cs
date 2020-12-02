using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class SkipException : EIException
    {
        internal SkipException() : base("Option enabled - Failed logs are skipped")
        {
        }

    }
}
