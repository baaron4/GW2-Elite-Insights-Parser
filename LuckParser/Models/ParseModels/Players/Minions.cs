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
                    damage_logs.AddRange(minion.getDamageLogs(instidFilter, log, start, end));
                }
            }
            return damage_logs;
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
                    cast_logs.AddRange(minion.getCastLogs(log, start, end));
                }
            }
            return cast_logs;
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
