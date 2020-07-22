using System.Collections.Generic;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class StatusEventsContainer
    {
        public Dictionary<AgentItem, List<AliveEvent>> AliveEvents { get; } = new Dictionary<AgentItem, List<AliveEvent>>();
        public Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEvents { get; } = new Dictionary<AgentItem, List<AttackTargetEvent>>();
        public Dictionary<AgentItem, List<DeadEvent>> DeadEvents { get; } = new Dictionary<AgentItem, List<DeadEvent>>();
        public Dictionary<AgentItem, List<DespawnEvent>> DespawnEvents { get; } = new Dictionary<AgentItem, List<DespawnEvent>>();
        public Dictionary<AgentItem, List<DownEvent>> DownEvents { get; } = new Dictionary<AgentItem, List<DownEvent>>();
        public Dictionary<AgentItem, List<EnterCombatEvent>> EnterCombatEvents { get; } = new Dictionary<AgentItem, List<EnterCombatEvent>>();
        public Dictionary<AgentItem, List<ExitCombatEvent>> ExitCombatEvents { get; } = new Dictionary<AgentItem, List<ExitCombatEvent>>();
        public Dictionary<AgentItem, List<HealthUpdateEvent>> HealthUpdateEvents { get; } = new Dictionary<AgentItem, List<HealthUpdateEvent>>();
        public Dictionary<AgentItem, List<MaxHealthUpdateEvent>> MaxHealthUpdateEvents { get; } = new Dictionary<AgentItem, List<MaxHealthUpdateEvent>>();
        public Dictionary<AgentItem, List<SpawnEvent>> SpawnEvents { get; } = new Dictionary<AgentItem, List<SpawnEvent>>();
        public Dictionary<AgentItem, List<TargetableEvent>> TargetableEvents { get; } = new Dictionary<AgentItem, List<TargetableEvent>>();
        public Dictionary<AgentItem, List<TeamChangeEvent>> TeamChangeEvents { get; } = new Dictionary<AgentItem, List<TeamChangeEvent>>();
        public Dictionary<AgentItem, List<BreakbarStateEvent>> BreakbarStateEvents { get; } = new Dictionary<AgentItem, List<BreakbarStateEvent>>();
        public Dictionary<AgentItem, List<BreakbarPercentEvent>> BreakbarPercentEvents { get; } = new Dictionary<AgentItem, List<BreakbarPercentEvent>>();
        //public Dictionary<AgentItem, List<TagEvent>> TagEvents { get; } = new Dictionary<AgentItem, List<TagEvent>>();

    }
}
