using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTBarrierCombatData
    {
        private readonly Dictionary<AgentItem, List<EXTAbstractHealingEvent>> _barrierData;
        private readonly Dictionary<AgentItem, List<EXTAbstractHealingEvent>> _barrierReceivedData;
        private readonly Dictionary<long, List<EXTAbstractHealingEvent>> _barrierDataByID;

        internal EXTBarrierCombatData(Dictionary<AgentItem, List<EXTAbstractHealingEvent>> barrierData, Dictionary<AgentItem, List<EXTAbstractHealingEvent>> barrierReceivedData, Dictionary<long, List<EXTAbstractHealingEvent>> barrierDataByID)
        {
            _barrierData = barrierData;
            _barrierReceivedData = barrierReceivedData;
            _barrierDataByID = barrierDataByID;
        }

        public IReadOnlyList<EXTAbstractHealingEvent> GetHealData(AgentItem key)
        {
            if (_barrierData.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }
        public IReadOnlyList<EXTAbstractHealingEvent> GetHealReceivedData(AgentItem key)
        {
            if (_barrierReceivedData.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }

        public IReadOnlyList<EXTAbstractHealingEvent> GetHealData(long key)
        {
            if (_barrierDataByID.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }
    }
}
