using System.Numerics;
using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.LogData;

namespace GW2EIEvtcParser.EIData;

public class InstancePhaseData : PhaseDataWithMetaData
{
    internal InstancePhaseData(long start, long end, string name, bool success, Mode mode, StartStatus startStatus, LogData logData) : base(start, end, name, success, mode, startStatus, logData, PhaseType.Instance)
    {
    }
}
