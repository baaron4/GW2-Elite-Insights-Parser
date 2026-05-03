using System.Numerics;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class EmoteEvent : AnimatedCastEvent
{
    public readonly long EmoteID;
    public readonly EmoteGUIDEvent EmoteGUIDEvent = EmoteGUIDEvent.DummyEmoteGUID;

    internal EmoteEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, CombatItem? endItem, 
        long maxEnd, IReadOnlyDictionary<long, EmoteGUIDEvent> emoteGUIDict) : base(startItem, agentData, skillData, endItem, maxEnd)
    {
        if (startItem != null)
        {
            EmoteID = startItem.IsStateChange == StateChange.AnimationStart ? startItem.OverstackValue : startItem.Pad;
            if (emoteGUIDict.TryGetValue(EmoteID, out var emoteGUIDEvent))
            {
                EmoteGUIDEvent = emoteGUIDEvent;
            }
        }
    }

}
