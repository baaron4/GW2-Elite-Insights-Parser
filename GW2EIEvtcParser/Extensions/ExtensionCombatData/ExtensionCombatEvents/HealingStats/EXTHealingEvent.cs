using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions;

public abstract class EXTHealingEvent : EXTHealingExtensionEvent
{
    public int HealingDone { get; protected set; }

    internal EXTHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
    }

    public EXTHealingType GetHealingType(ParsedEvtcLog log)
    {
        return log.CombatData.EXTHealingCombatData.GetHealingType(Skill, log);
    }
    internal override double GetValue()
    {
        return HealingDone;
    }

}
