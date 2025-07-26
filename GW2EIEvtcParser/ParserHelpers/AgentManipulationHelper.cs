using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser;

public static class AgentManipulationHelper
{

    internal delegate bool ExtraRedirection(CombatItem evt, AgentItem from, AgentItem to);
    internal delegate void StateEventProcessing(CombatItem evt, AgentItem from, AgentItem to);
    /// <summary>
    /// Method used to redirect a subset of events from redirectFrom to to
    /// </summary>
    /// <param name="combatData"></param>
    /// <param name="extensions"></param>
    /// <param name="agentData"></param>
    /// <param name="redirectFrom">AgentItem the events need to be redirected from</param>
    /// <param name="stateCopyFroms">AgentItems from where last known states (hp, position, etc) will be copied from</param>
    /// <param name="to">AgentItem the events need to be redirected to</param>
    /// <param name="copyPositionalDataFromAttackTarget">If true, "to" will get the positional data from attack targets, if possible</param>
    /// <param name="extraRedirections">function to handle special conditions, given event either src or dst matches from</param>
    internal static void RedirectNPCEventsAndCopyPreviousStates(List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions, AgentData agentData, AgentItem redirectFrom, List<AgentItem> stateCopyFroms, AgentItem to, bool copyPositionalDataFromAttackTarget, ExtraRedirection? extraRedirections = null, StateEventProcessing? stateEventProcessing = null)
    {
        if (!(redirectFrom.IsNPC && to.IsNPC))
        {
            throw new InvalidOperationException("Expected NPCs in RedirectNPCEventsAndCopyPreviousStates");
        }
        // Redirect combat events
        foreach (CombatItem evt in combatData)
        {
            if (to.InAwareTimes(evt.Time))
            {
                var srcMatchesAgent = evt.SrcMatchesAgent(redirectFrom, extensions);
                var dstMatchesAgent = evt.DstMatchesAgent(redirectFrom, extensions);
                if (extraRedirections != null && !extraRedirections(evt, redirectFrom, to))
                {
                    continue;
                }
                if (srcMatchesAgent)
                {
                    evt.OverrideSrcAgent(to);
                }
                if (dstMatchesAgent)
                {
                    evt.OverrideDstAgent(to);
                }
            }
        }
        // Copy attack targets
        var attackTargetAgents = new HashSet<AgentItem>();
        var attackTargetsToCopy = combatData.Where(x => x.IsStateChange == StateChange.AttackTarget && x.DstMatchesAgent(redirectFrom)).ToList() ;
        var targetableOns = combatData.Where(x => x.IsStateChange == StateChange.Targetable && x.DstAgent == 1);
        // Events copied
        var copied = new List<CombatItem>(attackTargetsToCopy.Count + 10);
        foreach (CombatItem c in attackTargetsToCopy)
        {
            var cExtra = new CombatItem(c);
            cExtra.OverrideTime(to.FirstAware - 1); // To make sure they are put before all actual agent events
            cExtra.OverrideDstAgent(to);
            combatData.Add(cExtra);
            copied.Add(cExtra);
            AgentItem at = agentData.GetAgent(c.SrcAgent, c.Time);
            if (targetableOns.Any(x => x.SrcMatchesAgent(at)))
            {
                attackTargetAgents.Add(at);
            }
        }
        // Copy states
        var stateEventsToCopy = new List<CombatItem>();
        Func<CombatItem, bool> canCopyFromAgent = (evt) => stateCopyFroms.Any(x => evt.SrcMatchesAgent(x));
        var stateChangeCopyFromAgentConditions = new List<Func<CombatItem, bool>>()
        {
            (x) => x.IsStateChange == StateChange.BreakbarState,
            (x) => x.IsStateChange == StateChange.MaxHealthUpdate,
            (x) => x.IsStateChange == StateChange.HealthUpdate,
            (x) => x.IsStateChange == StateChange.BreakbarPercent,
            (x) => x.IsStateChange == StateChange.BarrierUpdate,
            (x) => (x.IsStateChange == StateChange.EnterCombat || x.IsStateChange == StateChange.ExitCombat),
            (x) => (x.IsStateChange == StateChange.Spawn || x.IsStateChange == StateChange.Despawn || x.IsStateChange == StateChange.ChangeDead || x.IsStateChange == StateChange.ChangeDown || x.IsStateChange == StateChange.ChangeUp),
        };
        if (!copyPositionalDataFromAttackTarget || attackTargetAgents.Count == 0)
        {
            stateChangeCopyFromAgentConditions.Add((x) => x.IsStateChange == StateChange.Position);
            stateChangeCopyFromAgentConditions.Add((x) => x.IsStateChange == StateChange.Rotation);
            stateChangeCopyFromAgentConditions.Add((x) => x.IsStateChange == StateChange.Velocity);
        }
        foreach (Func<CombatItem, bool> stateChangeCopyCondition in stateChangeCopyFromAgentConditions)
        {
            CombatItem? stateToCopy = combatData.LastOrDefault(x => stateChangeCopyCondition(x) && canCopyFromAgent(x) && x.Time <= to.FirstAware);
            if (stateToCopy != null)
            {
                stateEventsToCopy.Add(stateToCopy);
            }
        }
        // Copy positional data from attack targets
        if (copyPositionalDataFromAttackTarget && attackTargetAgents.Count != 0)
        {
            Func<CombatItem, bool> canCopyFromAttackTarget = (evt) => attackTargetAgents.Any(x => evt.SrcMatchesAgent(x));
            var stateChangeCopyFromAttackTargetConditions = new List<Func<CombatItem, bool>>()
            {
                (x) => x.IsStateChange == StateChange.Position,
                (x) => x.IsStateChange == StateChange.Rotation,
                (x) => x.IsStateChange == StateChange.Velocity,
            };
            foreach (Func<CombatItem, bool> stateChangeCopyCondition in stateChangeCopyFromAttackTargetConditions)
            {
                CombatItem? stateToCopy = combatData.LastOrDefault(x => stateChangeCopyCondition(x) && canCopyFromAttackTarget(x) && x.Time <= to.FirstAware);
                if (stateToCopy != null)
                {
                    stateEventsToCopy.Add(stateToCopy);
                }
            }
        }
        if (stateEventsToCopy.Count > 0)
        {
            foreach (CombatItem c in stateEventsToCopy)
            {
                var cExtra = new CombatItem(c);
                cExtra.OverrideTime(to.FirstAware-1); // To make sure they are put before all actual agent events
                cExtra.OverrideSrcAgent(to);
                combatData.Add(cExtra);
                copied.Add(cExtra);
            }
        }
        if (copied.Count > 0)
        {
            combatData.SortByTime();
            foreach (CombatItem c in copied)
            {
                c.OverrideTime(to.FirstAware);
                if (stateEventProcessing != null)
                {
                    combatData.SortByTime();
                    stateEventProcessing(c, redirectFrom, to);
                }
            }
            if (stateEventProcessing != null)
            {
                combatData.SortByTime();
            }
        }
        // Redirect NPC and Gadget masters
        IReadOnlyList<AgentItem> masterRedirectionCandidates = [
             .. agentData.GetAgentByType(AgentItem.AgentType.NPC),
             .. agentData.GetAgentByType(AgentItem.AgentType.Gadget)
            ];
        foreach (AgentItem ag in masterRedirectionCandidates)
        {
            if (ag.Master == redirectFrom && to.InAwareTimes(ag.FirstAware))
            {
                ag.SetMaster(to);
            }
        }

        to.AddMergeFrom(redirectFrom, to.FirstAware, to.LastAware);
    }

