using System;

namespace LuckParser.Exceptions
{
    public class TooShortException : Exception
    {
        public TooShortException() : base("Fight is too short, aborted")
        {
        }

    }
}
