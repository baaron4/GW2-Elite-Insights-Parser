namespace GW2EIEvtcParser.Exceptions
{
    public class SkipException : EINonFatalException
    {
        internal SkipException() : base("Option enabled - Failed logs are skipped")
        {
        }

    }
}