    internal static void RedirectPlayerEventsAndCopyPreviousStates(List<CombatItem> combatData, IReadOnlyList<CombatItem> combatDataFrom, IReadOnlyDictionary<uint, ExtensionHandler> extensions, AgentData agentData, AgentItem redirectFrom, AgentItem to)
    {
        if (!(redirectFrom.IsPlayer && to.IsPlayer))
        {
            throw new InvalidOperationException("Expected Players in RedirectPlayerEventsAndCopyPreviousStates");
        }
        // Redirect combat events
        var buffOnFromEvents = new List<CombatItem>();
        var copied = new List<CombatItem>();
        foreach (CombatItem evt in combatDataFrom)
        {
            // Special handling for buffs
            if (evt.IsBuffApply() && evt.DstMatchesAgent(redirectFrom, extensions))
            {
                buffOnFromEvents.Add(evt);
                continue;
            }
            if (evt.IsBuffRemoval() && evt.SrcMatchesAgent(redirectFrom, extensions))
            {
                buffOnFromEvents.Add(evt);
                continue;
            }
            // The rest
            if (to.InAwareTimes(evt.Time))
            {
                var srcMatchesAgent = evt.SrcMatchesAgent(redirectFrom, extensions);
                var dstMatchesAgent = evt.DstMatchesAgent(redirectFrom, extensions);
                if (srcMatchesAgent)
                {
                    evt.OverrideSrcAgent(to);
                }
                if (dstMatchesAgent)
                {
                    evt.OverrideDstAgent(to);
                }
            }
        }
        // Copy states
        var stateEventsToCopy = new List<CombatItem>();
        Func<CombatItem, bool> canCopyFromAgent = (evt) => evt.SrcMatchesAgent(redirectFrom);
        var stateChangeCopyFromAgentConditions = new List<Func<CombatItem, bool>>()
        {
            (x) => x.IsStateChange == StateChange.BreakbarState,
            (x) => x.IsStateChange == StateChange.MaxHealthUpdate,
            (x) => x.IsStateChange == StateChange.HealthUpdate,
            (x) => x.IsStateChange == StateChange.BreakbarPercent,
            (x) => x.IsStateChange == StateChange.BarrierUpdate,
            (x) => (x.IsStateChange == StateChange.EnterCombat || x.IsStateChange == StateChange.ExitCombat),
            (x) => x.IsStateChange == StateChange.Position,
            (x) => x.IsStateChange == StateChange.Rotation,
            (x) => x.IsStateChange == StateChange.Velocity,
        };
        foreach (Func<CombatItem, bool> stateChangeCopyCondition in stateChangeCopyFromAgentConditions)
        {
            CombatItem? stateToCopy = combatDataFrom.LastOrDefault(x => stateChangeCopyCondition(x) && canCopyFromAgent(x) && x.Time <= to.FirstAware);
            if (stateToCopy != null)
            {
                stateEventsToCopy.Add(stateToCopy);
            }
        }
        if (stateEventsToCopy.Count > 0)
        {
            foreach (CombatItem c in stateEventsToCopy)
            {
                var cExtra = new CombatItem(c);
                cExtra.OverrideTime(to.FirstAware - 1); // To make sure they are put before all actual agent events
                cExtra.OverrideSrcAgent(to);
                combatData.Add(cExtra);
                copied.Add(cExtra);
            }
        }
        if (copied.Count > 0)
        {
            combatData.SortByTime();
            foreach (CombatItem c in copied)
            {
                c.OverrideTime(to.FirstAware);
            }
        }
        // TODO overlapping minions
        // Redirect NPC and Gadget masters
        IReadOnlyList<AgentItem> masterRedirectionCandidates = [
             .. agentData.GetAgentByType(AgentItem.AgentType.NPC),
             .. agentData.GetAgentByType(AgentItem.AgentType.Gadget)
            ];
        foreach (AgentItem ag in masterRedirectionCandidates)
        {
            if (ag.Master == redirectFrom && to.InAwareTimes(ag.FirstAware))
            {
                ag.SetMaster(to);
            }
        }
        to.AddParentFrom(redirectFrom);
    }

