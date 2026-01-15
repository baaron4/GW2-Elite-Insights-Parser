using System.Numerics;
using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.LogData;

namespace GW2EIEvtcParser.EIData;

public class EncounterPhaseData : PhaseDataWithMetaData
{
    internal EncounterPhaseData(long start, long end, string name, bool success, string icon, Mode mode, long encounterID) : base(start, end, name, success, icon, mode, encounterID, PhaseType.Encounter)
    {
    }

    internal EncounterPhaseData(long start, long end, string name, bool success, string icon, Mode mode, StartStatus startStatus, long encounterID) : base(start, end, name, success, icon, mode, startStatus, encounterID, PhaseType.Encounter)
    {
    }

    internal EncounterPhaseData(long start, long end, string name, ParsedEvtcLog log) : base(start, end, name, log, PhaseType.Encounter)
    {
    }
}
