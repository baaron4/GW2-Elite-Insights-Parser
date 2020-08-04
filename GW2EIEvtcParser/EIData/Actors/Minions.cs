using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class Minions : AbstractActor
    {
        public List<NPC> MinionList { get; }
        public AbstractSingleActor Master { get; }

        internal Minions(AbstractSingleActor master, NPC firstMinion) : base(firstMinion.AgentItem)
        {
            MinionList = new List<NPC> { firstMinion };
            Master = master;
        }

        internal void AddMinion(NPC minion)
        {
            MinionList.Add(minion);
        }

        public override List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<AbstractDamageEvent>();
                foreach (NPC minion in MinionList)
                {
                    DamageLogs.AddRange(minion.GetDamageLogs(null, log, 0, log.FightData.FightEnd));
                }
                DamageLogsByDst = DamageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && DamageLogsByDst.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return DamageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        /*public List<DamageLog> getHealingLogs(ParsedEvtcLog log, long start, long end)
        {
            List<DamageLog> res = new List<DamageLog>();
            foreach (Minion minion in this)
            {
                res.AddRange(minion.getHealingLogs(log, start, end));
            }
            return res;
        }*/

        public override List<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageTakenlogs == null)
            {
                DamageTakenlogs = new List<AbstractDamageEvent>();
                foreach (NPC minion in MinionList)
                {
                    DamageTakenlogs.AddRange(minion.GetDamageTakenLogs(null, log, 0, log.FightData.FightEnd));
                }
                DamageTakenLogsBySrc = DamageTakenlogs.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && DamageTakenLogsBySrc.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return DamageTakenlogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        private void InitCastLogs(ParsedEvtcLog log)
        {
            CastLogs = new List<AbstractCastEvent>();
            foreach (NPC minion in MinionList)
            {
                CastLogs.AddRange(minion.GetCastLogs(log, 0, log.FightData.FightEnd));
            }
            CastLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }

        public override List<AbstractCastEvent> GetCastLogs(ParsedEvtcLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                InitCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractCastEvent> GetIntersectingCastLogs(ParsedEvtcLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                InitCastLogs(log);
            }
            return CastLogs.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();
        }

        public List<List<Segment>> GetLifeSpanSegments(ParsedEvtcLog log)
        {
            var minionsSegments = new List<List<Segment>>();
            var fightDur = log.FightData.FightEnd;
            foreach (NPC minion in MinionList)
            {
                (List<(long start, long end)> deads, List<(long start, long end)> downs, List<(long start, long end)> dcs) = minion.GetStatus(log);
                var minionSegments = new List<Segment>();
                var off = new List<(long start, long end)>();
                off.AddRange(deads);
                off.AddRange(downs);
                off.AddRange(dcs);
                off.Sort((x, y) => x.start.CompareTo(y.start));
                if (off.Any())
                {
                    long presenceStart = 0;
                    foreach ((long start, long end) in off)
                    {
                        minionSegments.Add(new Segment(presenceStart, start, 1));
                        minionSegments.Add(new Segment(start, end, 0));
                        presenceStart = end;
                    }
                    minionSegments.Add(new Segment(presenceStart, fightDur, 1));
                } 
                else
                {
                    long start = Math.Max(minion.FirstAware, 0);
                    long end = Math.Min(minion.LastAware, fightDur);
                    minionSegments.Add(new Segment(0, start, 0));
                    minionSegments.Add(new Segment(start, end, 1));
                    minionSegments.Add(new Segment(end, fightDur, 0));
                }
                minionsSegments.Add(minionSegments);
            }
            return minionsSegments;
        }
    }
}
