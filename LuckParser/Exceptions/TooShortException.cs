using System;

namespace LuckParser.Exceptions
{
    public class TooShortException : Exception
    {
        public TooShortException() : base("Fight is less than 1 second, aborted")
        {
        }

    }
}
