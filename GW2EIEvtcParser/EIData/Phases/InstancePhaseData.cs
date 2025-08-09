using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class InstancePhaseData : PhaseData
{
    internal InstancePhaseData(long start, long end) : base(start, end, PhaseType.Instance)
    {
    }

    internal InstancePhaseData(long start, long end, string name) : base(start, end, name, PhaseType.Instance)
    {
    }
}
