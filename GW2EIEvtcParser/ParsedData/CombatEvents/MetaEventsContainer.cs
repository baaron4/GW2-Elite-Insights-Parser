namespace GW2EIEvtcParser.ParsedData;

internal class MetaEventsContainer
{
    public GW2BuildEvent? GW2BuildEvent { get; set; }
    public InstanceStartEvent? InstanceStartEvent { get; set; }
    public LanguageEvent? LanguageEvent { get; set; }
    public LogEndEvent? LogEndEvent { get; set; }
    public LogStartEvent? LogStartEvent { get; set; }
    public List<LogEndEvent> LogEndEvents { get; } = [];
    public List<LogStartEvent> LogStartEvents { get; } = [];
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
}
