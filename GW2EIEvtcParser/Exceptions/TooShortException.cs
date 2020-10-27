using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class TooShortException : Exception
    {
        internal TooShortException(long shortnessValue, long minValue) : base("Fight is too short: " + shortnessValue + " < " + minValue)
        {
        }

    }
}
