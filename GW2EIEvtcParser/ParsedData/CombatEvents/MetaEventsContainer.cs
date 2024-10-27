using System.Collections.Generic;

namespace GW2EIEvtcParser.ParsedData;

internal class MetaEventsContainer
{
    public GW2BuildEvent? GW2BuildEvent { get; set; }
    public InstanceStartEvent? InstanceStartEvent { get; set; }
    public LanguageEvent? LanguageEvent { get; set; }
    public LogEndEvent? LogEndEvent { get; set; }
    public LogStartEvent? LogStartEvent { get; set; }
    public readonly List<LogNPCUpdateEvent> LogNPCUpdateEvents = new();
    public readonly List<MapIDEvent> MapIDEvents = new();
    public readonly List<ShardEvent> ShardEvents = new();
    public readonly List<TickRateEvent> TickRateEvents = new();
    public PointOfViewEvent? PointOfViewEvent { get; set; }
    public FractalScaleEvent? FractalScaleEvent { get; set; }
    public EvtcVersionEvent? EvtcVersionEvent { get; set; }
    public readonly Dictionary<AgentItem, List<GuildEvent>> GuildEvents = new();
    public readonly Dictionary<long, BuffInfoEvent> BuffInfoEvents = new();
    public readonly Dictionary<byte, List<BuffInfoEvent>> BuffInfoEventsByCategory = new();
    public readonly Dictionary<long, SkillInfoEvent> SkillInfoEvents = new();
    public readonly List<ErrorEvent> ErrorEvents = new();
    public readonly Dictionary<long, EffectGUIDEvent> EffectGUIDEventsByEffectID = new();
    public readonly Dictionary<string, EffectGUIDEvent> EffectGUIDEventsByGUID = new();
    public readonly Dictionary<long, MarkerGUIDEvent> MarkerGUIDEventsByMarkerID = new();
    public readonly Dictionary<string, MarkerGUIDEvent> MarkerGUIDEventsByGUID = new();
}
