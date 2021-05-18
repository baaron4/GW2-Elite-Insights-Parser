using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractActor
    {
        protected AgentItem SourceAgent { get; }
        public string Character { get; protected set; }

        public uint Toughness => SourceAgent.Toughness;
        public uint Condition => SourceAgent.Condition;
        public uint Concentration => SourceAgent.Concentration;
        public uint Healing => SourceAgent.Healing;
        public ushort InstID => SourceAgent.InstID;
        public string Prof => SourceAgent.Prof;
        public ulong Agent => SourceAgent.Agent;
        public long LastAware => SourceAgent.LastAware;
        public long FirstAware => SourceAgent.FirstAware;
        public int ID => SourceAgent.ID;
        public uint HitboxHeight => SourceAgent.HitboxHeight;
        public uint HitboxWidth => SourceAgent.HitboxWidth;
        public bool IsFakeActor => IsDummyActor || IsCustomActor;
        public bool IsDummyActor => SourceAgent.IsDummy;
        public bool IsCustomActor { get; protected set; } = false;
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
            SourceAgent = agent;
        }
        // Getters
        // Damage logs
        public abstract IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        public abstract IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        /// <summary>
        /// cached method for damage modifiers
        /// </summary>
        internal IReadOnlyList<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
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

        internal IReadOnlyList<AbstractHealthDamageEvent> GetConditionHitDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_conditionHitDamageEventsPerPhasePerTarget == null)
            {
                _conditionHitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_conditionHitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetHitDamageEvents(target, log, start, end).Where(x => x.ConditionDamageBased(log)).ToList();
                _conditionHitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        internal IReadOnlyList<AbstractHealthDamageEvent> GetPowerHitDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_powerHitDamageEventsPerPhasePerTarget == null)
            {
                _powerHitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_powerHitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetHitDamageEvents(target, log, start, end).Where(x => !x.ConditionDamageBased(log)).ToList();
                _powerHitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
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
