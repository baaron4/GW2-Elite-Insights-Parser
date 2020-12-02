using System;

namespace GW2EIParser.Exceptions
{
    public class EIException : Exception
    {

        internal EIException(Exception inner) : base("Operation aborted", inner)
        {
        }
    }
}
