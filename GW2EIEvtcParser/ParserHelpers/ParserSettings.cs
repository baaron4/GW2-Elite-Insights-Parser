namespace GW2EIEvtcParser;

public class EvtcParserSettings
{
    public bool AnonymousPlayers { get; init; } = false;
    public bool SkipFailedTries { get; init; } = false;
    public bool ComputePhases { get; init; } = true;
    public bool ComputeCombatReplay { get; init; } = true;
    public bool ComputeDamageModifiers { get; init; } = true;
    internal bool CanComputeDamageModifiers => ComputeDamageModifiers && ComputeDamage && ComputeBuff;
    public bool ComputeDamage { get; init; } = true;
    public bool ParseExtensions { get; init; } = true;
    public bool ComputeCast { get; init; } = true;
    public bool ComputeBuff { get; init; } = true;
    public bool ComputeMechanics { get; init; } = true;
    public readonly long TooShortLimit;
    public readonly long TooBigLimit;
    public bool DetailedWvWParse { get; init; } = false;

    public EvtcParserSettings(long tooShortLimit, long tooBigLimit)
    {
        TooShortLimit = Math.Max(tooShortLimit, ParserHelper.MinimumInCombatDuration);
        TooBigLimit = Math.Max(tooBigLimit, ParserHelper.MinimumFileSizeMB);
    }
}
