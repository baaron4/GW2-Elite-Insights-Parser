using System;

namespace GW2EIParser.Exceptions
{
    public class TooShortException : Exception
    {
        public TooShortException() : base("Error Encountered: Fight is too short")
        {
        }

    }
}
