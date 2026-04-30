using System.Numerics;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class EmoteEvent : AnimatedCastEvent
{
    public readonly long EmoteID;
    public readonly EmoteGUIDEvent EmoteGUIDEvent = EmoteGUIDEvent.DummyEmoteGUID;

    internal readonly byte Behavior;
    internal EmoteEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, CombatItem? endItem, long maxEnd, IReadOnlyDictionary<long, EmoteGUIDEvent> emoteGUIDict) : base(startItem, agentData, skillData, endItem, maxEnd)
    {
        EmoteID = (startItem ?? endItem ?? throw new InvalidOperationException("Either start or end item must be non null")).Pad;
        if (emoteGUIDict.TryGetValue(EmoteID, out var emoteGUIDEvent))
        {
            EmoteGUIDEvent = emoteGUIDEvent;
        }
        if (startItem != null)
        {
            Behavior = startItem.IsOffcycle;
        }
    }

}
