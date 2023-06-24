using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class AgentConnector : Connector
    {
        private readonly AgentItem _agent;

        public AgentConnector(AbstractSingleActor agent)
        {
            _agent = agent.AgentItem;
        }

        public AgentConnector(AgentItem agent)
        {
            _agent = agent;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return _agent.UniqueID;
        }
    }
}
