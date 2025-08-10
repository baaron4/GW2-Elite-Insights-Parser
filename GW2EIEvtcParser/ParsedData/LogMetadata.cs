using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIEvtcParser.ParsedData;

public class LogMetadata
{
    private const string DefaultTimeValue = "MISSING";

    // Fields
    public readonly string ArcVersion = "";
    public readonly string ArcVersionBuild = "";
    public readonly int EvtcBuild;
    public readonly int EvtcRevision;
    public readonly string Language = "N/A";
    public readonly LanguageEvent.LanguageEnum LanguageID;
    public ulong GW2Build { get; private set; } = 0;
    public AgentItem? PoV { get; private set; } = null;
    public string PoVAccount { get; private set; } = "N/A";
    public string PoVName { get; private set; } = "N/A";
    private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss zz";
    private readonly string _dateFormatStd = "yyyy-MM-dd HH:mm:ss zzz";
    public string DateStart { get; private set; } = DefaultTimeValue;
    public string DateEnd { get; private set; } = DefaultTimeValue;
    public string DateStartStd { get; private set; } = DefaultTimeValue;
    public string? DateInstanceStartStd { get; private set; } = null;
    public string DateEndStd { get; private set; } = DefaultTimeValue;
    public readonly string? LogInstanceIP = null;

    public readonly IReadOnlyList<ExtensionHandler> UsedExtensions;

    public IReadOnlyList<string> LogErrors => _logErrors;
    private readonly List<string> _logErrors = [];

    // Constructors
    internal LogMetadata(EvtcVersionEvent evtcVersion, CombatData combatData, long evtcLogDuration, IReadOnlyList<Player> playerList, IReadOnlyDictionary<uint, ExtensionHandler> extensions, ParserController operation)
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
        PointOfViewEvent? povEvt = combatData.GetPointOfViewEvent();
        if (povEvt != null)
        {
            SetPOV(povEvt.PoV, playerList);
        }
        operation.UpdateProgressWithCancellationCheck("Parsing: PoV " + PoVName);
        //
        GW2Build = combatData.GetGW2BuildEvent().Build;
        operation.UpdateProgressWithCancellationCheck("Parsing: GW2 Build " + GW2Build);
        //
        LanguageEvent? langEvt = combatData.GetLanguageEvent();
        if (langEvt != null)
        {
            Language = langEvt.ToString();
            LanguageID = langEvt.Language;
        }
        operation.UpdateProgressWithCancellationCheck("Parsing: Language " + Language);
        //
        SquadCombatStartEvent? logStr = combatData.GetLogStartEvent();
        if (logStr != null)
        {
            SetDateStart(logStr.ServerUnixTimeStamp);
            SetDateStartStd(logStr.ServerUnixTimeStamp);
            unixStart = logStr.ServerUnixTimeStamp;
        }
        //
        SquadCombatEndEvent? logEnd = combatData.GetLogEndEvent();
        if (logEnd != null)
        {
            SetDateEnd(logEnd.ServerUnixTimeStamp);
            SetDateEndStd(logEnd.ServerUnixTimeStamp);
            unixEnd = logEnd.ServerUnixTimeStamp;
        }
        // log end event is missing, log start is present
        if (DateEnd == DefaultTimeValue && DateStart != DefaultTimeValue)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Missing Log End Event");
            double dur = Math.Round(evtcLogDuration / 1000.0, 3);
            SetDateEnd(dur + unixStart);
            SetDateEndStd(dur + unixStart);
            unixEnd = dur + unixStart;
        }
        // log start event is missing, log end is present
        if (DateEnd != DefaultTimeValue && DateStart == DefaultTimeValue)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Missing Log Start Event");
            double dur = Math.Round(evtcLogDuration / 1000.0, 3);
            SetDateStart(unixEnd - dur);
            SetDateStartStd(unixEnd - dur);
            unixStart = unixEnd - dur;
        }
        operation.UpdateProgressWithCancellationCheck("Parsing: Log Start " + DateStartStd);
        operation.UpdateProgressWithCancellationCheck("Parsing: Log End " + DateEndStd);
        InstanceStartEvent? instanceStart = combatData.GetInstanceStartEvent();
        if (instanceStart != null)
        {
            long instanceStartSeconds = instanceStart.TimeOffsetFromInstanceCreation / 1000;
            DateInstanceStartStd = GetDateTimeStd(unixStart - instanceStartSeconds);
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
        Player? povPlayer = playerList.FirstOrDefault(x => x.AgentItem.Is(pov));
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

    private void SetDateStart(double unixSeconds)
    {
        DateStart = GetDateTime(unixSeconds);
    }

    private void SetDateEnd(double unixSeconds)
    {
        DateEnd = GetDateTime(unixSeconds);
    }

    private void SetDateStartStd(double unixSeconds)
    {
        DateStartStd = GetDateTimeStd(unixSeconds);
    }

    private void SetDateEndStd(double unixSeconds)
    {
        DateEndStd = GetDateTimeStd(unixSeconds);
    }
}
