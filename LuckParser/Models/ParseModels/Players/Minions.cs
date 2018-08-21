using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Minions : List<Minion>
    {
        private int _instid;
        private List<DamageLog> _damageLogs = new List<DamageLog>();
        private List<DamageLog> _filteredDamageLogs = new List<DamageLog>();
        private List<CastLog> _castLogs = new List<CastLog>();
        public Minions(int instid) : base()
        {
            _instid = instid;
        }

        public List<DamageLog> GetDamageLogs(int instidFilter, ParsedLog log, long start, long end)
        {
            if (_damageLogs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    _damageLogs.AddRange(minion.GetDamageLogs(0, log, 0, log.GetBossData().GetAwareDuration()));
                }
            }
            if (_filteredDamageLogs.Count == 0)
            {
                _filteredDamageLogs = _damageLogs.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).ToList();
            }
            if (instidFilter > 0)
            {
                return _filteredDamageLogs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();
            }
            return _damageLogs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();
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
            if (_castLogs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    _castLogs.AddRange(minion.GetCastLogs(log, 0, log.GetBossData().GetAwareDuration()));
                }
            }
            return _castLogs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();
        }

        public int GetInstid()
        {
            return _instid;
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
