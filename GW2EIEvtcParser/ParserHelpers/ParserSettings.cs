namespace GW2EIEvtcParser;

public class EvtcParserSettings
{
    public readonly bool AnonymousPlayers;
    public readonly bool SkipFailedTries;
    public readonly bool ParsePhases;
    public readonly bool ParseCombatReplay;
    public readonly bool ComputeDamageModifiers;
    public readonly long TooShortLimit;
    public readonly bool DetailedWvWParse;

    public EvtcParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers, long tooShortLimit) : this(anonymousPlayer, skipFailedTries, parsePhases, parseCombatReplay, computeDamageModifiers, tooShortLimit, false)
    {
    }

    public EvtcParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers, long tooShortLimit, bool detailedWvW)
    {
        AnonymousPlayers = anonymousPlayer;
        SkipFailedTries = skipFailedTries;
        ParsePhases = parsePhases;
        ParseCombatReplay = parseCombatReplay;
        ComputeDamageModifiers = computeDamageModifiers;
        TooShortLimit = Math.Max(tooShortLimit, ParserHelper.MinimumInCombatDuration);
        DetailedWvWParse = detailedWvW;
    }
}
