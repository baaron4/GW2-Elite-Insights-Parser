using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTBarrierCombatData
    {
        private readonly Dictionary<AgentItem, List<EXTAbstractBarrierEvent>> _barrierData;
        private readonly Dictionary<AgentItem, List<EXTAbstractBarrierEvent>> _barrierReceivedData;
        private readonly Dictionary<long, List<EXTAbstractBarrierEvent>> _barrierDataByID;

        internal EXTBarrierCombatData(Dictionary<AgentItem, List<EXTAbstractBarrierEvent>> barrierData, Dictionary<AgentItem, List<EXTAbstractBarrierEvent>> barrierReceivedData, Dictionary<long, List<EXTAbstractBarrierEvent>> barrierDataByID)
        {
            _barrierData = barrierData;
            _barrierReceivedData = barrierReceivedData;
            _barrierDataByID = barrierDataByID;
        }

        public IReadOnlyList<EXTAbstractBarrierEvent> GetBarrierData(AgentItem key)
        {
            if (_barrierData.TryGetValue(key, out List<EXTAbstractBarrierEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractBarrierEvent>();
        }
        public IReadOnlyList<EXTAbstractBarrierEvent> GetBarrierReceivedData(AgentItem key)
        {
            if (_barrierReceivedData.TryGetValue(key, out List<EXTAbstractBarrierEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractBarrierEvent>();
        }

        public IReadOnlyList<EXTAbstractBarrierEvent> GetBarrierData(long key)
        {
            if (_barrierDataByID.TryGetValue(key, out List<EXTAbstractBarrierEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractBarrierEvent>();
        }
    }
}
