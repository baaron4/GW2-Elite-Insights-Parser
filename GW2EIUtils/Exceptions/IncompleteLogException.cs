using System;

namespace GW2EIUtils.Exceptions
{
    public class IncompleteLogException : Exception
    {
        public IncompleteLogException() : base("Log incomplete")
        {
        }

    }
}
