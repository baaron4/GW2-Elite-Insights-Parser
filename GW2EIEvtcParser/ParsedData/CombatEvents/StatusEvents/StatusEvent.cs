namespace GW2EIEvtcParser.ParsedData;

public abstract class StatusEvent : TimeCombatEvent
{
    public AgentItem Src { get; protected set; }

    public bool IsCustom { get; protected set; }

    internal StatusEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        Src = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
    }

    internal StatusEvent(AgentItem src, long time) : base(time)
    {
        IsCustom = true;
        Src = src.EnglobingAgentItem;
    }

}
