namespace GW2EIEvtcParser.Exceptions;

public class TooBigException : EINonFatalException
{
    internal TooBigException(long size, long maxValue) : base("File is too big: " + size + " mb > " + maxValue + " mb")
    {
    }

}
