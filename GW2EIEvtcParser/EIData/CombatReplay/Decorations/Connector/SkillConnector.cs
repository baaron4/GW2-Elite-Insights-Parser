using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class SkillConnector : Connector
{
    public readonly AgentItem Agent;

    public SkillConnector(AbstractSingleActor agent)
    {
        Agent = agent.AgentItem;
    }

    public SkillConnector(AgentItem agent)
    {
        Agent = agent;
    }

    public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return Agent.UniqueID;
    }
}
