using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class Minions : List<Minion>
    {
        public Minions() : base()
        {
        }

        public List<DamageLog> getDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)
        {
            List<DamageLog> res = new List<DamageLog>();
            foreach (Minion minion in this)
            {
                res.AddRange(minion.getDamageLogs(instidFilter, bossData, combatList, agentData, start, end));
            }
            return res;
        }

        public List<CastLog> getCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)
        {
            List<CastLog> res = new List<CastLog>();
            foreach (Minion minion in this)
            {
                res.AddRange(minion.getCastLogs( bossData, combatList, agentData, start, end));
            }
            return res;
        }

        public ushort getInstid()
        {
            if (Count > 0)
            {
                return this[0].getInstid();
            }
            return 0;
        }

        public string getCharacter()
        {
            if (Count > 0)
            {
                return this[0].getCharacter();
            }
            return "";
        }

    }
}
