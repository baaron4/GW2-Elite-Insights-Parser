using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMasterPlayer : AbstractPlayer
    {

        // Minions
        private Dictionary<string, Minions> minions = new Dictionary<string, Minions>();

        public AbstractMasterPlayer(AgentItem agent) : base(agent)
        {

        }


        public Dictionary<string, Minions> getMinions(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (minions.Count == 0)
            {
                setMinions(bossData, combatList, agentData);
            }
            return minions;
        }

        private void setMinions(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            List<ushort> combatMinionIDList = combatList.Where(x => x.getSrcMasterInstid() == instid).Select(x => x.getSrcInstid()).Distinct().ToList();
            foreach (ushort petid in combatMinionIDList)
            {
                AgentItem agent = agentData.getNPCAgentList().FirstOrDefault(x => x.getInstid() == petid);
                if (agent != null)
                {
                    List<DamageLog> damageLogs = getDamageLogs(0, bossData, combatList, agentData, 0, bossData.getAwareDuration()).Where(x => x.getInstidt() == petid).ToList();
                    if (damageLogs.Count == 0)
                    {
                        continue;
                    }
                    string id = agent.getName();
                    if (!minions.ContainsKey(id))
                    {
                        minions[id] = new Minions();
                    }
                    minions[id].Add(new Minion(instid, agent));
                }
            }
        }

    }
}
