namespace GW2EIEvtcParser.ParsedData;

public class GadgetAnimationEvent : TimeCombatEvent
{
    public readonly ulong AnimationToken;
    public readonly AgentItem Gadget;
    internal GadgetAnimationEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        AnimationToken = evtcItem.DstAgent;
        Gadget = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
    }

}
