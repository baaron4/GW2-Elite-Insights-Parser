using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class AgentConnector : Connector
    {
        private AbstractMasterActor _agent;

        public AgentConnector(AbstractMasterActor agent)
        {
            _agent = agent;
        }

        public override object GetConnectedTo(CombatReplayMap map)
        {
            return _agent.GetCombatReplayID();
        }
    }
}
