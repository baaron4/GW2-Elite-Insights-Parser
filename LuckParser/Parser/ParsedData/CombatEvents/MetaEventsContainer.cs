using System.Collections.Generic;

namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class MetaEventsContainer
    {
        public readonly List<BuildEvent> BuildEvents = new List<BuildEvent>();
        public readonly List<LanguageEvent> LanguageEvents = new List<LanguageEvent>();
        public readonly List<LogEndEvent> LogEndEvents = new List<LogEndEvent>();
        public readonly List<LogStartEvent> LogStartEvents = new List<LogStartEvent>();
        public readonly List<MapIDEvent> MapIDEvents = new List<MapIDEvent>();
        public readonly List<RewardEvent> RewardEvents = new List<RewardEvent>();
        public readonly List<ShardEvent> ShardEvents = new List<ShardEvent>();
        public readonly List<PointOfViewEvent> PointOfViewEvents = new List<PointOfViewEvent>();
    }
}