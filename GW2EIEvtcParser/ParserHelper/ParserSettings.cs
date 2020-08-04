namespace GW2EIEvtcParser
{
    public class EvtcParserSettings
    {
        public bool AnonymousPlayer { get; }
        public bool SkipFailedTries { get; }
        public bool ParsePhases { get; }
        public bool ParseCombatReplay { get; }
        public bool ComputeDamageModifiers { get; }

        public EvtcParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers)
        {
            AnonymousPlayer = anonymousPlayer;
            SkipFailedTries = skipFailedTries;
            ParsePhases = parsePhases;
            ParseCombatReplay = parseCombatReplay;
            ComputeDamageModifiers = computeDamageModifiers;
        }
    }
}
