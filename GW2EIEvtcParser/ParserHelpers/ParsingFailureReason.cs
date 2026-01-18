using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser.ParserHelpers;

/// <summary>
/// Reason for the parsing failure
/// </summary>
public class ParsingFailureReason
{
    private readonly Exception _reason;

    public bool IsEvtcContentIssue => _reason is EvtcContentException;

    public bool IsSafeToIgnore => _reason is EINonFatalException;

    public bool IsParserBug => _reason is not EIException;

    public string Reason => _reason.Message;

    internal ParsingFailureReason(Exception ex)
    {
        _reason = ParserHelper.GetFinalException(ex);
    }

    /// <summary>
    /// Throws the exception
    /// </summary>
    public void Throw()
    {
        throw new Exception("Parsing Failed", _reason);
    }

    /// <summary>
    /// Throws the exception if reason is not an <see cref="EIException"/>
    /// </summary>
    public void ThrowIfUnknown()
    {
        if (IsParserBug)
        {
            throw new Exception("Parsing Failed", _reason);
        }
    }

    public override string ToString() => _reason.Message;

}
