using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class Minion : AbstractPlayer
    {
        private ushort master_id;

        public Minion(ushort master, AgentItem agent) : base(agent)
        {
            master_id = master;
        }

        protected override void setDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getSrcInstid())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getNPCAgentList())
                    {//selecting all
                        addDamageLog(time, item.getInstid(), c, damage_logs);
                    }
                }

            }
        }

        protected override void setDamagetakenLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData, MechanicData m_data)
        {
            // nothing to do
            return;
        }
    }
}
