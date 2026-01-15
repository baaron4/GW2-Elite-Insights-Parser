using System.Numerics;
using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.LogData;

namespace GW2EIEvtcParser.EIData;

public class InstancePhaseData : PhaseDataWithMetaData
{
    internal InstancePhaseData(long start, long end, string name, ParsedEvtcLog log) : base(start, end, name, log, PhaseType.Instance)
    {
    }
}
