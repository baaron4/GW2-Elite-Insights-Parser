using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser
{
    public class CancellationException : Exception
    {
        public GridRow Row { get; }

        public CancellationException(GridRow row)
        {
            Row = row;
        }
    }
}
