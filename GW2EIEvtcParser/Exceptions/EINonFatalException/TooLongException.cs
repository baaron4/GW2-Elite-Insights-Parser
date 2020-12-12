namespace GW2EIEvtcParser.Exceptions
{
    public class TooLongException : EINonFatalException
    {
        internal TooLongException() : base("Fight is longer than 24h")
        {
        }

    }
}
