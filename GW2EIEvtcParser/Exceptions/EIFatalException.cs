namespace GW2EIEvtcParser.Exceptions
{
    public abstract class EIFatalException : EIException
    {
        internal EIFatalException(string message) : base(message)
        {
        }

    }
}
