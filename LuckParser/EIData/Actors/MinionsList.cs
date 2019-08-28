using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.EIData
{
    public class MinionsList : List<Minion>
    {
        public int MinionID { get; }
        private List<AbstractDamageEvent> _damageLogs;
        private Dictionary<AgentItem, List<AbstractDamageEvent>> _damageLogsByDst;
        private List<AbstractCastEvent> _castLogs;
        public string Character => Count > 0 ? this[0].Character : "";

        public MinionsList(int id)
        {
            MinionID = id;
        }

        public List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (_damageLogs == null)
            {
                _damageLogs = new List<AbstractDamageEvent>();
                foreach (Minion minion in this)
                {
                    _damageLogs.AddRange(minion.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
                }
                _damageLogsByDst = _damageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && _damageLogsByDst.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
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

        public List<AbstractCastEvent> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (_castLogs == null)
            {
                _castLogs = new List<AbstractCastEvent>();
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
