using System;

namespace GW2EIUtils.Exceptions
{
    public class EncompassException : Exception
    {

        public EncompassException() : base("Operation aborted")
        {
        }

        public EncompassException(Exception inner) : base("Operation aborted", inner)
        {
        }
    }
}
