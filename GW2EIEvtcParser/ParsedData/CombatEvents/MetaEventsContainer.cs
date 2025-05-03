namespace GW2EIEvtcParser.ParsedData;

internal class MetaEventsContainer
{
    public GW2BuildEvent? GW2BuildEvent { get; set; }
    public InstanceStartEvent? InstanceStartEvent { get; set; }
    public LanguageEvent? LanguageEvent { get; set; }
    public SquadCombatEndEvent? LogEndEvent { get; set; }
    public SquadCombatStartEvent? LogStartEvent { get; set; }
    public List<SquadCombatEndEvent> SquadCombatEndEvents { get; } = [];
    public List<SquadCombatStartEvent> SquadCombatStartEvents { get; } = [];
    public readonly List<LogNPCUpdateEvent> LogNPCUpdateEvents = [];
    public readonly List<MapIDEvent> MapIDEvents = [];
    public readonly List<ShardEvent> ShardEvents = [];
    public readonly List<TickRateEvent> TickRateEvents = [];
    public PointOfViewEvent? PointOfViewEvent { get; set; }
    public FractalScaleEvent? FractalScaleEvent { get; set; }
    public EvtcVersionEvent? EvtcVersionEvent { get; set; }
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
}
