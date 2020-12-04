using System;
using GW2EIEvtcParser;

namespace GW2EIParser.Exceptions
{
    public class ProgramException : Exception
    {

        internal ProgramException(Exception ex) : base("Operation aborted", ParserHelper.GetFinalException(ex))
        {
        }
    }
}
