using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class Minions : List<Minion>
    {
        private int instid;
        private List<DamageLog> damage_logs = new List<DamageLog>();
        private List<DamageLog> filtered_damage_logs = new List<DamageLog>();
        private List<CastLog> cast_logs = new List<CastLog>();
        public Minions(int instid) : base()
        {
            this.instid = instid;
        }

        public List<DamageLog> getDamageLogs(int instidFilter, ParsedLog log, long start, long end)
        {
            if (damage_logs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    damage_logs.AddRange(minion.GetDamageLogs(0, log, 0, log.GetBossData().getAwareDuration()));
                }
            }
            if (filtered_damage_logs.Count == 0)
            {
                filtered_damage_logs = damage_logs.Where(x => x.getDstInstidt() == log.GetBossData().getInstid()).ToList();
            }
            if (instidFilter > 0)
            {
                return filtered_damage_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
            }
            return damage_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }

        public List<DamageLog> getDamageLogs(List<AgentItem> redirection, ParsedLog log, long start, long end)
        {
            List<DamageLog> dls = getDamageLogs(0, log, start, end);
            List<DamageLog> res = new List<DamageLog>();
            foreach (AgentItem a in redirection)
            {
                res.AddRange(dls.Where(x => x.getDstInstidt() == a.getInstid() && x.getTime() >= a.getFirstAware() - log.GetBossData().getFirstAware() && x.getTime() <= a.getLastAware() - log.GetBossData().getFirstAware()));
            }
            res.Sort((x, y) => x.getTime() < y.getTime() ? -1 : 1);
            return res;
        }

        /*public List<DamageLog> getHealingLogs(ParsedLog log, long start, long end)
        {
            List<DamageLog> res = new List<DamageLog>();
            foreach (Minion minion in this)
            {
                res.AddRange(minion.getHealingLogs(log, start, end));
            }
            return res;
        }*/

        public List<CastLog> getCastLogs(ParsedLog log, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    cast_logs.AddRange(minion.GetCastLogs(log, 0, log.GetBossData().getAwareDuration()));
                }
            }
            return cast_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }

        public int getInstid()
        {
            return instid;
        }

        public string getCharacter()
        {
            if (Count > 0)
            {
                return this[0].GetCharacter();
            }
            return "";
        }

    }
}
