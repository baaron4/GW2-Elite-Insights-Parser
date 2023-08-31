using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class SkillConnector : Connector
    {
        public AgentItem Agent { get; }

        public SkillConnector(AbstractSingleActor agent)
        {
            Agent = agent.AgentItem;
        }

        public SkillConnector(AgentItem agent)
        {
            Agent = agent;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return Agent.UniqueID;
        }
    }
}
