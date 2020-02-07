using System;
using System.Configuration;
using System.IO;

namespace GW2EIParser.Parser
{
    public class ParserSettings
    {
        public bool AnonymousPlayer { get; protected set; }
        public bool SkipFailedTries { get; protected set; }
        public bool ParsePhases { get; protected set; }
        public bool ParseCombatReplay { get; protected set; }
        public bool ComputeDamageModifiers { get; protected set; }
        public bool RawTimelineArrays { get; protected set; }
        public bool MultiTasks { get; protected set; }

        public ParserSettings()
        {
            AnonymousPlayer = Properties.Settings.Default.Anonymous;
            SkipFailedTries = Properties.Settings.Default.SkipFailedTries;
            ParsePhases = Properties.Settings.Default.ParsePhases;
            ParseCombatReplay = Properties.Settings.Default.ParseCombatReplay;
            ComputeDamageModifiers = Properties.Settings.Default.ComputeDamageModifiers;
            RawTimelineArrays = Properties.Settings.Default.RawTimelineArrays;
            MultiTasks = Properties.Settings.Default.MultiThreaded;
        }
    }
}
