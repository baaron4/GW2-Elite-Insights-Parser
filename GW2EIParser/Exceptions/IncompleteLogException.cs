using System;

namespace GW2EIParser.Exceptions
{
    public class IncompleteLogException : Exception
    {
        public IncompleteLogException() : base("Log incomplete")
        {
        }

    }
}
