namespace GW2EIEvtcParser.Exceptions
{
    public class TooLongException : EINonFatalException
    {
        internal TooLongException() : base("Fight is took longer than 24h - may be a broken evtc")
        {
        }

    }
}
