using System;
using GW2EIEvtcParser;

namespace GW2EIParser.Exceptions
{
    public class EIException : Exception
    {

        internal EIException(Exception ex) : base("Operation aborted", ParserHelper.GetFinalException(ex))
        {
        }
    }
}
