namespace GW2EIEvtcParser.Exceptions
{
    public abstract class EINonFatalException : EIException
    {
        internal EINonFatalException(string message) : base(message)
        {
        }

    }
}
