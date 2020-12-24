using GW2EIEvtcParser.Interfaces;

namespace GW2EIEvtcParser.ParsedData
{
    public class BarrierUpdateEvent : AbstractStatusEvent, IStateable
    {
        public double BarrierPercent { get; }

        internal BarrierUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            BarrierPercent = evtcItem.DstAgent / 100.0;
        }

        public (long start, double value) ToState()
        {
            return (Time, BarrierPercent);
        }
    }
}
