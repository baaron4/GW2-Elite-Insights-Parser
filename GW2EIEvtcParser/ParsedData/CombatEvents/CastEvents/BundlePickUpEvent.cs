using System.Numerics;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class BundlePickUpEvent : GadgetInteractEvent
{
    public readonly long BundleID;
    internal BundlePickUpEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, 
        CombatItem? endItem, long maxEnd) : base(startItem, agentData, skillData, endItem, maxEnd)
    {
        // Break the link
        Caster = ParserHelper._unknownAgent;
        if (startItem != null && startItem.IsStateChange == StateChange.AnimationStart)
        {
            BundleID = startItem.OverstackValue;
        }
    }
    internal BundlePickUpEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem effectTarget) : base(caster, skill, start, dur, effectTarget)
    {
    }

}
