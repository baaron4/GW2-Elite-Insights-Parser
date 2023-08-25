using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class AgentConnector : Connector
    {
        public AgentItem Agent { get; }

        public AgentConnector(AbstractSingleActor agent)
        {
            Agent = agent.AgentItem;
        }

        public AgentConnector(AgentItem agent)
        {
            Agent = agent;
        }

        protected class AgentConnectorDescriptor : ConnectorDescriptor
        {
            public int MasterId { get; private set; }
            public AgentConnectorDescriptor(AgentConnector connector, CombatReplayMap map) : base(connector, map)
            {
                MasterId = connector.Agent.UniqueID;
            }
        }

        public override ConnectorDescriptor GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new AgentConnectorDescriptor(this, map);
        }
    }
}
