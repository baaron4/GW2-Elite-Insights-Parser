using System;
using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser.ParserHelpers
{
    /// <summary>
    /// Reason for the parsing failure
    /// </summary>
    public class ParsingFailureReason
    {
        private Exception _reason { get; }

        internal ParsingFailureReason(Exception ex)
        {
            _reason = ParserHelper.GetFinalException(ex);
        }

        /// <summary>
        /// Throws the exception
        /// </summary>
        public void Throw()
        {
            throw _reason;
        }

        /// <summary>
        /// Throws the exception if reason is not a non fatal ei exception
        /// </summary>
        public void ThrowIfFatal()
        {
            if (!(_reason is EINonFatalException))
            {
                throw _reason;
            }
        }

        /// <summary>
        /// Throws the exception if reason is not an ei exception
        /// </summary>
        public void ThrowIfUnexpected()
        {
            if (!(_reason is EIException))
            {
                throw _reason;
            }
        }

    }
}
