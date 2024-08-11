using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class EXTAbstractBarrierEvent : EXTAbstractHealingExtensionEvent
    {
        public int BarrierGiven { get; protected set; }

        internal EXTAbstractBarrierEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

    }
}
