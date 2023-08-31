using System.Collections.Generic;

namespace GW2EIEvtcParser.ParsedData
{
    internal class MetaEventsContainer
    {
        public BuildEvent BuildEvent { get; set; }
        public InstanceStartEvent InstanceStartEvent { get; set; }
        public LanguageEvent LanguageEvent { get; set; }
        public LogEndEvent LogEndEvent { get; set; }
        public LogStartEvent LogStartEvent { get; set; }
        public List<LogStartNPCUpdateEvent> LogStartNPCUpdateEvents { get; } = new List<LogStartNPCUpdateEvent>();
        public List<MapIDEvent> MapIDEvents { get; } = new List<MapIDEvent>();
        public List<ShardEvent> ShardEvents { get; } = new List<ShardEvent>();
        public List<TickRateEvent> TickRateEvents { get; } = new List<TickRateEvent>();
        public PointOfViewEvent PointOfViewEvent { get; set; }
        public FractalScaleEvent FractalScaleEvent { get; set; }
        public Dictionary<AgentItem, List<GuildEvent>> GuildEvents { get; } = new Dictionary<AgentItem, List<GuildEvent>>();
        public Dictionary<long, BuffInfoEvent> BuffInfoEvents { get; } = new Dictionary<long, BuffInfoEvent>();
        public Dictionary<byte, List<BuffInfoEvent>> BuffInfoEventsByCategory { get; } = new Dictionary<byte, List<BuffInfoEvent>>();
        public Dictionary<long, SkillInfoEvent> SkillInfoEvents { get; } = new Dictionary<long, SkillInfoEvent>();
        public List<ErrorEvent> ErrorEvents { get; } = new List<ErrorEvent>();
        public Dictionary<long, EffectGUIDEvent> EffectGUIDEventsByEffectID { get; } = new Dictionary<long, EffectGUIDEvent>();
        public Dictionary<string, EffectGUIDEvent> EffectGUIDEventsByGUID { get; } = new Dictionary<string, EffectGUIDEvent>();
        public Dictionary<long, MarkerGUIDEvent> MarkerGUIDEventsByMarkerID { get; } = new Dictionary<long, MarkerGUIDEvent>();
        public Dictionary<string, MarkerGUIDEvent> MarkerGUIDEventsByGUID { get; } = new Dictionary<string, MarkerGUIDEvent>();
    }
}
