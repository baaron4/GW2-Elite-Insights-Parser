using LuckParser.Parser;
using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class LogData
    {
        public static readonly string DefaultTimeValue = "MISSING";

        // Fields
        public readonly string BuildVersion;
        public ulong GW2Version { get; set; }
        public AgentItem PoV { get; private set; } = null;
        public string PoVName { get; private set; } = "N/A";
        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss zz";
        public string LogStart { get; private set; }
        public string LogEnd { get; private set; }

        // Constructors
        public LogData(string buildVersion, CombatData combatData, List<CombatItem> allCombatItems)
        {
            BuildVersion = buildVersion;
            LogStart = DefaultTimeValue;
            LogEnd = DefaultTimeValue;
        }
        
        // Setters
        private void SetPOV(AgentItem pov)
        {
            PoV = pov;
            PoVName = pov.Name.Substring(0, pov.Name.LastIndexOf('\0')).Split(':')[0].TrimEnd('\u0000');
        }

        private string GetDateTime(long unixSeconds)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            return dtDateTime.ToString(_dateFormat);
        }

        private void SetLogStart(long unixSeconds)
        {
            LogStart = GetDateTime(unixSeconds);
        }

        private void SetLogEnd(long unixSeconds)
        {
            LogEnd = GetDateTime(unixSeconds);
        }
    }
}