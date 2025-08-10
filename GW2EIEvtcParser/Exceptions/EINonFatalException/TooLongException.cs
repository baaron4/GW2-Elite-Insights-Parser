namespace GW2EIEvtcParser.Exceptions;

public class TooLongException : EINonFatalException
{
    internal TooLongException() : base("Log is longer than 24h")
    {
    }

}
