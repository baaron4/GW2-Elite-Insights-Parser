using System;
using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Parser.ParsedData
{
    public class LogData
    {
        public static readonly string DefaultTimeValue = "MISSING";

        // Fields
        public string ArcVersion { get; } = "N/A";
        public string Language { get; } = "N/A";
        public LanguageEvent.LanguageEnum LanguageID { get; }
        public ulong GW2Build { get; } = 0;
        public AgentItem PoV { get; private set; } = null;
        public string PoVName { get; private set; } = "N/A";
        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss zz";
        private readonly string _dateFormatStd = "yyyy-MM-dd HH:mm:ss zzz";
        public string LogStart { get; private set; } = DefaultTimeValue;
        public string LogEnd { get; private set; } = DefaultTimeValue;
        public string LogStartStd { get; private set; } = DefaultTimeValue;
        public string LogEndStd { get; private set; } = DefaultTimeValue;

        public List<string> LogErrors { get; } = new List<string>();

        // Constructors
        public LogData(string buildVersion, CombatData combatData, long evtcLogDuration, List<Player> playerList, OperationController operation)
        {
            ArcVersion = buildVersion;
            double unixStart = 0;
            double unixEnd = 0;
            //
            PointOfViewEvent povEvt = combatData.GetPointOfViewEvent();
            if (povEvt != null)
            {
                SetPOV(povEvt.PoV, playerList);
            }
            operation.UpdateProgressWithCancellationCheck("PoV " + PoVName);
            //
            BuildEvent buildEvt = combatData.GetBuildEvent();
            if (buildEvt != null)
            {
                GW2Build = buildEvt.Build;
            }
            operation.UpdateProgressWithCancellationCheck("GW2 Build " + GW2Build);
            //
            LanguageEvent langEvt = combatData.GetLanguageEvent();
            if (langEvt != null)
            {
                Language = langEvt.ToString();
                LanguageID = langEvt.Language;
            }
            operation.UpdateProgressWithCancellationCheck("Language " + Language);
            //
            LogStartEvent logStr = combatData.GetLogStartEvent();
            if (logStr != null)
            {
                SetLogStart(logStr.LocalUnixTimeStamp);
                SetLogStartStd(logStr.LocalUnixTimeStamp);
                unixStart = logStr.LocalUnixTimeStamp;
            }
            //
            LogEndEvent logEnd = combatData.GetLogEndEvent();
            if (logEnd != null)
            {
                SetLogEnd(logEnd.LocalUnixTimeStamp);
                SetLogEndStd(logEnd.LocalUnixTimeStamp);
                unixEnd = logEnd.LocalUnixTimeStamp;
            }
            // log end event is missing, log start is present
            if (LogEnd == DefaultTimeValue && LogStart != DefaultTimeValue)
            {
                operation.UpdateProgressWithCancellationCheck("Missing Log End Event");
                double dur = Math.Round(evtcLogDuration / 1000.0, 3);
                SetLogEnd(dur + unixStart);
                SetLogEndStd(dur + unixStart);
            }
            // log start event is missing, log end is present
            if (LogEnd != DefaultTimeValue && LogStart == DefaultTimeValue)
            {
                operation.UpdateProgressWithCancellationCheck("Missing Log Start Event");
                double dur = Math.Round(evtcLogDuration / 1000.0, 3);
                SetLogStart(unixEnd - dur);
                SetLogStartStd(unixEnd - dur);
            }
            operation.UpdateProgressWithCancellationCheck("Log Start " + LogStartStd);
            operation.UpdateProgressWithCancellationCheck("Log End " + LogEndStd);
            //
            foreach (ErrorEvent evt in combatData.GetErrorEvents())
            {
                operation.UpdateProgressWithCancellationCheck("Error " + evt.Message);
                LogErrors.Add(evt.Message);
            }
        }

        // Setters
        private void SetPOV(AgentItem pov, List<Player> playerList)
        {
            PoV = pov;
            Player povPlayer = playerList.Find(x => x.AgentItem == pov);
            if (povPlayer != null)
            {
                PoVName = povPlayer.Character;
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
