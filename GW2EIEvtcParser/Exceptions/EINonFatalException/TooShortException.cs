namespace GW2EIEvtcParser.Exceptions
{
    public class TooShortException : EINonFatalException
    {
        internal TooShortException(long shortnessValue, long minValue) : base("Fight is too short: " + shortnessValue + " < " + minValue)
        {
        }

        internal TooShortException(string message) : base(message)
        {
        }

    }
}
