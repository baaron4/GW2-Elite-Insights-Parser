using System.Numerics;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class EffectEventAgentCreate : SplitEffectEvent
{
    internal EffectEventAgentCreate(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, EffectGUIDEvent> effectGUIDs, Dictionary<long, List<EffectEventAgentCreate>> effectEventsByTrackingID) : base(evtcItem, agentData, effectGUIDs)
    {
        _dst = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        if (TrackingID != 0)
        {
            Add(effectEventsByTrackingID, TrackingID, this);
        }
    }

}
