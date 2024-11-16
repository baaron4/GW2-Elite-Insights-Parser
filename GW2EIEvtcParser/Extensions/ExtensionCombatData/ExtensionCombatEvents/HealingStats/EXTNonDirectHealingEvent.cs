using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public class EXTNonDirectHealingEvent : EXTHealingEvent
{

    internal EXTNonDirectHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        HealingDone = -evtcItem.BuffDmg;
    }
}
