using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Parser.ParsedData
{
    public class LogData
    {
        public static readonly string DefaultTimeValue = "MISSING";

        // Fields
        public string BuildVersion { get; }
        public ulong GW2Version { get; }
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
            double unixStart = 0;
            double unixEnd = 0;
            foreach (PointOfViewEvent povEvt in combatData.GetPointOfViewEvents())
            {
                SetPOV(povEvt.PoV);
            }
            foreach (BuildEvent buildEvt in combatData.GetBuildEvents())
            {
                GW2Version = buildEvt.Build;
            }
            foreach (LogStartEvent logStr in combatData.GetLogStartEvents())
            {
                SetLogStart(logStr.LocalUnixTimeStamp);
                unixStart = logStr.LocalUnixTimeStamp;
            }
            foreach (LogEndEvent logEnd in combatData.GetLogEndEvents())
            {
                SetLogEnd(logEnd.LocalUnixTimeStamp);
                unixEnd = logEnd.LocalUnixTimeStamp;
            }
            // log end event is missing, log start is present
            if (LogEnd == DefaultTimeValue && LogStart != DefaultTimeValue)
            {
                double dur = Math.Round((allCombatItems.Max(x => x.LogTime) - allCombatItems.Min(x => x.LogTime)) / 1000.0, 3);
                SetLogEnd(dur + unixStart);
            }
            // log start event is missing, log end is present
            if (LogEnd != DefaultTimeValue && LogStart == DefaultTimeValue)
            {
                double dur = Math.Round((allCombatItems.Max(x => x.LogTime) - allCombatItems.Min(x => x.LogTime)) / 1000.0, 3);
                SetLogStart(unixEnd - dur);
            }
        }

        // Setters
        private void SetPOV(AgentItem pov)
        {
            PoV = pov;
            try
            {
                PoVName = pov.Name.Substring(0, pov.Name.LastIndexOf('\0')).Split(':')[0].TrimEnd('\u0000');
            }
            catch (Exception)
            {
                PoVName = pov.Name;
            }
        }

        private string GetDateTime(double unixSeconds)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            return dtDateTime.ToString(_dateFormat);
        }

        private void SetLogStart(double unixSeconds)
        {
            LogStart = GetDateTime(unixSeconds);
        }

        private void SetLogEnd(double unixSeconds)
        {
            LogEnd = GetDateTime(unixSeconds);
        }
    }
}