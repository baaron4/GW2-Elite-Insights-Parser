using LuckParser.Parser;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Minions : List<Minion>
    {
        public readonly int MinionID;
        private List<DamageLog> _damageLogs;
        private Dictionary<AgentItem, List<DamageLog>> _damageLogsByDst;
        private List<CastLog> _castLogs;
        public string Character => Count > 0 ? this[0].Character : "";

        public Minions(int id)
        {
            MinionID = id;
        }

        public List<DamageLog> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (_damageLogs == null)
            {
                _damageLogs = new List<DamageLog>();
                foreach (Minion minion in this)
                {
                    _damageLogs.AddRange(minion.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
                }
                _damageLogsByDst = _damageLogs.GroupBy(x => log.AgentData.GetAgentByInstID(x.DstInstId, log.FightData.ToLogSpace(x.Time))).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && _damageLogsByDst.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return _damageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
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
            if (_castLogs == null)
            {
                _castLogs = new List<CastLog>();
                foreach (Minion minion in this)
                {
                    _castLogs.AddRange(minion.GetCastLogs(log, 0, log.FightData.FightDuration));
                }
                _castLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            return _castLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

    }
}
