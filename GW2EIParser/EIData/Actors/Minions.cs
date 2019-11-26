using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class Minions : AbstractActor
    {
        public List<NPC> MinionList { get; }
        public AbstractSingleActor Master { get; }

        public Minions(AbstractSingleActor master, NPC firstMinion) : base(firstMinion.AgentItem)
        {
            MinionList = new List<NPC> { firstMinion };
            Master = master;
        }

        public void AddMinion(NPC minion)
        {
            MinionList.Add(minion);
        }

        public override List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<AbstractDamageEvent>();
                foreach (NPC minion in MinionList)
                {
                    DamageLogs.AddRange(minion.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
                }
                DamageLogsByDst = DamageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && DamageLogsByDst.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return DamageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
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

        public override List<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageTakenlogs == null)
            {
                DamageTakenlogs = new List<AbstractDamageEvent>();
                foreach (NPC minion in MinionList)
                {
                    DamageTakenlogs.AddRange(minion.GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration));
                }
                DamageTakenLogsBySrc = DamageTakenlogs.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && DamageTakenLogsBySrc.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return DamageTakenlogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractCastEvent> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                CastLogs = new List<AbstractCastEvent>();
                foreach (NPC minion in MinionList)
                {
                    CastLogs.AddRange(minion.GetCastLogs(log, 0, log.FightData.FightDuration));
                }
                CastLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractCastEvent> GetCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                CastLogs = new List<AbstractCastEvent>();
                foreach (NPC minion in MinionList)
                {
                    CastLogs.AddRange(minion.GetCastLogs(log, 0, log.FightData.FightDuration));
                }
                CastLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            return CastLogs.Where(x => x.Time >= start && x.Time + x.ActualDuration <= end).ToList();
        }
    }
}
