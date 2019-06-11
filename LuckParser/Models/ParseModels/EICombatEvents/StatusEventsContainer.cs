using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LuckParser.Models.ParseModels
{
    public class StatusEventsContainer
    {
        public readonly Dictionary<AgentItem, List<AliveEvent>> AliveEvents = new Dictionary<AgentItem, List<AliveEvent>>();
        public readonly Dictionary<AgentItem, List<AttackTargetEvent>> AttackTargetEvents = new Dictionary<AgentItem, List<AttackTargetEvent>>();
        public readonly Dictionary<AgentItem, List<DeadEvent>> DeadEvents = new Dictionary<AgentItem, List<DeadEvent>>();
        public readonly Dictionary<AgentItem, List<DespawnEvent>> DespawnEvents = new Dictionary<AgentItem, List<DespawnEvent>>();
        public readonly Dictionary<AgentItem, List<DownEvent>> DownEvents = new Dictionary<AgentItem, List<DownEvent>>();
        public readonly Dictionary<AgentItem, List<EnterCombatEvent>> EnterCombatEvents = new Dictionary<AgentItem, List<EnterCombatEvent>>();
        public readonly Dictionary<AgentItem, List<ExitCombatEvent>> ExitCombatEvents = new Dictionary<AgentItem, List<ExitCombatEvent>>();
        public readonly Dictionary<AgentItem, List<GuildEvent>> GuildEvents = new Dictionary<AgentItem, List<GuildEvent>>();
        public readonly Dictionary<AgentItem, List<HealthUpdateEvent>> HealthUpdateEvents = new Dictionary<AgentItem, List<HealthUpdateEvent>>();
        public readonly Dictionary<AgentItem, List<MaxHealthUpdateEvent>> MaxHealthUpdateEvents = new Dictionary<AgentItem, List<MaxHealthUpdateEvent>>();
        public readonly Dictionary<AgentItem, List<SpawnEvent>> SpawnEvents = new Dictionary<AgentItem, List<SpawnEvent>>();
        public readonly Dictionary<AgentItem, List<TargetableEvent>> TargetableEvents = new Dictionary<AgentItem, List<TargetableEvent>>();
        public readonly Dictionary<AgentItem, List<TeamChangeEvent>> TeamChangeEvents = new Dictionary<AgentItem, List<TeamChangeEvent>>();

    }
}