using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTNonDirectBarrierEvent : EXTAbstractBarrierEvent
    {

        internal EXTNonDirectBarrierEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BarrierGiven = -evtcItem.BuffDmg;
        }
    }
}
