using System;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Parser.ParsedData
{
    public class LogData
    {
        public static readonly string DefaultTimeValue = "MISSING";

        // Fields
        public string BuildVersion { get; }
        public string Language { get; }
        public LanguageEvent.LanguageEnum LanguageID { get; }
        public ulong GW2Version { get; }
        public AgentItem PoV { get; private set; } = null;
        public string PoVName { get; private set; } = "N/A";
        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss zz";
        private readonly string _dateFormatStd = "yyyy-MM-dd HH:mm:ss zzz";
        public string LogStart { get; private set; } = DefaultTimeValue;
        public string LogEnd { get; private set; } = DefaultTimeValue;
        public string LogStartStd { get; private set; } = DefaultTimeValue;
        public string LogEndStd { get; private set; } = DefaultTimeValue;

        // Constructors
        public LogData(string buildVersion, CombatData combatData, long evtcLogDuration)
        {
            BuildVersion = buildVersion;
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
            foreach (LanguageEvent langEvt in combatData.GetLanguageEvents())
            {
                Language = langEvt.ToString();
                LanguageID = langEvt.Language;
            }
            foreach (LogStartEvent logStr in combatData.GetLogStartEvents())
            {
                SetLogStart(logStr.LocalUnixTimeStamp);
                SetLogStartStd(logStr.LocalUnixTimeStamp);
                unixStart = logStr.LocalUnixTimeStamp;
            }
            foreach (LogEndEvent logEnd in combatData.GetLogEndEvents())
            {
                SetLogEnd(logEnd.LocalUnixTimeStamp);
                SetLogEndStd(logEnd.LocalUnixTimeStamp);
                unixEnd = logEnd.LocalUnixTimeStamp;
            }
            // log end event is missing, log start is present
            if (LogEnd == DefaultTimeValue && LogStart != DefaultTimeValue)
            {
                double dur = Math.Round(evtcLogDuration / 1000.0, 3);
                SetLogEnd(dur + unixStart);
                SetLogEndStd(dur + unixStart);
            }
            // log start event is missing, log end is present
            if (LogEnd != DefaultTimeValue && LogStart == DefaultTimeValue)
            {
                double dur = Math.Round(evtcLogDuration / 1000.0, 3);
                SetLogStart(unixEnd - dur);
                SetLogStartStd(unixEnd - dur);
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

        private string GetDateTimeStd(double unixSeconds)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            return dtDateTime.ToString(_dateFormatStd);
        }

        private void SetLogStart(double unixSeconds)
        {
            LogStart = GetDateTime(unixSeconds);
        }

        private void SetLogEnd(double unixSeconds)
        {
            LogEnd = GetDateTime(unixSeconds);
        }

        private void SetLogStartStd(double unixSeconds)
        {
            LogStartStd = GetDateTimeStd(unixSeconds);
        }

        private void SetLogEndStd(double unixSeconds)
        {
            LogEndStd = GetDateTimeStd(unixSeconds);
        }
    }
}
