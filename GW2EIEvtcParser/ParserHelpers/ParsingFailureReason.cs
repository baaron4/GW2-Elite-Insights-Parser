using System;

namespace GW2EIEvtcParser.ParserHelpers
{
    public class ParsingFailureReason
    {
        public Exception Reason { get; }
        internal ParsingFailureReason(Exception ex)
        {
            Reason = ex.GetFinalException();
        }

    }
}
