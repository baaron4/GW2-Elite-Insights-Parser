using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class AgentFacingAgentConnector : AgentFacingConnector
    {
        public AgentItem DstAgent { get; }

        public AgentFacingAgentConnector(AbstractSingleActor agent, AbstractSingleActor dstAgent) : this(agent.AgentItem, dstAgent.AgentItem)
        {
        }

        public AgentFacingAgentConnector(AgentItem agent, AgentItem dstAgent) : base(agent)
        {
            DstAgent = dstAgent;
        }
        public class AgentFacingAgentConnectorDescriptor : AgentFacingConnectorDescriptor
        {
            public int DstMasterId { get; private set; }
            public AgentFacingAgentConnectorDescriptor(AgentFacingAgentConnector connector, CombatReplayMap map) : base(connector, map)
            {
                DstMasterId = connector.DstAgent.UniqueID;
            }
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new AgentFacingAgentConnectorDescriptor(this, map);
        }
    }
}
