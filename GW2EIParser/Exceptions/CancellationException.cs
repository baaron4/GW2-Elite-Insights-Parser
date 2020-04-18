using System;

namespace GW2EIParser.Exceptions
{
    public class CancellationException : Exception
    {
        public Operation Operation { get; }

        public CancellationException(Operation operation) : base("Operation aborted")
        {
            Operation = operation;
        }

        public CancellationException(Operation operation, Exception inner) : base("Operation aborted", inner)
        {
            Operation = operation;
        }
    }
}
