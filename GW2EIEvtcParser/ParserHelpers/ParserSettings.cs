using System;

namespace GW2EIEvtcParser
{
    public class EvtcParserSettings
    {
        public bool AnonymousPlayers { get; }
        public bool SkipFailedTries { get; }
        public bool ParsePhases { get; }
        public bool ParseCombatReplay { get; }
        public bool ComputeDamageModifiers { get; }
        public long TooShortLimit { get; }
        public bool DetailedWvWParse { get; }

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
}
