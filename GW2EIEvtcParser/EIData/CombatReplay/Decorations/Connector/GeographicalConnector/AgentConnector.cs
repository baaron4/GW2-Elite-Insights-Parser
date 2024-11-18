using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class AgentConnector : GeographicalConnector
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

    public class AgentConnectorDescriptor : GeographicalConnectorDescriptor
    {
        public int MasterId { get; private set; }
        public AgentConnectorDescriptor(AgentConnector connector, CombatReplayMap map) : base(connector, map)
        {
            MasterId = connector.Agent.UniqueID;
        }
    }

    public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new AgentConnectorDescriptor(this, map);
    }
}
