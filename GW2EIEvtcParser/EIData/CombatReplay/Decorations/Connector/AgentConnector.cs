namespace GW2EIEvtcParser.EIData
{
    internal class AgentConnector : Connector
    {
        private readonly AbstractSingleActor _agent;

        public AgentConnector(AbstractSingleActor agent)
        {
            _agent = agent;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return _agent.CombatReplayID;
        }
    }
}
