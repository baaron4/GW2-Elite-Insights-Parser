using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class AgentFacingConnector : RotationConnector
    {
        public AgentItem Agent { get; }

        public AgentFacingConnector(AbstractSingleActor agent)
        {
            Agent = agent.AgentItem;
        }

        public AgentFacingConnector(AgentItem agent)
        {
            Agent = agent;
        }
        public class AgentFacingConnectorDescriptor : RotationConnectorDescriptor
        {
            public int MasterId { get; private set; }
            public AgentFacingConnectorDescriptor(AgentFacingConnector connector, CombatReplayMap map) : base(connector, map)
            {
                MasterId = connector.Agent.UniqueID;
            }
        }

        public override RotationConnectorDescriptor GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new AgentFacingConnectorDescriptor(this, map);
        }
    }
}
