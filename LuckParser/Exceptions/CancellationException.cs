using System;

namespace LuckParser.Exceptions
{
    public class CancellationException : Exception
    {
        public GridRow Row { get; }

        public CancellationException(GridRow row) : base("Operation aborted")
        {
            Row = row;
        }

        public CancellationException(GridRow row, Exception inner) : base("Operation aborted", inner)
        {
            Row = row;
        }
    }
}
