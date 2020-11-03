using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

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
        protected List<AbstractDamageEvent> DamageLogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractDamageEvent>> DamageLogsByDst { get; set; }
        private Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractDamageEvent>>> _damageLogsPerPhasePerTarget { get; } = new Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractDamageEvent>>>();
        protected List<AbstractDamageEvent> DamageTakenlogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractDamageEvent>> DamageTakenLogsBySrc { get; set; }
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
        public abstract IReadOnlyList<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end);

        /// <summary>
        /// cached method for damage modifiers
        /// </summary>
        internal IReadOnlyList<AbstractDamageEvent> GetHitDamageLogs(AbstractActor target, ParsedEvtcLog log, PhaseData phase)
        {
            if (!_damageLogsPerPhasePerTarget.TryGetValue(phase, out Dictionary<AbstractActor, List<AbstractDamageEvent>> targetDict))
            {
                targetDict = new Dictionary<AbstractActor, List<AbstractDamageEvent>>();
                _damageLogsPerPhasePerTarget[phase] = targetDict;
            }
            if (!targetDict.TryGetValue(target ?? ParserHelper._nullActor, out List<AbstractDamageEvent> dls))
            {
                dls = GetDamageLogs(target, log, phase.Start, phase.End).Where(x => x.HasHit).ToList();
                targetDict[target ?? ParserHelper._nullActor] = dls;
            }
            return dls;
        }

        public abstract IReadOnlyList<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedEvtcLog log, long start, long end);

        // Cast logs
        public abstract IReadOnlyList<AbstractCastEvent> GetCastLogs(ParsedEvtcLog log, long start, long end);
        public abstract IReadOnlyList<AbstractCastEvent> GetIntersectingCastLogs(ParsedEvtcLog log, long start, long end);
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
