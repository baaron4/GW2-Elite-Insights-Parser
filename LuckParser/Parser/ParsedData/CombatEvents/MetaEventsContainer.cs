using System.Collections.Generic;

namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class MetaEventsContainer
    {
        public List<BuildEvent> BuildEvents { get; } = new List<BuildEvent>();
        public List<LanguageEvent> LanguageEvents { get; } = new List<LanguageEvent>();
        public List<LogEndEvent> LogEndEvents { get; } = new List<LogEndEvent>();
        public List<LogStartEvent> LogStartEvents { get; } = new List<LogStartEvent>();
        public List<MapIDEvent> MapIDEvents { get; } = new List<MapIDEvent>();
        public List<RewardEvent> RewardEvents { get; } = new List<RewardEvent>();
        public List<ShardEvent> ShardEvents { get; } = new List<ShardEvent>();
        public List<PointOfViewEvent> PointOfViewEvents { get; } = new List<PointOfViewEvent>();
    }
}
