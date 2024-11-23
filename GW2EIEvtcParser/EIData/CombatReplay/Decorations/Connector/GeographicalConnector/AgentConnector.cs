using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class AgentConnector : GeographicalConnector
{
    public readonly AgentItem Agent;

    public AgentConnector(SingleActor agent)
    {
        Agent = agent.AgentItem;
    }

    public AgentConnector(AgentItem agent)
    {
        Agent = agent;
    }

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new AgentConnectorDescription(this, map, log);
    }
}
