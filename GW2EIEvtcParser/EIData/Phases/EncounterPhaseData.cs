using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.FightData;

namespace GW2EIEvtcParser.EIData;

public class EncounterPhaseData : PhaseData
{
    public bool Success { get; internal set; }
    public EncounterMode FightMode { get; internal set; } = EncounterMode.NotSet;
    public bool IsCM => FightMode == EncounterMode.CMNoName || FightMode == EncounterMode.CM;
    public bool IsLegendaryCM => FightMode == EncounterMode.LegendaryCM;
    internal EncounterPhaseData(long start, long end, bool success, EncounterMode mode) : base(start, end, PhaseType.Encounter)
    {
        Success = success;
        FightMode = mode;
    }

    internal EncounterPhaseData(long start, long end, string name, bool success, EncounterMode mode) : this(start, end, success, mode)
    {
        Name = name;
    }

    internal EncounterPhaseData(long start, long end, ParsedEvtcLog log) : this(start, end, log.FightData.Success, log.FightData.FightMode)
    {
    }

    internal EncounterPhaseData(long start, long end, string name, ParsedEvtcLog log) : this(start, end, name, log.FightData.Success, log.FightData.FightMode)
    {
    }
}