    internal static void SplitPlayerPerSpecAndSubgroup(List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions, AgentData agentData, AgentItem originalPlayer)
    {
        var previousSpec = Spec.Unknown;
        var previousGroup = -1;
        var previousPlayerAgent = originalPlayer;
        var combatItemsToUse = combatData.Where(x => x.SrcMatchesAgent(previousPlayerAgent) || x.DstMatchesAgent(previousPlayerAgent)).ToList();
        var enterCombatEvents = combatData.Where(x => x.IsStateChange == StateChange.EnterCombat && x.SrcMatchesAgent(previousPlayerAgent)).Select(x => new EnterCombatEvent(x, agentData)).Where(x => x.Spec != Spec.Unknown).ToList();
        var newPlayers = new List<AgentItem>(enterCombatEvents.Count);
        for (var i = 0; i < enterCombatEvents.Count; i++)
        {
            var enterCombat = enterCombatEvents[i];
            // We can leave the first enter combat alone
            if (i == 0)
            {
                previousSpec = enterCombat.Spec;
                previousGroup = enterCombat.Subgroup;
            } 
            else
            {
                if (enterCombat.Spec != previousSpec || enterCombat.Subgroup != previousGroup)
                {
                    previousSpec = enterCombat.Spec;
                    previousGroup = enterCombat.Subgroup;
                    // Redirect everything during aware times with the exception of:
                    // - copy previous state changes in a similar manner to RedirectEventsAndCopyPreviousStates
                    // - copy all buff events whose stacks are active during aware time
                    // if a minion is alive during aware time but also before FirstAware, they need to be split using the same rule
                    long start = enterCombat.Time;
                    long end = previousPlayerAgent.LastAware;

                    var newPlayerAgent = agentData.AddCustomAgentFrom(previousPlayerAgent, start, end, enterCombat.Spec);
                    newPlayers.Add(newPlayerAgent);

                    RedirectPlayerEventsAndCopyPreviousStates(combatData, combatItemsToUse, extensions, agentData, previousPlayerAgent, newPlayerAgent);

                    previousPlayerAgent.OverrideAwareTimes(previousPlayerAgent.FirstAware, start);
                    previousPlayerAgent = newPlayerAgent;
                    combatItemsToUse = combatData.Where(x => x.SrcMatchesAgent(previousPlayerAgent) || x.DstMatchesAgent(previousPlayerAgent)).ToList();
                }
            }
        }
    }

