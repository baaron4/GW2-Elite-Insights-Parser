namespace GW2EIEvtcParser;

public class EvtcParserSettings
{
    public readonly bool AnonymousPlayers;
    public readonly bool SkipFailedTries;
    public readonly bool ParsePhases;
    public readonly bool ParseCombatReplay;
    public readonly bool ComputeDamageModifiers;
    public readonly long TooShortLimit;
    public readonly long TooBigLimit;
    public readonly bool DetailedWvWParse;

    public EvtcParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers, long tooShortLimit, long tooBigLimit) : this(anonymousPlayer, skipFailedTries, parsePhases, parseCombatReplay, computeDamageModifiers, tooShortLimit, tooBigLimit, false)
    {
    }

    public EvtcParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers, long tooShortLimit, long tooBigLimit, bool detailedWvW)
    {
        AnonymousPlayers = anonymousPlayer;
        SkipFailedTries = skipFailedTries;
        ParsePhases = parsePhases;
        ParseCombatReplay = parseCombatReplay;
        ComputeDamageModifiers = computeDamageModifiers;
        TooShortLimit = Math.Max(tooShortLimit, ParserHelper.MinimumInCombatDuration);
        TooBigLimit = Math.Max(tooBigLimit, ParserHelper.MinimumFileSizeMB);
        DetailedWvWParse = detailedWvW;
    }
}
