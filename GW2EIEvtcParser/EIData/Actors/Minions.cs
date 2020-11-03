using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class Minions : AbstractActor
    {
        private List<NPC> _minionList { get; }

        public IReadOnlyList<NPC> MinionList => _minionList;
        public AbstractSingleActor Master { get; }

        internal Minions(AbstractSingleActor master, NPC firstMinion) : base(firstMinion.AgentItem)
        {
            _minionList = new List<NPC> { firstMinion };
            Master = master;
        }

        internal void AddMinion(NPC minion)
        {
            _minionList.Add(minion);
        }

        public override IReadOnlyList<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<AbstractDamageEvent>();
                foreach (NPC minion in _minionList)
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

        public override IReadOnlyList<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageTakenlogs == null)
            {
                DamageTakenlogs = new List<AbstractDamageEvent>();
                foreach (NPC minion in _minionList)
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
            foreach (NPC minion in _minionList)
            {
                CastLogs.AddRange(minion.GetCastLogs(log, 0, log.FightData.FightEnd));
            }
            CastLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }

        public override IReadOnlyList<AbstractCastEvent> GetCastLogs(ParsedEvtcLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                InitCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractCastEvent> GetIntersectingCastLogs(ParsedEvtcLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                InitCastLogs(log);
            }
            return CastLogs.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();
        }

        public IReadOnlyList<IReadOnlyList<Segment>> GetLifeSpanSegments(ParsedEvtcLog log)
        {
            var minionsSegments = new List<List<Segment>>();
            var fightDur = log.FightData.FightEnd;
            foreach (NPC minion in _minionList)
            {
                var minionSegments = new List<Segment>();
                long start = Math.Max(minion.FirstAware, 0);
                // Find end
                long end = minion.LastAware;
                DownEvent down = log.CombatData.GetDownEvents(minion.AgentItem).LastOrDefault();
                if (down != null)
                {
                    end = Math.Min(down.Time, end);
                }
                DeadEvent dead = log.CombatData.GetDeadEvents(minion.AgentItem).LastOrDefault();
                if (dead != null)
                {
                    end = Math.Min(dead.Time, end);
                }
                DespawnEvent despawn = log.CombatData.GetDespawnEvents(minion.AgentItem).LastOrDefault();
                if (despawn != null)
                {
                    end = Math.Min(despawn.Time, end);
                }
                //
                end = Math.Min(end, fightDur);
                minionSegments.Add(new Segment(0, start, 0));
                minionSegments.Add(new Segment(start, end, 1));
                minionSegments.Add(new Segment(end, fightDur, 0));
                minionsSegments.Add(minionSegments);
            }
            return minionsSegments;
        }
    }
}
