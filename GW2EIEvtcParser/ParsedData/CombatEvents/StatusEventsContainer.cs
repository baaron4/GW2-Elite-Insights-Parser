using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

internal class StatusEventsContainer
{
    public readonly Dictionary<AgentItem, List<AliveEvent>> AliveEvents = new();
    public readonly Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEvents = new();
    public readonly Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEventsByAttackTarget = new();
    public readonly Dictionary<AgentItem, List<DeadEvent>> DeadEvents = new();
    public readonly Dictionary<AgentItem, List<DespawnEvent>> DespawnEvents = new();
    public readonly Dictionary<AgentItem, List<DownEvent>> DownEvents = new();
    public readonly Dictionary<AgentItem, List<EnterCombatEvent>> EnterCombatEvents = new();
    public readonly Dictionary<AgentItem, List<ExitCombatEvent>> ExitCombatEvents = new();
    public readonly Dictionary<AgentItem, List<HealthUpdateEvent>> HealthUpdateEvents = new();
    public readonly Dictionary<AgentItem, List<BarrierUpdateEvent>> BarrierUpdateEvents = new();
    public readonly Dictionary<AgentItem, List<MaxHealthUpdateEvent>> MaxHealthUpdateEvents = new();
    public readonly Dictionary<AgentItem, List<SpawnEvent>> SpawnEvents = new();
    public readonly Dictionary<AgentItem, List<TargetableEvent>> TargetableEvents = new();
    public readonly Dictionary<AgentItem, List<TeamChangeEvent>> TeamChangeEvents = new();
    public readonly Dictionary<AgentItem, List<BreakbarStateEvent>> BreakbarStateEvents = new();
    public readonly Dictionary<AgentItem, List<BreakbarPercentEvent>> BreakbarPercentEvents = new();
    public readonly Dictionary<AgentItem, List<AbstractMovementEvent>> MovementEvents = new();
    public readonly Dictionary<AgentItem, List<EffectEvent>> EffectEventsBySrc = new();
    public readonly Dictionary<AgentItem, List<EffectEvent>> EffectEventsByDst = new();
    public readonly List<EffectEvent> EffectEvents = new();
    public readonly Dictionary<long, List<EffectEvent>> EffectEventsByEffectID = new();
    public readonly Dictionary<long, List<EffectEvent>> EffectEventsByTrackingID = new();
    public readonly List<MarkerEvent> MarkerEvents = new();
    public readonly Dictionary<AgentItem, List<MarkerEvent>> MarkerEventsBySrc = new();
    public readonly Dictionary<long, List<MarkerEvent>> MarkerEventsByID = new();
    public readonly Dictionary<SquadMarkerIndex, List<SquadMarkerEvent>> SquadMarkerEventsByIndex = new();
    public readonly Dictionary<AgentItem, List<Last90BeforeDownEvent>> Last90BeforeDownEventsBySrc = new();
    public readonly List<Last90BeforeDownEvent> Last90BeforeDownEvents = new();
    public readonly Dictionary<AgentItem, List<GliderEvent>> GliderEventsBySrc = new();
    public readonly Dictionary<AgentItem, List<StunBreakEvent>> StunBreakEventsBySrc = new();

}
