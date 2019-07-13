using LuckParser.Parser;

namespace LuckParser.EIData
{
    public class AgentConnector : Connector
    {
        private AbstractMasterActor _agent;

        public AgentConnector(AbstractMasterActor agent)
        {
            _agent = agent;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedLog log)
        {
            return _agent.GetCombatReplayID(log);
        }
    }
}
