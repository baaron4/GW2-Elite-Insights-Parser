using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    internal class StatusEventsContainer
    {
        public Dictionary<AgentItem, List<AliveEvent>> AliveEvents { get; } = new Dictionary<AgentItem, List<AliveEvent>>();
        public Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEvents { get; } = new Dictionary<AgentItem, List<AttackTargetEvent>>();
        public Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEventsByAttackTarget { get; } = new Dictionary<AgentItem, List<AttackTargetEvent>>();
        public Dictionary<AgentItem, List<DeadEvent>> DeadEvents { get; } = new Dictionary<AgentItem, List<DeadEvent>>();
        public Dictionary<AgentItem, List<DespawnEvent>> DespawnEvents { get; } = new Dictionary<AgentItem, List<DespawnEvent>>();
        public Dictionary<AgentItem, List<DownEvent>> DownEvents { get; } = new Dictionary<AgentItem, List<DownEvent>>();
        public Dictionary<AgentItem, List<EnterCombatEvent>> EnterCombatEvents { get; } = new Dictionary<AgentItem, List<EnterCombatEvent>>();
        public Dictionary<AgentItem, List<ExitCombatEvent>> ExitCombatEvents { get; } = new Dictionary<AgentItem, List<ExitCombatEvent>>();
        public Dictionary<AgentItem, List<HealthUpdateEvent>> HealthUpdateEvents { get; } = new Dictionary<AgentItem, List<HealthUpdateEvent>>();
        public Dictionary<AgentItem, List<BarrierUpdateEvent>> BarrierUpdateEvents { get; } = new Dictionary<AgentItem, List<BarrierUpdateEvent>>();
        public Dictionary<AgentItem, List<MaxHealthUpdateEvent>> MaxHealthUpdateEvents { get; } = new Dictionary<AgentItem, List<MaxHealthUpdateEvent>>();
        public Dictionary<AgentItem, List<SpawnEvent>> SpawnEvents { get; } = new Dictionary<AgentItem, List<SpawnEvent>>();
        public Dictionary<AgentItem, List<TargetableEvent>> TargetableEvents { get; } = new Dictionary<AgentItem, List<TargetableEvent>>();
        public Dictionary<AgentItem, List<TeamChangeEvent>> TeamChangeEvents { get; } = new Dictionary<AgentItem, List<TeamChangeEvent>>();
        public Dictionary<AgentItem, List<BreakbarStateEvent>> BreakbarStateEvents { get; } = new Dictionary<AgentItem, List<BreakbarStateEvent>>();
        public Dictionary<AgentItem, List<BreakbarPercentEvent>> BreakbarPercentEvents { get; } = new Dictionary<AgentItem, List<BreakbarPercentEvent>>();
        public Dictionary<AgentItem, List<AbstractMovementEvent>> MovementEvents { get; } = new Dictionary<AgentItem, List<AbstractMovementEvent>>();
        public Dictionary<AgentItem, List<EffectEvent>> EffectEventsBySrc { get; } = new Dictionary<AgentItem, List<EffectEvent>>();
        public Dictionary<AgentItem, List<EffectEvent>> EffectEventsByDst { get; } = new Dictionary<AgentItem, List<EffectEvent>>();
        public List<EffectEvent> EffectEvents { get; } = new List<EffectEvent>();
        public Dictionary<long, List<EffectEvent>> EffectEventsByEffectID { get; } = new Dictionary<long, List<EffectEvent>>();
        public Dictionary<long, List<EffectEvent>> EffectEventsByTrackingID { get; } = new Dictionary<long, List<EffectEvent>>();
        public Dictionary<AgentItem, List<MarkerEvent>> MarkerEvents { get; } = new Dictionary<AgentItem, List<MarkerEvent>>();
        public Dictionary<long, List<MarkerEvent>> MarkerEventsByID { get; } = new Dictionary<long, List<MarkerEvent>>();
        public Dictionary<SquadMarkerIndex, List<SquadMarkerEvent>> SquadMarkerEventsByIndex { get; } = new Dictionary<SquadMarkerIndex, List<SquadMarkerEvent>>();
        public Dictionary<AgentItem, List<Last90BeforeDownEvent>> Last90BeforeDownEventsBySrc { get; } = new Dictionary<AgentItem, List<Last90BeforeDownEvent>>();
        public List<Last90BeforeDownEvent> Last90BeforeDownEvents { get; } = new List<Last90BeforeDownEvent>();
        public Dictionary<AgentItem, List<GliderEvent>> GliderEventsBySrc { get; } = new Dictionary<AgentItem, List<GliderEvent>>();
        public Dictionary<AgentItem, List<StunBreakEvent>> StunBreakEventsBySrc { get; } = new Dictionary<AgentItem, List<StunBreakEvent>>();

    }
}
