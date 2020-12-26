using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class NPC : AbstractSingleActor
    {
        private CachingCollection<Dictionary<long, FinalBuffs>> _buffs;
        private List<Segment> _breakbarPercentUpdates { get; set; }
        // Constructors
        internal NPC(AgentItem agent) : base(agent)
        {
        }

        private int _health = -1;

        public int GetHealth(CombatData combatData)
        {
            if (_health == -1)
            {
                IReadOnlyList<MaxHealthUpdateEvent> maxHpUpdates = combatData.GetMaxHealthUpdateEvents(AgentItem);
                _health = maxHpUpdates.Count > 0 ? maxHpUpdates.Max(x => x.MaxHealth) : 1;
            }
            return _health;
        }

        public IReadOnlyList<Segment> GetBreakbarPercentUpdates(ParsedEvtcLog log)
        {
            if (_breakbarPercentUpdates == null)
            {
                _breakbarPercentUpdates = Segment.FromStates(log.CombatData.GetBreakbarPercentEvents(AgentItem).Select(x => x.ToState()).ToList(), 0, log.FightData.FightEnd);
            }
            return _breakbarPercentUpdates;
        }

        internal void OverrideName(string name)
        {
            Character = name;
        }

        public override string GetIcon()
        {
            return AgentItem.Type == AgentItem.AgentType.EnemyPlayer ? ParserHelper.GetHighResolutionProfIcon(Prof) : ParserHelper.GetNPCIcon(ID);
        }

        internal void SetManualHealth(int health)
        {
            _health = health;
        }

        public IReadOnlyDictionary<long, FinalBuffs> GetBuffs(ParsedEvtcLog log, long start, long end)
        {
            if (_buffs == null)
            {
                _buffs = new CachingCollection<Dictionary<long, FinalBuffs>>(log);
            }
            if (!_buffs.TryGetValue(start, end, out Dictionary<long, FinalBuffs> value))
            {
                value = ComputeBuffs(log, start, end);
                _buffs.Set(start, end, value);
            }
            return value;
        }

        private Dictionary<long, FinalBuffs> ComputeBuffs(ParsedEvtcLog log, long start, long end)
        {
            BuffDistribution buffDistribution = GetBuffDistribution(log, start, end);
            var rates = new Dictionary<long, FinalBuffs>();
            Dictionary<long, long> buffPresence = GetBuffPresence(log, start, end);

            long phaseDuration = end - start;

            foreach (Buff buff in GetTrackedBuffs(log))
            {
                if (buffDistribution.HasBuffID(buff.ID))
                {
                    rates[buff.ID] = new FinalBuffs(buff, buffDistribution, buffPresence, phaseDuration);
                }
            }
            return rates;
        }

        protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log)
        {
            log.FightData.Logic.ComputeNPCCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any() && log.FightData.Logic.Targets.Contains(this))
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }


        //

        public override AbstractSingleActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return new NPCSerializable(this, log, map, CombatReplay);
        }

        protected override void InitCombatReplay(ParsedEvtcLog log)
        {
            CombatReplay = new CombatReplay();
            if (!log.CombatData.HasMovementData)
            {
                // no combat replay support on fight
                return;
            }
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightEnd);
            TrimCombatReplay(log);
        }
    }
}
