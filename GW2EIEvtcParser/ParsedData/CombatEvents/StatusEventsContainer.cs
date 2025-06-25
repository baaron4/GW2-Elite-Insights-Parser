using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

internal class StatusEventsContainer
{
    public readonly Dictionary<AgentItem, List<TargetableEvent>> TargetableEvents = [];
    public readonly List<AttackTargetEvent> AttackTargetEvents = [];
    public readonly Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEventsBySrc = [];
    public readonly Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEventsByAttackTarget = [];

    public readonly Dictionary<AgentItem, List<AliveEvent>> AliveEvents = [];
    public readonly Dictionary<AgentItem, List<DeadEvent>> DeadEvents = [];
    public readonly Dictionary<AgentItem, List<DownEvent>> DownEvents = [];
    public readonly Dictionary<AgentItem, List<DespawnEvent>> DespawnEvents = [];
    public readonly Dictionary<AgentItem, List<SpawnEvent>> SpawnEvents = [];

    public readonly Dictionary<AgentItem, List<EnterCombatEvent>> EnterCombatEvents = [];
    public readonly Dictionary<AgentItem, List<ExitCombatEvent>> ExitCombatEvents = [];

    public readonly Dictionary<AgentItem, List<HealthUpdateEvent>> HealthUpdateEvents = [];
    public readonly Dictionary<AgentItem, List<BarrierUpdateEvent>> BarrierUpdateEvents = [];
    public readonly Dictionary<AgentItem, List<MaxHealthUpdateEvent>> MaxHealthUpdateEvents = [];

    public readonly Dictionary<AgentItem, List<TeamChangeEvent>> TeamChangeEvents = [];

    public readonly Dictionary<AgentItem, List<BreakbarStateEvent>> BreakbarStateEvents = [];
    public readonly Dictionary<AgentItem, List<BreakbarPercentEvent>> BreakbarPercentEvents = [];

    public readonly Dictionary<AgentItem, List<MovementEvent>> MovementEvents = [];

    public readonly List<EffectEvent> EffectEvents = [];
    public readonly Dictionary<AgentItem, List<EffectEvent>> EffectEventsBySrc = [];
    public readonly Dictionary<AgentItem, List<EffectEvent>> EffectEventsByDst = [];
    public readonly Dictionary<long, List<EffectEvent>> EffectEventsByEffectID = [];
    public readonly Dictionary<long, List<EffectEvent>> EffectEventsByTrackingID = [];
    public readonly Dictionary<long, List<EffectEventAgentCreate>> AgentEffectEventsByTrackingID = [];
    public readonly Dictionary<long, List<EffectEventGroundCreate>> GroundEffectEventsByTrackingID = [];

    public readonly List<MarkerEvent> MarkerEvents = [];
    public readonly Dictionary<AgentItem, List<MarkerEvent>> MarkerEventsBySrc = [];
    public readonly Dictionary<long, List<MarkerEvent>> MarkerEventsByID = [];

    public readonly Dictionary<SquadMarkerIndex, List<SquadMarkerEvent>> SquadMarkerEventsByIndex = [];

    public readonly Dictionary<AgentItem, List<Last90BeforeDownEvent>> Last90BeforeDownEventsBySrc = [];
    public readonly List<Last90BeforeDownEvent> Last90BeforeDownEvents = [];

    public readonly Dictionary<AgentItem, List<GliderEvent>> GliderEventsBySrc = [];

    public readonly Dictionary<AgentItem, List<StunBreakEvent>> StunBreakEventsBySrc = [];


    public readonly List<MissileEvent> MissileEvents = [];
    public readonly Dictionary<AgentItem, List<MissileEvent>> MissileEventsBySrc = [];
    public readonly Dictionary<AgentItem, List<MissileLaunchEvent>> MissileLaunchEventsByDst = [];
    public readonly Dictionary<AgentItem, List<MissileEvent>> MissileDamagingEventsBySrc = [];
    public readonly Dictionary<long, List<MissileEvent>> MissileEventsBySkillID = [];
    public readonly Dictionary<long, List<MissileEvent>> MissileEventsByTrackingID = [];

}
