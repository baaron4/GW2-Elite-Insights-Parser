using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class Minions : AbstractActor
    {
        private List<NPC> _minionList { get; }
        public AgentItem ReferenceAgentItem => AgentItem;

        public IReadOnlyList<NPC> MinionList => _minionList;
        public AbstractSingleActor Master { get; }
        public EXTMinionsHealingHelper EXTHealing { get; }
        public EXTMinionsBarrierHelper EXTBarrier { get; }

        internal Minions(AbstractSingleActor master, NPC firstMinion) : base(firstMinion.AgentItem)
        {
            _minionList = new List<NPC> { firstMinion };
            Master = master;
            EXTHealing = new EXTMinionsHealingHelper(this);
            EXTBarrier = new EXTMinionsBarrierHelper(this);
            Character = firstMinion.Character;
#if DEBUG
            Character += " (" + ID + ")";
#endif
        }

        internal void AddMinion(NPC minion)
        {
            _minionList.Add(minion);
        }

        public override IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageEvents == null)
            {
                DamageEvents = new List<AbstractHealthDamageEvent>();
                foreach (NPC minion in _minionList)
                {
                    DamageEvents.AddRange(minion.GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                DamageEvents = DamageEvents.OrderBy(x => x.Time).ToList();
                DamageEventByDst = DamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageEventByDst.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (BreakbarDamageEvents == null)
            {
                BreakbarDamageEvents = new List<AbstractBreakbarDamageEvent>();
                foreach (NPC minion in _minionList)
                {
                    BreakbarDamageEvents.AddRange(minion.GetBreakbarDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                BreakbarDamageEvents = BreakbarDamageEvents.OrderBy(x => x.Time).ToList();
                BreakbarDamageEventsByDst = BreakbarDamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageEventsByDst.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }

            return BreakbarDamageEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
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

        public override IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageTakenEvents == null)
            {
                DamageTakenEvents = new List<AbstractHealthDamageEvent>();
                foreach (NPC minion in _minionList)
                {
                    DamageTakenEvents.AddRange(minion.GetDamageTakenEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                DamageTakenEvents = DamageTakenEvents.OrderBy(x => x.Time).ToList();
                DamageTakenEventsBySrc = DamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (BreakbarDamageTakenEvents == null)
            {
                BreakbarDamageTakenEvents = new List<AbstractBreakbarDamageEvent>();
                foreach (NPC minion in _minionList)
                {
                    BreakbarDamageTakenEvents.AddRange(minion.GetBreakbarDamageTakenEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                BreakbarDamageTakenEvents = BreakbarDamageTakenEvents.OrderBy(x => x.Time).ToList();
                BreakbarDamageTakenEventsBySrc = BreakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        private void InitCastEvents(ParsedEvtcLog log)
        {
            CastEvents = new List<AbstractCastEvent>();
            foreach (NPC minion in _minionList)
            {
                CastEvents.AddRange(minion.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd));
            }
            CastEvents = CastEvents.OrderBy(x => x.Time).ThenBy(x => x.Skill.IsSwap).ToList();
        }

        public override IReadOnlyList<AbstractCastEvent> GetCastEvents(ParsedEvtcLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                InitCastEvents(log);
            }
            return CastEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractCastEvent> GetIntersectingCastEvents(ParsedEvtcLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                InitCastEvents(log);
            }
            return CastEvents.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();
        }

        internal bool IsActive(ParsedEvtcLog log)
        {
            if (log.CombatData.HasEXTHealing && EXTHealing.GetOutgoingHealEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd).Any())
            {
                return true;
            }
            if (ParserHelper.IsKnownMinionID(ReferenceAgentItem, Master.Spec))
            {
                return true;
            }
            if (log.CombatData.HasEXTBarrier && EXTBarrier.GetOutgoingBarrierEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd).Any())
            {
                return true;
            }
            if (GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd).Any())
            {
                return true;
            }
            if (GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Any(x => x.SkillId != SkillIDs.WeaponSwap && x.SkillId != SkillIDs.MirageCloakDodge))
            {
                return true;
            }
            /*if (_minionList[0] is NPC && _minionList.Any(x => log.CombatData.GetMovementData(x.AgentItem).Any()))
            {
                return true;
            }*/
            return false;
        }

        internal IReadOnlyList<IReadOnlyList<Segment>> GetLifeSpanSegments(ParsedEvtcLog log)
        {
            var minionsSegments = new List<List<Segment>>();
            long fightEnd = log.FightData.FightEnd;
            foreach (NPC minion in _minionList)
            {
                var minionSegments = new List<Segment>();
                long start = Math.Max(minion.FirstAware, 0);
                // Find end
                long end = minion.LastAware;
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
                end = Math.Min(end, fightEnd);
                minionSegments.Add(new Segment(log.FightData.FightStart, start, 0));
                minionSegments.Add(new Segment(start, end, 1));
                minionSegments.Add(new Segment(end, fightEnd, 0));
                minionSegments.RemoveAll(x => x.Start > x.End);
                minionsSegments.Add(minionSegments);
            }
            return minionsSegments;
        }
    }
}
