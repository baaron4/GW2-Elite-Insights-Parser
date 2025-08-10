namespace GW2EIEvtcParser.Exceptions;

public class TooShortException : EINonFatalException
{
    internal TooShortException(long shortnessValue, long minValue) : base("Log is too short: " + shortnessValue + " < " + minValue)
    {
    }

}
