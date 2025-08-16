using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class BreakbarPhaseData : SubPhasePhaseData
{
    internal BreakbarPhaseData(long start, long end, string name) : base(start, end, name)
    {
        BreakbarPhase = true;
    }
}
