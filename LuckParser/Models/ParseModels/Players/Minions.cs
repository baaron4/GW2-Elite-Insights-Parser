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
                    damage_logs.AddRange(minion.getDamageLogs(0, log, 0, log.getBossData().getAwareDuration()));
                }
            }
            if (filtered_damage_logs.Count == 0)
            {
                filtered_damage_logs = damage_logs.Where(x => x.getDstInstidt() == log.getBossData().getInstid()).ToList();
            }
            if (instidFilter > 0)
            {
                return filtered_damage_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
            }
            return damage_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
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
                    cast_logs.AddRange(minion.getCastLogs(log, 0, log.getBossData().getAwareDuration()));
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
                return this[0].getCharacter();
            }
            return "";
        }

    }
}
