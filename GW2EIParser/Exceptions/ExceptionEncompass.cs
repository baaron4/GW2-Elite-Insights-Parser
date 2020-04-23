using System;

namespace GW2EIParser.Exceptions
{
    public class ExceptionEncompass : Exception
    {

        public ExceptionEncompass() : base("Operation aborted")
        {
        }

        public ExceptionEncompass(Exception inner) : base("Operation aborted", inner)
        {
        }
    }
}
