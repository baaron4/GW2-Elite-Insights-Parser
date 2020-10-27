using System;

namespace GW2EIEvtcParser
{
    public class EvtcParserSettings
    {
        public bool AnonymousPlayer { get; }
        public bool SkipFailedTries { get; }
        public bool ParsePhases { get; }
        public bool ParseCombatReplay { get; }
        public bool ComputeDamageModifiers { get; }
        public long CustomTooShort { get; }

        public EvtcParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers, long customTooShort)
        {
            AnonymousPlayer = anonymousPlayer;
            SkipFailedTries = skipFailedTries;
            ParsePhases = parsePhases;
            ParseCombatReplay = parseCombatReplay;
            ComputeDamageModifiers = computeDamageModifiers;
            CustomTooShort = Math.Max(customTooShort, 2200);
        }
    }
}
