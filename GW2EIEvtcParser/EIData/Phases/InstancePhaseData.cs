using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.FightData;

namespace GW2EIEvtcParser.EIData;

public class InstancePhaseData : PhaseDataWithMetaData
{
    internal InstancePhaseData(long start, long end, bool success, string icon, EncounterMode mode) : base(start, end, success, icon, mode, PhaseType.Instance)
    {
    }

    internal InstancePhaseData(long start, long end, string name, bool success, string icon, EncounterMode mode) : base(start, end, name, success, icon, mode, PhaseType.Instance)
    {
    }

    internal InstancePhaseData(long start, long end, ParsedEvtcLog log) : base(start, end, log, PhaseType.Instance)
    {
    }

    internal InstancePhaseData(long start, long end, string name, ParsedEvtcLog log) : base(start, end, name, log, PhaseType.Instance)
    {
    }
}