    internal static void RegroupSameAgentsAndDetermineTeams(AgentData agentData, IReadOnlyList<CombatItem> combatItems, EvtcVersionEvent evtcVersion, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var toRemove = new List<AgentItem>(100);
        var toAdd = new List<AgentItem>(30);
        var combatDataDict = combatItems.Where(x => x.SrcIsAgent(extensions) || x.DstIsAgent(extensions));
        var srcCombatDataDict = combatDataDict.Where(x => x.SrcIsAgent(extensions)).GroupBy(x => agentData.GetAgent(x.SrcAgent, x.Time)).ToDictionary(x => x.Key, x => x.ToList());
        var dstCombatDataDict = combatDataDict.Where(x => x.DstIsAgent(extensions)).GroupBy(x => agentData.GetAgent(x.DstAgent, x.Time)).ToDictionary(x => x.Key, x => x.ToList());
        // NPCS
        {
            var npcsBySpeciesIDs = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => !x.IsNonIdentifiedSpecies()).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var npcsBySpeciesID in npcsBySpeciesIDs)
            {
                var agentsByInstid = npcsBySpeciesID.Value.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var pair in agentsByInstid)
                {
                    var agents = pair.Value;
                    if (agents.Count > 1)
                    {
                        AgentItem firstItem = agents.First();
                        var newTargetAgent = new AgentItem(firstItem);
                        newTargetAgent.OverrideAwareTimes(agents.Min(x => x.FirstAware), agents.Max(x => x.LastAware));
                        foreach (AgentItem agentItem in agents)
                        {
                            if (srcCombatDataDict.TryGetValue(agentItem, out var srcCombatItems))
                            {
                                srcCombatItems.ForEach(x => x.OverrideSrcAgent(newTargetAgent));
                            }
                            if (dstCombatDataDict.TryGetValue(agentItem, out var dstCombatItems))
                            {
                                dstCombatItems.ForEach(x => x.OverrideDstAgent(newTargetAgent));
                            }
                            agentData.SwapMasters(agentItem, newTargetAgent);
                        }
                        toRemove.AddRange(agents);
                        toAdd.Add(newTargetAgent);
                    }
                }
            }
        }
        // Non squad Players
        {
            IReadOnlyList<AgentItem> nonSquadPlayerAgents = agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer);
            if (nonSquadPlayerAgents.Any())
            {
                var teamChangeDict = combatItems.Where(x => x.IsStateChange == StateChange.TeamChange).GroupBy(x => x.SrcAgent).ToDictionary(x => x.Key, x => x.ToList());

                IReadOnlyList<AgentItem> squadPlayers = agentData.GetAgentByType(AgentItem.AgentType.Player);
                ulong greenTeam = ulong.MaxValue;
                var greenTeams = new List<ulong>();
                foreach (AgentItem a in squadPlayers)
                {
                    if (teamChangeDict.TryGetValue(a.Agent, out var teamChangeList))
                    {
                        greenTeams.AddRange(teamChangeList.Where(x => x.SrcMatchesAgent(a)).Select(TeamChangeEvent.GetTeamIDInto));
                        if (evtcVersion.Build > ArcDPSBuilds.TeamChangeOnDespawn)
                        {
                            greenTeams.AddRange(teamChangeList.Where(x => x.SrcMatchesAgent(a)).Select(TeamChangeEvent.GetTeamIDComingFrom));
                        }
                    }
                }
                greenTeams.RemoveAll(x => x == 0);
                if (greenTeams.Count != 0)
                {
                    greenTeam = greenTeams.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).First();
                }
                var uniqueNonSquadPlayers = new List<AgentItem>();
                foreach (AgentItem nonSquadPlayer in nonSquadPlayerAgents)
                {
                    if (teamChangeDict.TryGetValue(nonSquadPlayer.Agent, out var teamChangeList))
                    {
                        var team = teamChangeList.Where(x => x.SrcMatchesAgent(nonSquadPlayer)).Select(TeamChangeEvent.GetTeamIDInto).ToList();
                        if (evtcVersion.Build > ArcDPSBuilds.TeamChangeOnDespawn)
                        {
                            team.AddRange(teamChangeList.Where(x => x.SrcMatchesAgent(nonSquadPlayer)).Select(TeamChangeEvent.GetTeamIDComingFrom));
                        }
                        team.RemoveAll(x => x == 0);
                        nonSquadPlayer.OverrideIsNotInSquadFriendlyPlayer(team.Any(x => x == greenTeam));
                    }
                }
                var nonSquadPlayersByInstids = nonSquadPlayerAgents.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var nonSquadPlayersByInstid in nonSquadPlayersByInstids)
                {
                    var agents = nonSquadPlayersByInstid.Value;
                    if (agents.Count > 1)
                    {
                        AgentItem firstItem = agents.First();
                        var newPlayerAgent = new AgentItem(firstItem);
                        newPlayerAgent.OverrideAwareTimes(agents.Min(x => x.FirstAware), agents.Max(x => x.LastAware));
                        foreach (AgentItem agentItem in agents)
                        {
                            if (srcCombatDataDict.TryGetValue(agentItem, out var srcCombatItems))
                            {
                                srcCombatItems.ForEach(x => x.OverrideSrcAgent(newPlayerAgent));
                            }
                            if (dstCombatDataDict.TryGetValue(agentItem, out var dstCombatItems))
                            {
                                dstCombatItems.ForEach(x => x.OverrideDstAgent(newPlayerAgent));
                            }
                            agentData.SwapMasters(agentItem, newPlayerAgent);
                        }
                        toRemove.AddRange(agents);
                        toAdd.Add(newPlayerAgent);
                    }
                }
            }
        }
        // Players
        {
            IReadOnlyList<AgentItem> playerAgents = agentData.GetAgentByType(AgentItem.AgentType.Player);
            var playerAgentsByNames = playerAgents.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var playerAgentsByName in playerAgentsByNames)
            {
                var agents = playerAgentsByName.Value;
                if (agents.Count > 1)
                {
                    AgentItem firstItem = agents.First();
                    var newPlayerAgent = new AgentItem(firstItem);
                    newPlayerAgent.OverrideAwareTimes(agents.Min(x => x.FirstAware), agents.Max(x => x.LastAware));
                    foreach (AgentItem agentItem in agents)
                    {
                        if (srcCombatDataDict.TryGetValue(agentItem, out var srcCombatItems))
                        {
                            srcCombatItems.ForEach(x => x.OverrideSrcAgent(newPlayerAgent));
                        }
                        if (dstCombatDataDict.TryGetValue(agentItem, out var dstCombatItems))
                        {
                            dstCombatItems.ForEach(x => x.OverrideDstAgent(newPlayerAgent));
                        }
                        agentData.SwapMasters(agentItem, newPlayerAgent);
                    }
                    toRemove.AddRange(agents);
                    toAdd.Add(newPlayerAgent);
                }
            }
        }
        agentData.ReplaceAgents(toRemove, toAdd);
    }

    /// <summary>
    /// Method used to redirect all events from redirectFrom to to
    /// </summary>
    /// <param name="combatData"></param>
    /// <param name="extensions"></param>
    /// <param name="agentData"></param>
    /// <param name="redirectFrom">AgentItem the events need to be redirected from</param>
    /// <param name="to">AgentItem the events need to be redirected to</param>
    /// <param name="extraRedirections">function to handle special conditions, given event either src or dst matches from</param>
    internal static void RedirectAllEvents(IReadOnlyList<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions, AgentData agentData, AgentItem redirectFrom, AgentItem to, ExtraRedirection? extraRedirections = null)
    {
        // Redirect combat events
        foreach (CombatItem evt in combatData)
        {
            var srcMatchesAgent = evt.SrcMatchesAgent(redirectFrom, extensions);
            var dstMatchesAgent = evt.DstMatchesAgent(redirectFrom, extensions);
            if (!dstMatchesAgent && !srcMatchesAgent)
            {
                continue;
            }
            if (extraRedirections != null && !extraRedirections(evt, redirectFrom, to))
            {
                continue;
            }
            if (srcMatchesAgent)
            {
                evt.OverrideSrcAgent(to);
            }
            if (dstMatchesAgent)
            {
                evt.OverrideDstAgent(to);
            }
        }
        agentData.SwapMasters(redirectFrom, to);
        to.AddMergeFrom(redirectFrom, redirectFrom.FirstAware, redirectFrom.LastAware);
    }

}
