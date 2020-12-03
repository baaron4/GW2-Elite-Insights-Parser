using System;

namespace GW2EIEvtcParser
{
    public class EvtcParserSettings
    {
        internal bool AnonymousPlayer { get; }
        internal bool SkipFailedTries { get; }
        internal bool ParsePhases { get; }
        internal bool ParseCombatReplay { get; }
        internal bool ComputeDamageModifiers { get; }
        internal long TooShortLimit { get; }
        internal bool DetailedWvWParse { get; }

        public EvtcParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers, long tooShortLimit) : this(anonymousPlayer, skipFailedTries, parsePhases, parseCombatReplay, computeDamageModifiers, tooShortLimit, false)
        {
        }

        public EvtcParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers, long tooShortLimit, bool detailledWvW)
        {
            AnonymousPlayer = anonymousPlayer;
            SkipFailedTries = skipFailedTries;
            ParsePhases = parsePhases;
            ParseCombatReplay = parseCombatReplay;
            ComputeDamageModifiers = computeDamageModifiers;
            TooShortLimit = Math.Max(tooShortLimit, 2200);
            DetailedWvWParse = detailledWvW;
        }
    }
}
