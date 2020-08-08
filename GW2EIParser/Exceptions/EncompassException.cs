using System;

namespace GW2EIParser.Exceptions
{
    public class EncompassException : Exception
    {

        internal EncompassException() : base("Operation aborted")
        {
        }

        internal EncompassException(Exception inner) : base("Operation aborted", inner)
        {
        }
    }
}
