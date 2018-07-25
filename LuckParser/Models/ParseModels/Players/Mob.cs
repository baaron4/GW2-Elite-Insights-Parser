using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels.Players
{
    public class Mob : AbstractMasterPlayer
    {
        private String mobName;

        // Constructors
        public Mob(AgentItem agent) : base(agent)
        {
            String[] name = agent.getName().Split('\0');
            mobName = name[1];
           
        }

        //setters
        protected override void setDamagetakenLogs(ParsedLog log)
        {
        }
        //getters
        public string getMobName()
        {
            return mobName;
        }

        protected override void setAdditionalCombatReplayData(ParsedLog log)
        {
            // nothing to do here, thrash mobs don't have additional data
        }

        protected override void setCombatReplayIcon(ParsedLog log)
        {
            // nothing to do here, this will be managed by the boss-parent
        }
    }
}
