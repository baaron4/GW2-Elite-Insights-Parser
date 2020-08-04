using System;
using System.Configuration;
using System.IO;

namespace GW2EIParser.Parser
{
    public class ParserSettings
    {
        public bool AnonymousPlayer { get; }
        public bool SkipFailedTries { get; }
        public bool ParsePhases { get; }
        public bool ParseCombatReplay { get; }
        public bool ComputeDamageModifiers { get; }

        public ParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers)
        {
            AnonymousPlayer = anonymousPlayer;
            SkipFailedTries = skipFailedTries;
            ParsePhases = parsePhases;
            ParseCombatReplay = parseCombatReplay;
            ComputeDamageModifiers = computeDamageModifiers;
        }
    }
}
