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

        public List<DamageLog> GetDamageLogs(int instidFilter, ParsedLog log, long start, long end)
        {
            if (damage_logs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    damage_logs.AddRange(minion.GetDamageLogs(0, log, 0, log.GetBossData().GetAwareDuration()));
                }
            }
            if (filtered_damage_logs.Count == 0)
            {
                filtered_damage_logs = damage_logs.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).ToList();
            }
            if (instidFilter > 0)
            {
                return filtered_damage_logs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();
            }
            return damage_logs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();
        }

        public List<DamageLog> GetDamageLogs(List<AgentItem> redirection, ParsedLog log, long start, long end)
        {
            List<DamageLog> dls = GetDamageLogs(0, log, start, end);
            List<DamageLog> res = new List<DamageLog>();
            foreach (AgentItem a in redirection)
            {
                res.AddRange(dls.Where(x => x.GetDstInstidt() == a.GetInstid() && x.GetTime() >= a.GetFirstAware() - log.GetBossData().GetFirstAware() && x.GetTime() <= a.GetLastAware() - log.GetBossData().GetFirstAware()));
            }
            res.Sort((x, y) => x.GetTime() < y.GetTime() ? -1 : 1);
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

        public List<CastLog> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    cast_logs.AddRange(minion.GetCastLogs(log, 0, log.GetBossData().GetAwareDuration()));
                }
            }
            return cast_logs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();
        }

        public int GetInstid()
        {
            return instid;
        }

        public string GetCharacter()
        {
            if (Count > 0)
            {
                return this[0].GetCharacter();
            }
            return "";
        }

    }
}
