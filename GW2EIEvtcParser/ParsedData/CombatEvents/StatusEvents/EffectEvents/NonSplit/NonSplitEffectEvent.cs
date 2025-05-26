using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public abstract class NonSplitEffectEvent : EffectEvent
{

    internal NonSplitEffectEvent(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, EffectGUIDEvent> effectGUIDs) : base(evtcItem, agentData, effectGUIDs)
    {
        if (evtcItem.DstAgent != 0)
        {
            _dst = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        }
        else
        {
            Position = new(
                BitConverter.Int32BitsToSingle(evtcItem.Value),
                BitConverter.Int32BitsToSingle(evtcItem.BuffDmg),
                BitConverter.Int32BitsToSingle(unchecked((int)evtcItem.OverstackValue))
            );
        }
    }
}
