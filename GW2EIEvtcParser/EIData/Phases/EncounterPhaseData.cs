using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.FightData;

namespace GW2EIEvtcParser.EIData;

public class EncounterPhaseData : PhaseDataWithMetaData
{
    internal EncounterPhaseData(long start, long end, bool success, string icon, EncounterMode mode) : base(start, end, success, icon, mode, PhaseType.Encounter)
    {
    }

    internal EncounterPhaseData(long start, long end, string name, bool success, string icon, EncounterMode mode) : base(start, end, name, success, icon, mode, PhaseType.Encounter)
    {
    }

    internal EncounterPhaseData(long start, long end, ParsedEvtcLog log) : base(start, end, log, PhaseType.Encounter)
    {
    }

    internal EncounterPhaseData(long start, long end, string name, ParsedEvtcLog log) : base(start, end, name, log, PhaseType.Encounter)
    {
    }
}
