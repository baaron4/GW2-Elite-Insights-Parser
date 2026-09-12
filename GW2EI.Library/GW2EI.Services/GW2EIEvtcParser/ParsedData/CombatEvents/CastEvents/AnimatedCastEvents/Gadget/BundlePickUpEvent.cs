namespace GW2EIEvtcParser.ParsedData;

public class BundlePickUpEvent : AnimatedCastEvent
{
    public readonly long BundleID;
    public AgentItem BundleGadget => EffectTarget;
    internal BundlePickUpEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, 
        CombatItem? endItem, long maxEnd) : base(startItem, agentData, skillData, endItem, maxEnd)
    {
        // Break the link
        Caster = ParserHelper._unknownAgent;
        if (startItem != null)
        {
            BundleID = startItem.OverstackValue;
        }
    }
    internal BundlePickUpEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem effectTarget) : base(caster, skill, start, dur, effectTarget)
    {
    }

}
