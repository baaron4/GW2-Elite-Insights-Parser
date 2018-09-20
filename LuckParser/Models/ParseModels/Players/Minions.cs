using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Minions : List<Minion>
    {
        public readonly int InstID;
        private readonly List<DamageLog> _damageLogs = new List<DamageLog>();
        private List<DamageLog> _filteredDamageLogs = new List<DamageLog>();
        private readonly List<CastLog> _castLogs = new List<CastLog>();
        public string Character
        {
            get
            {
                return Count > 0 ? this[0].Character : "";
            }
        }

        public Minions(int instid)
        {
            InstID = instid;
        }

        public List<DamageLog> GetDamageLogs(int instidFilter, ParsedLog log, long start, long end)
        {
            if (_damageLogs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    _damageLogs.AddRange(minion.GetDamageLogs(0, log, 0, log.FightData.FightDuration));
                }
            }
            if (_filteredDamageLogs.Count == 0)
            {
                _filteredDamageLogs = _damageLogs.Where(x => x.DstInstId == log.Boss.InstID).ToList();
            }
            if (instidFilter > 0)
            {
                return _filteredDamageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return _damageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public List<DamageLog> GetDamageLogs(List<AgentItem> redirection, ParsedLog log, long start, long end)
        {
            List<DamageLog> dls = GetDamageLogs(0, log, start, end);
            List<DamageLog> res = new List<DamageLog>();
            foreach (AgentItem a in redirection)
            {
                res.AddRange(dls.Where(x => x.DstInstId == a.InstID && x.Time >= a.FirstAware - log.FightData.FightStart && x.Time <= a.LastAware - log.FightData.FightStart));
            }
            res.Sort((x, y) => x.Time < y.Time ? -1 : 1);
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
            if (_castLogs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    _castLogs.AddRange(minion.GetCastLogs(log, 0, log.FightData.FightDuration));
                }
            }
            return _castLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

    }
}
