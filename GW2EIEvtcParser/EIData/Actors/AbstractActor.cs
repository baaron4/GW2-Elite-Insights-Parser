using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractActor
    {
        public AgentItem AgentItem { get; }
        public string Character { get; protected set; }

        public uint Toughness => AgentItem.Toughness;
        public uint Condition => AgentItem.Condition;
        public uint Concentration => AgentItem.Concentration;
        public uint Healing => AgentItem.Healing;
        public ushort InstID => AgentItem.InstID;
        public string Prof => AgentItem.Prof;
        public ulong Agent => AgentItem.Agent;
        public long LastAware => AgentItem.LastAware;
        public long FirstAware => AgentItem.FirstAware;
        public int ID => AgentItem.ID;
        public uint HitboxHeight => AgentItem.HitboxHeight;
        public uint HitboxWidth => AgentItem.HitboxWidth;
        public bool IsFakeActor { get; protected set; }
        // Damage
        protected List<AbstractHealthDamageEvent> DamageEvents { get; set; }
        protected Dictionary<AgentItem, List<AbstractHealthDamageEvent>> DamageEventByDst { get; set; }
        private CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> _hitDamageEventsPerPhasePerTarget;
        private CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> _powerHitDamageEventsPerPhasePerTarget;
        private CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> _conditionHitDamageEventsPerPhasePerTarget;
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
        // Getters
        // Damage logs
        public abstract List<AbstractHealthDamageEvent> GetDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end);

        public abstract List<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end);

        /// <summary>
        /// cached method for damage modifiers
        /// </summary>
        internal List<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if(_hitDamageEventsPerPhasePerTarget == null)
            {
                _hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetDamageEvents(target, log, start, end).Where(x => x.HasHit).ToList();
                _hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        internal List<AbstractHealthDamageEvent> GetConditionHitDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_conditionHitDamageEventsPerPhasePerTarget == null)
            {
                _conditionHitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_conditionHitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetHitDamageEvents(target, log, start, end).Where(x => x.IsCondi(log)).ToList();
                _conditionHitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        internal List<AbstractHealthDamageEvent> GetPowerHitDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_powerHitDamageEventsPerPhasePerTarget == null)
            {
                _powerHitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_powerHitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetHitDamageEvents(target, log, start, end).Where(x => !x.IsCondi(log)).ToList();
                _powerHitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        public abstract List<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractActor target, ParsedEvtcLog log, long start, long end);

        public abstract List<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenEvents(AbstractActor target, ParsedEvtcLog log, long start, long end);

        // Cast logs
        public abstract List<AbstractCastEvent> GetCastEvents(ParsedEvtcLog log, long start, long end);
        public abstract List<AbstractCastEvent> GetIntersectingCastEvents(ParsedEvtcLog log, long start, long end);
        // privates

        protected static bool KeepIntersectingCastLog(AbstractCastEvent evt, long start, long end)
        {
            return (evt.Time >= start && evt.Time <= end) || // start inside
                (evt.EndTime >= start && evt.EndTime <= end) || // end inside
                (evt.Time <= start && evt.EndTime >= end); // start before, end after
        }

        protected static void Add<T>(Dictionary<T, long> dictionary, T key, long value)
        {
            if (dictionary.TryGetValue(key, out long existing))
            {
                dictionary[key] = existing + value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
