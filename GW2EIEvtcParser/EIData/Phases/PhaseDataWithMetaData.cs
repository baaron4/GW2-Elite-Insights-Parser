using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.FightData;

namespace GW2EIEvtcParser.EIData;

public abstract class PhaseDataWithMetaData : PhaseData
{
    public readonly bool Success;
    public readonly EncounterMode FightMode = EncounterMode.NotSet;
    public readonly EncounterStartStatus StartStatus = EncounterStartStatus.NotSet;
    public bool IsLateStart => StartStatus == EncounterStartStatus.Late || MissingPreEvent;
    public bool MissingPreEvent => StartStatus == EncounterStartStatus.NoPreEvent;
    public bool IsCM => FightMode == EncounterMode.CMNoName || FightMode == EncounterMode.CM;
    public bool IsLegendaryCM => FightMode == EncounterMode.LegendaryCM;

    public readonly string NameNoMode = "";
    public readonly string Icon;
    internal PhaseDataWithMetaData(long start, long end, bool success, string icon, EncounterMode mode, PhaseType type) : base(start, end, type)
    {
        Success = success;
        FightMode = mode;
        Icon = icon;
    }

    internal PhaseDataWithMetaData(long start, long end, string name, bool success, string icon, EncounterMode mode, PhaseType type) : this(start, end, success, icon, mode, type)
    {
        NameNoMode = name;
        Name = name
            + (FightMode == EncounterMode.CM ? " CM" : "")
            + (FightMode == EncounterMode.LegendaryCM ? " LCM" : "")
            + (FightMode == EncounterMode.Story ? " Story" : "")
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
