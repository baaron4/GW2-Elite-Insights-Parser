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

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return Agent.UniqueID;
        }
    }
}
