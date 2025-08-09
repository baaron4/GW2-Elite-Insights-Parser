using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.FightData;

namespace GW2EIEvtcParser.EIData;

public class EncounterPhaseData : PhaseData
{
    public readonly bool Success;
    public readonly EncounterMode FightMode = EncounterMode.NotSet;
    public bool IsCM => FightMode == EncounterMode.CMNoName || FightMode == EncounterMode.CM;
    public bool IsLegendaryCM => FightMode == EncounterMode.LegendaryCM;

    public readonly string EncounterName = "";
    public readonly string Icon;
    internal EncounterPhaseData(long start, long end, bool success, string icon, EncounterMode mode) : base(start, end, PhaseType.Encounter)
    {
        Success = success;
        FightMode = mode;
        Icon = icon;
    }

    internal EncounterPhaseData(long start, long end, string name, bool success, string icon, EncounterMode mode) : this(start, end, success, icon, mode)
    {
        EncounterName = name;
        Name = name
            + (FightMode == EncounterMode.CM ? " CM" : "")
            + (FightMode == EncounterMode.LegendaryCM ? " LCM" : "")
            + (FightMode == EncounterMode.Story ? " Story" : "");
    }

    internal EncounterPhaseData(long start, long end, ParsedEvtcLog log) : this(start, end, log.FightData.Success, log.FightData.Logic.Icon, log.FightData.FightMode)
    {
    }

    internal EncounterPhaseData(long start, long end, string name, ParsedEvtcLog log) : this(start, end, name, log.FightData.Success, log.FightData.Logic.Icon, log.FightData.FightMode)
    {
    }
}
