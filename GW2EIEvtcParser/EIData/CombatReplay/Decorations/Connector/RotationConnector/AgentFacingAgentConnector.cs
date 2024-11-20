using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class AgentFacingAgentConnector : AgentFacingConnector
{
    public readonly AgentItem DstAgent;

    public AgentFacingAgentConnector(SingleActor agent, SingleActor dstAgent) : this(agent.AgentItem, dstAgent.AgentItem)
    {
    }

    public AgentFacingAgentConnector(AgentItem agent, AgentItem dstAgent) : base(agent)
    {
        DstAgent = dstAgent;
    }

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new AgentFacingAgentConnectorDescription(this, map, log);
    }
}
