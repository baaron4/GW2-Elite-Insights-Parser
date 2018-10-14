using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class AgentConnector : Connector
    {
        private AbstractMasterPlayer _agent;

        public AgentConnector(AbstractMasterPlayer agent)
        {
            _agent = agent;
        }

        public override object GetConnectedTo(CombatReplayMap map)
        {
            return _agent.GetCombatReplayID();
        }
    }
}
