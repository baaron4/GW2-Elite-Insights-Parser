using System;

namespace GW2EIUtils.Exceptions
{
    public class TooShortException : Exception
    {
        public TooShortException() : base("Fight is too short")
        {
        }

    }
}
