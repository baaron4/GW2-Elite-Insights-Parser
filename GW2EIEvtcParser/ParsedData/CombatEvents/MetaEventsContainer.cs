namespace GW2EIEvtcParser.ParsedData;

internal class MetaEventsContainer
{
    public GW2BuildEvent? GW2BuildEvent;
    public InstanceStartEvent? InstanceStartEvent;
    public LanguageEvent? LanguageEvent;
    public SquadCombatEndEvent? LogEndEvent;
    public SquadCombatStartEvent? LogStartEvent;
    public List<SquadCombatEndEvent> SquadCombatEndEvents { get; } = [];
    public List<SquadCombatStartEvent> SquadCombatStartEvents { get; } = [];
    public readonly List<LogNPCUpdateEvent> LogNPCUpdateEvents = [];
    public MapIDEvent? MapIDEvent;
    public readonly List<MapChangeEvent> MapChangeEvents = [];
    public ShardEvent? ShardEvent;
    public readonly List<TickRateEvent> TickRateEvents = [];
    public PointOfViewEvent? PointOfViewEvent;
    public FractalScaleEvent? FractalScaleEvent;
    public EvtcVersionEvent? EvtcVersionEvent;
    public WvWTeamsEvent? WvWTeamsEvent;

    public readonly Dictionary<AgentItem, List<GuildEvent>> GuildEvents = [];

    public readonly Dictionary<long, BuffInfoEvent> BuffInfoEvents = [];
    public readonly Dictionary<byte, List<BuffInfoEvent>> BuffInfoEventsByCategory = [];

    public readonly Dictionary<long, SkillInfoEvent> SkillInfoEvents = [];

    public readonly List<ErrorEvent> ErrorEvents = [];

    public readonly Dictionary<long, EffectGUIDEvent> EffectGUIDEventsByEffectID = [];
    public readonly Dictionary<GUID, EffectGUIDEvent> EffectGUIDEventsByGUID = [];

    public readonly Dictionary<long, MarkerGUIDEvent> MarkerGUIDEventsByMarkerID = [];
    public readonly Dictionary<GUID, MarkerGUIDEvent> MarkerGUIDEventsByGUID = [];

    public readonly Dictionary<long, SpeciesGUIDEvent> SpeciesGUIDEventsBySpeciesID = [];
    public readonly Dictionary<GUID, SpeciesGUIDEvent> SpeciesGUIDEventsByGUID = [];

    public readonly Dictionary<long, SkillGUIDEvent> SkillGUIDEventsBySkillID = [];
    public readonly Dictionary<GUID, SkillGUIDEvent> SkillGUIDEventsByGUID = [];

    public readonly Dictionary<long, EmoteGUIDEvent> EmoteGUIDEventsByEmoteID = [];
    public readonly Dictionary<GUID, EmoteGUIDEvent> EmoteGUIDEventsByGUID = [];

    public readonly Dictionary<ulong, TeamGUIDEvent> TeamGUIDEventsByTeamID = [];
    public readonly Dictionary<GUID, TeamGUIDEvent> TeamGUIDEventsByGUID = [];

    public readonly Dictionary<long, TransformationGUIDEvent> TransformationGUIDEventsByTransformationID = [];
    public readonly Dictionary<GUID, TransformationGUIDEvent> TransformationGUIDEventsByGUID = [];

    public readonly List<AttackTargetEvent> AttackTargetEvents = [];
    public readonly Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEventsBySrc = [];
    public readonly Dictionary<AgentItem, AttackTargetEvent> AttackTargetEventByAttackTarget = [];
}
