namespace GW2EIEvtcParser;

public class EvtcParserSettings
{
    public bool AnonymousPlayers = false;
    public bool SkipFailedTries = false;
    public bool ComputePhases = true;
    public bool ComputeCombatReplay = true;
    public bool ComputeDamageModifiers = true;
    internal bool CanComputeDamageModifiers => ComputeDamageModifiers && ComputeDamage && ComputeBuff;
    public bool ComputeDamage = true;
    public bool ParseExtensions = true;
    public bool ComputeCast = true;
    public bool ComputeBuff = true;
    public bool ComputeMechanic = true;
    public readonly long TooShortLimit;
    public readonly long TooBigLimit;
    public bool DetailedWvWParse;

    public EvtcParserSettings(long tooShortLimit, long tooBigLimit)
    {
        TooShortLimit = Math.Max(tooShortLimit, ParserHelper.MinimumInCombatDuration);
        TooBigLimit = Math.Max(tooBigLimit, ParserHelper.MinimumFileSizeMB);
    }
}
