using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public class EXTBarrierCombatData
{
    private readonly Dictionary<AgentItem, List<EXTBarrierEvent>> _barrierData;
    private readonly Dictionary<AgentItem, List<EXTBarrierEvent>> _barrierReceivedData;
    private readonly Dictionary<long, List<EXTBarrierEvent>> _barrierDataByID;

    internal EXTBarrierCombatData(Dictionary<AgentItem, List<EXTBarrierEvent>> barrierData, Dictionary<AgentItem, List<EXTBarrierEvent>> barrierReceivedData, Dictionary<long, List<EXTBarrierEvent>> barrierDataByID)
    {
        _barrierData = barrierData;
        _barrierReceivedData = barrierReceivedData;
        _barrierDataByID = barrierDataByID;
    }

    public IReadOnlyList<EXTBarrierEvent> GetBarrierData(AgentItem key)
    {
        return CombatData.GetTimeValueOrEmpty(_barrierData, key);
    }
    public IReadOnlyList<EXTBarrierEvent> GetBarrierReceivedData(AgentItem key)
    {
        return CombatData.GetTimeValueOrEmpty(_barrierReceivedData, key);
    }

    public IReadOnlyList<EXTBarrierEvent> GetBarrierData(long key)
    {
        return _barrierDataByID.GetValueOrEmpty(key);
    }
}
