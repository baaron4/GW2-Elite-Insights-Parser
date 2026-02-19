using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public abstract class EXTBarrierEvent : EXTHealingExtensionEvent
{
    public int BarrierGiven { get; protected set; }

    internal EXTBarrierEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
    }
    internal override double GetValue()
    {
        return BarrierGiven;
    }

}
