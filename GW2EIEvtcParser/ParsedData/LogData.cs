using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIEvtcParser.ParsedData
{
    public class LogData
    {
        private const string DefaultTimeValue = "MISSING";

        // Fields
        public string ArcVersion { get; }
        public string ArcVersionBuild { get; }
        public int EvtcBuild { get; }
        public int EvtcRevision { get; }
        public string Language { get; } = "N/A";
        public LanguageEvent.LanguageEnum LanguageID { get; }
        public ulong GW2Build { get; private set; } = 0;
        public AgentItem PoV { get; private set; } = null;
        public string PoVAccount { get; private set; } = "N/A";
        public string PoVName { get; private set; } = "N/A";
        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss zz";
        private readonly string _dateFormatStd = "yyyy-MM-dd HH:mm:ss zzz";
        public string LogStart { get; private set; } = DefaultTimeValue;
        public string LogEnd { get; private set; } = DefaultTimeValue;
        public string LogStartStd { get; private set; } = DefaultTimeValue;
        public string LogInstanceStartStd { get; private set; } = null;
        public string LogEndStd { get; private set; } = DefaultTimeValue;
        public string LogInstanceIP { get; } = null;

        public IReadOnlyList<AbstractExtensionHandler> UsedExtensions { get; }

        public IReadOnlyList<string> LogErrors => _logErrors;
        private readonly List<string> _logErrors = new List<string>();

        // Constructors
        internal LogData(EvtcVersionEvent evtcVersion, CombatData combatData, long evtcLogDuration, IReadOnlyList<Player> playerList, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, ParserController operation)
        {
            if (evtcVersion != null)
            {
                ArcVersion = evtcVersion.ToEVTCString(false);
                ArcVersionBuild = evtcVersion.ToEVTCString(true);
                EvtcBuild = evtcVersion.Build;
                EvtcRevision = evtcVersion.Revision;
            }
            double unixStart = 0;
            double unixEnd = 0;
            //
            PointOfViewEvent povEvt = combatData.GetPointOfViewEvent();
            if (povEvt != null)
            {
                SetPOV(povEvt.PoV, playerList);
            }
            operation.UpdateProgressWithCancellationCheck("Parsing: PoV " + PoVName);
            //
            GW2BuildEvent gw2BuildEvent = combatData.GetGW2BuildEvent();
            if (gw2BuildEvent != null)
            {
                GW2Build = gw2BuildEvent.Build;
            }
            operation.UpdateProgressWithCancellationCheck("Parsing: GW2 Build " + GW2Build);
            //
            LanguageEvent langEvt = combatData.GetLanguageEvent();
            if (langEvt != null)
            {
                Language = langEvt.ToString();
                LanguageID = langEvt.Language;
            }
            operation.UpdateProgressWithCancellationCheck("Parsing: Language " + Language);
            //
            LogStartEvent logStr = combatData.GetLogStartEvent();
            if (logStr != null)
            {
                SetLogStart(logStr.ServerUnixTimeStamp);
                SetLogStartStd(logStr.ServerUnixTimeStamp);
                unixStart = logStr.ServerUnixTimeStamp;
            }
            //
            LogEndEvent logEnd = combatData.GetLogEndEvent();
            if (logEnd != null)
            {
                SetLogEnd(logEnd.ServerUnixTimeStamp);
                SetLogEndStd(logEnd.ServerUnixTimeStamp);
                unixEnd = logEnd.ServerUnixTimeStamp;
            }
            // log end event is missing, log start is present
            if (LogEnd == DefaultTimeValue && LogStart != DefaultTimeValue)
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Missing Log End Event");
                double dur = Math.Round(evtcLogDuration / 1000.0, 3);
                SetLogEnd(dur + unixStart);
                SetLogEndStd(dur + unixStart);
                unixEnd = dur + unixStart;
            }
            // log start event is missing, log end is present
            if (LogEnd != DefaultTimeValue && LogStart == DefaultTimeValue)
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Missing Log Start Event");
                double dur = Math.Round(evtcLogDuration / 1000.0, 3);
                SetLogStart(unixEnd - dur);
                SetLogStartStd(unixEnd - dur);
                unixStart = unixEnd - dur;
            }
            operation.UpdateProgressWithCancellationCheck("Parsing: Log Start " + LogStartStd);
            operation.UpdateProgressWithCancellationCheck("Parsing: Log End " + LogEndStd);
            InstanceStartEvent instanceStart = combatData.GetInstanceStartEvent();
            if (instanceStart != null)
            {
                long instanceStartSeconds = instanceStart.TimeOffsetFromInstanceCreation / 1000;
                LogInstanceStartStd = GetDateTimeStd(unixStart - instanceStartSeconds);
                LogInstanceIP = instanceStart.InstanceIP;
            }
            //
            foreach (ErrorEvent evt in combatData.GetErrorEvents())
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Error " + evt.Message);
                _logErrors.Add(evt.Message);
            }
            //
            UsedExtensions = extensions.Values.ToList();
        }

        // Setters
        private void SetPOV(AgentItem pov, IReadOnlyList<Player> playerList)
        {
            PoV = pov;
            Player povPlayer = playerList.FirstOrDefault(x => x.AgentItem == pov);
            if (povPlayer != null)
            {
                PoVName = povPlayer.Character;
                PoVAccount = povPlayer.Account;
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
