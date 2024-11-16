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
        if (_barrierData.TryGetValue(key, out List<EXTBarrierEvent> res))
        {
            return res;
        }
        return new List<EXTBarrierEvent>();
    }
    public IReadOnlyList<EXTBarrierEvent> GetBarrierReceivedData(AgentItem key)
    {
        if (_barrierReceivedData.TryGetValue(key, out List<EXTBarrierEvent> res))
        {
            return res;
        }
        return new List<EXTBarrierEvent>();
    }

    public IReadOnlyList<EXTBarrierEvent> GetBarrierData(long key)
    {
        if (_barrierDataByID.TryGetValue(key, out List<EXTBarrierEvent> res))
        {
            return res;
        }
        return new List<EXTBarrierEvent>();
    }
}
