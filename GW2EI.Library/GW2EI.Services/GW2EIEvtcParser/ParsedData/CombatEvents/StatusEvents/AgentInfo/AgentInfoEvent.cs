namespace GW2EIEvtcParser.ParsedData;

public abstract class AgentInfoEvent : StatusEvent
{
    public ParserHelper.Spec Type => Src.Spec;

    internal AgentInfoEvent(CombatItem evtcItem, AgentData agentData, EvtcVersionEvent evtcVersion) : base(evtcItem, agentData)
    {
    }

}
