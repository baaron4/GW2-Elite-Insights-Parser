namespace GW2EIEvtcParser.ParsedData;

public class LogNPCUpdateEvent : LogDateEvent
{
    public readonly int AgentID;

    public readonly AgentItem TriggerAgent = ParserHelper._unknownAgent;

    public readonly bool TriggerIsGadget;

    internal LogNPCUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
    {
        AgentID = (ushort)evtcItem.SrcAgent;
        if (evtcItem.DstAgent > 0)
        {
            TriggerIsGadget = evtcItem.IsFlanking > 0;
            TriggerAgent = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        }
    }

}
