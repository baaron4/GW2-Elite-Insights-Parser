using System;
using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser.ParserHelpers
{
    /// <summary>
    /// Reason for the parsing failure
    /// If the Reason is not an <see cref="EIException"/> please contact the developpers
    /// If the Reason is an <see cref="EINonFatalException"/>, that means that in the current conditions, the log will not be parsed
    /// If the Reason is an <see cref="EIFatalException"/>, that means there is something wrong with the input evtc file
    /// </summary>
    public class ParsingFailureReason
    {
        public Exception Reason { get; }

        internal ParsingFailureReason(Exception ex)
        {
            Reason = ParserHelper.GetFinalException(ex);
        }

    }
}
