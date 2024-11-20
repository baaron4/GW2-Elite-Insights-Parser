using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class SkillConnector : Connector
{
    public readonly AgentItem Agent;

    public SkillConnector(SingleActor agent)
    {
        Agent = agent.AgentItem;
    }

    public SkillConnector(AgentItem agent)
    {
        Agent = agent;
    }

    public override SkillConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new SkillConnectorDescription(this, map, log);
    }
}
