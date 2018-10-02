using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Minions : List<Minion>
    {
        public readonly int MinionID;
        private readonly List<DamageLog> _damageLogs = new List<DamageLog>();
        private Dictionary<ushort, List<DamageLog>> _damageLogsByDst = new Dictionary<ushort, List<DamageLog>>();
        private readonly List<CastLog> _castLogs = new List<CastLog>();
        public string Character => Count > 0 ? this[0].Character : "";

        public Minions(int id)
        {
            MinionID = id;
        }

        public List<DamageLog> GetDamageLogs(AbstractPlayer target, ParsedLog log, long start, long end)
        {
            if (_damageLogs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    _damageLogs.AddRange(minion.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
                }
                _damageLogsByDst = _damageLogs.GroupBy(x => x.DstInstId).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && _damageLogsByDst.TryGetValue(target.InstID, out var list))
            {
                long targetStart = target.FirstAware - log.FightData.FightStart;
                long targetEnd = target.LastAware - log.FightData.FightStart;
                return list.Where(x => x.Time >= start && x.Time > targetStart && x.Time <= end && x.Time < targetEnd).ToList();
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
