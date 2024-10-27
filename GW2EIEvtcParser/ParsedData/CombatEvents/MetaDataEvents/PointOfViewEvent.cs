namespace GW2EIEvtcParser.ParsedData;

public class PointOfViewEvent : AbstractMetaDataEvent
{
    public readonly AgentItem PoV;

    internal PointOfViewEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
    {
        PoV = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
    }

}
