using System.Numerics;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class EmoteEvent : AnimatedCastEvent
{
    public readonly long EmoteID;

    internal EmoteEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, CombatItem? endItem, long maxEnd) : base(startItem, agentData, skillData, endItem, maxEnd)
    {
        EmoteID = (startItem ?? endItem ?? throw new InvalidOperationException("Either start or end item must be non null")).Pad;
    }

}
