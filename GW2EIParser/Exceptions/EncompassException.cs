using System;

namespace GW2EIParser.Exceptions
{
    public class EncompassException : Exception
    {

        internal EncompassException(Exception inner) : base("Operation aborted", inner)
        {
        }
    }
}
