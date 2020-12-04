using System;

namespace GW2EIEvtcParser.Exceptions
{
    public abstract class EIException : Exception
    {
        internal EIException(string message) : base(message)
        {
        }

    }
}
