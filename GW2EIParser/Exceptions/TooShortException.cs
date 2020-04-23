using System;

namespace GW2EIParser.Exceptions
{
    public class TooShortException : Exception
    {
        public TooShortException() : base("Fight is too short")
        {
        }

    }
}
