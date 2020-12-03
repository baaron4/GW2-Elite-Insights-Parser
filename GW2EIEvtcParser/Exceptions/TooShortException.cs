using System;

namespace GW2EIEvtcParser.Exceptions
{
    public class TooShortException : EIException
    {
        internal TooShortException(long shortnessValue, long minValue) : base("Fight is too short: " + shortnessValue + " < " + minValue)
        {
        }

        internal TooShortException(string message) : base(message)
        {
        }

    }
}
