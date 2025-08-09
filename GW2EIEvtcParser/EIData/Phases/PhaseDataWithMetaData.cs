using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.FightData;

namespace GW2EIEvtcParser.EIData;

public abstract class PhaseDataWithMetaData : PhaseData
{
    public readonly bool Success;
    public readonly EncounterMode Mode = EncounterMode.NotSet;
    public readonly EncounterStartStatus StartStatus = EncounterStartStatus.NotSet;
    public bool IsLateStart => StartStatus == EncounterStartStatus.Late || MissingPreEvent;
    public bool MissingPreEvent => StartStatus == EncounterStartStatus.NoPreEvent;
    public bool IsCM => Mode == EncounterMode.CMNoName || Mode == EncounterMode.CM;
    public bool IsLegendaryCM => Mode == EncounterMode.LegendaryCM;

    public readonly string NameNoMode = "";
    public readonly string Icon;
    internal PhaseDataWithMetaData(long start, long end, bool success, string icon, EncounterMode mode, PhaseType type) : base(start, end, type)
    {
        Success = success;
        Mode = mode;
        Icon = icon;
    }

    internal PhaseDataWithMetaData(long start, long end, string name, bool success, string icon, EncounterMode mode, PhaseType type) : this(start, end, success, icon, mode, type)
    {
        NameNoMode = name;
        Name = name
            + (Mode == EncounterMode.CM ? " CM" : "")
            + (Mode == EncounterMode.LegendaryCM ? " LCM" : "")
            + (Mode == EncounterMode.Story ? " Story" : "")
            + (IsLateStart && !MissingPreEvent ? " (Late Start)" : "")
            + (MissingPreEvent ? " (No Pre-Event)" : ""); ;
    }

    internal PhaseDataWithMetaData(long start, long end, ParsedEvtcLog log, PhaseType type) : this(start, end, log.FightData.Success, log.FightData.Logic.Icon, log.FightData.FightMode, type)
    {
        StartStatus = log.FightData.FightStartStatus;
    }

    internal PhaseDataWithMetaData(long start, long end, string name, ParsedEvtcLog log, PhaseType type) : this(start, end, name, log.FightData.Success, log.FightData.Logic.Icon, log.FightData.FightMode, type)
    {
    }
}
