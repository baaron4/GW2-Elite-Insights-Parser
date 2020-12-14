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
        protected List<AbstractHealthDamageEvent> DamageLogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractHealthDamageEvent>> DamageLogsByDst { get; set; }
        private CachingCollection<List<AbstractHealthDamageEvent>> _hitDamageLogsPerPhasePerTarget;
        protected List<AbstractHealthDamageEvent> DamageTakenlogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractHealthDamageEvent>> DamageTakenLogsBySrc { get; set; }
        // Breakbar Damage
        protected List<AbstractBreakbarDamageEvent> BreakbarDamageLogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractBreakbarDamageEvent>> BreakbarDamageLogsByDst { get; set; }
        protected List<AbstractBreakbarDamageEvent> BreakbarDamageTakenLogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractBreakbarDamageEvent>> BreakbarDamageTakenLogsBySrc { get; set; }
        // Cast
        protected List<AbstractCastEvent> CastLogs { get; set; }

        protected AbstractActor(AgentItem agent)
        {
            string[] name = agent.Name.Split('\0');
            Character = name[0];
            AgentItem = agent;
        }
        // Getters
        // Damage logs
        public abstract List<AbstractHealthDamageEvent> GetDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end);

        public abstract List<AbstractBreakbarDamageEvent> GetBreakbarDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end);

        /// <summary>
        /// cached method for damage modifiers
        /// </summary>
        internal List<AbstractHealthDamageEvent> GetHitDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if(_hitDamageLogsPerPhasePerTarget == null)
            {
                _hitDamageLogsPerPhasePerTarget = new CachingCollection<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_hitDamageLogsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetDamageLogs(target, log, start, end).Where(x => x.HasHit).ToList();
                _hitDamageLogsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        public abstract List<AbstractHealthDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedEvtcLog log, long start, long end);

        public abstract List<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenLogs(AbstractActor target, ParsedEvtcLog log, long start, long end);

        // Cast logs
        public abstract List<AbstractCastEvent> GetCastLogs(ParsedEvtcLog log, long start, long end);
        public abstract List<AbstractCastEvent> GetIntersectingCastLogs(ParsedEvtcLog log, long start, long end);
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
