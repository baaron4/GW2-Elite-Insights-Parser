using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractActor
    {
        protected AgentItem AgentItem { get; }
        public string Character { get; protected set; }

        public int UniqueID => AgentItem.UniqueID;
        public uint Toughness => AgentItem.Toughness;
        public uint Condition => AgentItem.Condition;
        public uint Concentration => AgentItem.Concentration;
        public uint Healing => AgentItem.Healing;
        public ParserHelper.Spec Spec => AgentItem.Spec;
        public ParserHelper.Spec BaseSpec => AgentItem.BaseSpec;
        public long LastAware => AgentItem.LastAware;
        public long FirstAware => AgentItem.FirstAware;
        public int ID => AgentItem.ID;
        public uint HitboxHeight => AgentItem.HitboxHeight;
        public uint HitboxWidth => AgentItem.HitboxWidth;
        public bool IsFakeActor => AgentItem.IsFake;
        // Damage
        protected List<AbstractHealthDamageEvent> DamageEvents { get; set; }
        protected Dictionary<AgentItem, List<AbstractHealthDamageEvent>> DamageEventByDst { get; set; }
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>> _typedHitDamageEvents = new Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>>();
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>> _typedHitDamageTakenEvents = new Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>>();
        protected List<AbstractHealthDamageEvent> DamageTakenEvents { get; set; }
        protected Dictionary<AgentItem, List<AbstractHealthDamageEvent>> DamageTakenEventsBySrc { get; set; }
        // Breakbar Damage
        protected List<AbstractBreakbarDamageEvent> BreakbarDamageEvents { get; set; }
        protected Dictionary<AgentItem, List<AbstractBreakbarDamageEvent>> BreakbarDamageEventsByDst { get; set; }
        protected List<AbstractBreakbarDamageEvent> BreakbarDamageTakenEvents { get; set; }
        protected Dictionary<AgentItem, List<AbstractBreakbarDamageEvent>> BreakbarDamageTakenEventsBySrc { get; set; }
        // Cast
        protected List<AbstractCastEvent> CastEvents { get; set; }

        protected AbstractActor(AgentItem agent)
        {
            string[] name = agent.Name.Split('\0');
            Character = name[0];
            AgentItem = agent;
        }

        public bool IsSpecies(int id)
        {
            return AgentItem.IsSpecies(id);
        }

        public bool IsSpecies(ArcDPSEnums.TrashID id)
        {
            return AgentItem.IsSpecies(id);
        }

        public bool IsSpecies(ArcDPSEnums.TargetID id)
        {
            return AgentItem.IsSpecies(id);
        }

        public bool IsSpecies(ArcDPSEnums.MinionID id)
        {
            return AgentItem.IsSpecies(id);
        }

        public bool IsSpecies(ArcDPSEnums.ChestID id)
        {
            return AgentItem.IsSpecies(id);
        }

        public bool IsAnySpecies(IEnumerable<ArcDPSEnums.TrashID> ids)
        {
            return AgentItem.IsAnySpecies(ids);
        }

        public bool IsAnySpecies(IEnumerable<ArcDPSEnums.TargetID> ids)
        {
            return AgentItem.IsAnySpecies(ids);
        }

        public bool IsAnySpecies(IEnumerable<ArcDPSEnums.MinionID> ids)
        {
            return AgentItem.IsAnySpecies(ids);
        }

        public bool IsAnySpecies(IEnumerable<ArcDPSEnums.ChestID> ids)
        {
            return AgentItem.IsAnySpecies(ids);
        }

        // Getters
        // Damage logs
        public abstract IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        public abstract IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        public IReadOnlyList<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end, ParserHelper.DamageType damageType)
        {
            if (!_typedHitDamageEvents.TryGetValue(damageType, out CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> hitDamageEventsPerPhasePerTarget))
            {
                hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
                _typedHitDamageEvents[damageType] = hitDamageEventsPerPhasePerTarget;
            }
            if (!hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetDamageEvents(target, log, start, end).Where(x => x.HasHit).ToList();
                switch (damageType)
                {
                    case ParserHelper.DamageType.Power:
                        dls.RemoveAll(x => x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.Strike:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent);
                        break;
                    case ParserHelper.DamageType.LifeLeech:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent ndhd && !ndhd.IsLifeLeech);
                        break;
                    case ParserHelper.DamageType.Condition:
                        dls.RemoveAll(x => !x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.StrikeAndCondition:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent && !x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.StrikeAndConditionAndLifeLeech:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent ndhd && !x.ConditionDamageBased(log) && !ndhd.IsLifeLeech);
                        break;
                    case ParserHelper.DamageType.All:
                        break;
                    default:
                        throw new NotImplementedException("Not implemented damage type " + damageType);
                }
                hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        public IReadOnlyList<AbstractHealthDamageEvent> GetHitDamageTakenEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end, ParserHelper.DamageType damageType)
        {
            if (!_typedHitDamageTakenEvents.TryGetValue(damageType, out CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> hitDamageTakenEventsPerPhasePerTarget))
            {
                hitDamageTakenEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
                _typedHitDamageTakenEvents[damageType] = hitDamageTakenEventsPerPhasePerTarget;
            }
            if (!hitDamageTakenEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetDamageTakenEvents(target, log, start, end).Where(x => x.HasHit).ToList();
                switch (damageType)
                {
                    case ParserHelper.DamageType.Power:
                        dls.RemoveAll(x => x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.Strike:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent);
                        break;
                    case ParserHelper.DamageType.LifeLeech:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent ndhd && !ndhd.IsLifeLeech);
                        break;
                    case ParserHelper.DamageType.Condition:
                        dls.RemoveAll(x => !x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.StrikeAndCondition:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent && !x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.StrikeAndConditionAndLifeLeech:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent ndhd && !x.ConditionDamageBased(log) && !ndhd.IsLifeLeech);
                        break;
                    case ParserHelper.DamageType.All:
                        break;
                    default:
                        throw new NotImplementedException("Not implemented damage type " + damageType);
                }
                hitDamageTakenEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        public abstract IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        public abstract IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        // Cast logs
        public abstract IReadOnlyList<AbstractCastEvent> GetCastEvents(ParsedEvtcLog log, long start, long end);
        public abstract IReadOnlyList<AbstractCastEvent> GetIntersectingCastEvents(ParsedEvtcLog log, long start, long end);
        // privates

        protected static bool KeepIntersectingCastLog(AbstractCastEvent evt, long start, long end)
        {
            return (evt.Time >= start && evt.Time <= end) || // start inside
                (evt.EndTime >= start && evt.EndTime <= end) || // end inside
                (evt.Time <= start && evt.EndTime >= end); // start before, end after
        }
    }
}
