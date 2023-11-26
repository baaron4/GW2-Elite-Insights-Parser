using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal static class EncounterLogicUtils
    {
        internal static void RegroupTargetsByID(int id, AgentData agentData, IReadOnlyList<CombatItem> combatItems, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            IReadOnlyList<AgentItem> agents = agentData.GetNPCsByID(id);
            if (agents.Count > 1)
            {
                AgentItem firstItem = agents.First();
                var newTargetAgent = new AgentItem(firstItem);
                newTargetAgent.OverrideAwareTimes(agents.Min(x => x.FirstAware), agents.Max(x => x.LastAware));
                agentData.ReplaceAgentsFromID(newTargetAgent);
                foreach (AgentItem agentItem in agents)
                {
                    RedirectAllEvents(combatItems, extensions, agentData, agentItem, newTargetAgent);
                }
            }
        }

        internal static void NegateDamageAgainstBarrier(CombatData combatData, IReadOnlyList<AgentItem> agentItems)
        {
            var dmgEvts = new List<AbstractHealthDamageEvent>();
            foreach (AgentItem agentItem in agentItems)
            {
                dmgEvts.AddRange(combatData.GetDamageTakenData(agentItem));
            }
            foreach (AbstractHealthDamageEvent de in dmgEvts)
            {
                if (de.ShieldDamage > 0)
                {
                    de.NegateShieldDamage();
                }
            }
        }

        /*protected static void AdjustTimeRefreshBuff(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, long id)
        {
            if (buffsById.TryGetValue(id, out List<AbstractBuffEvent> buffList))
            {
                var agentsToSort = new HashSet<AgentItem>();
                foreach (AbstractBuffEvent be in buffList)
                {
                    if (be is AbstractBuffRemoveEvent abre)
                    {
                        // to make sure remove events are before applications
                        abre.OverrideTime(abre.Time - 1);
                        agentsToSort.Add(abre.To);
                    }
                }
                if (buffList.Count > 0)
                {
                    buffsById[id].Sort((x, y) => x.Time.CompareTo(y.Time));
                }
                foreach (AgentItem a in agentsToSort)
                {
                    buffsByDst[a].Sort((x, y) => x.Time.CompareTo(y.Time));
                }
            }
        }*/

        internal static List<ErrorEvent> GetConfusionDamageMissingMessage(int arcdpsVersion)
        {
            if (arcdpsVersion > ArcDPSEnums.ArcDPSBuilds.ProperConfusionDamageSimulation)
            {
                return new List<ErrorEvent>();
            }
            return new List<ErrorEvent>()
            {
                new ErrorEvent("Missing confusion damage")
            };
        }
        internal static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, long buffID, AgentItem target, bool beginWithStart, bool padEnd)
        {
            bool needStart = beginWithStart;
            var main = combatData.GetBuffData(buffID).Where(x => x.To == target && (x is BuffApplyEvent || x is BuffRemoveAllEvent)).ToList();
            var filtered = new List<AbstractBuffEvent>();
            for (int i = 0; i < main.Count; i++)
            {
                AbstractBuffEvent c = main[i];
                if (needStart && c is BuffApplyEvent)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c is BuffRemoveAllEvent)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1] is BuffApplyEvent))
                    {
                        needStart = true;
                        filtered.Add(c);
                    }
                }
            }
            if (padEnd && filtered.Any() && filtered.Last() is BuffApplyEvent)
            {
                AbstractBuffEvent last = filtered.Last();
                filtered.Add(new BuffRemoveAllEvent(_unknownAgent, last.To, target.LastAware, int.MaxValue, last.BuffSkill, ArcDPSEnums.IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
            }
            return filtered;
        }

        internal static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, long buffID, AbstractSingleActor target, bool beginWithStart, bool padEnd)
        {
            return GetFilteredList(combatData, buffID, target.AgentItem, beginWithStart, padEnd);
        }

        internal static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, IEnumerable<long> buffIDs, AgentItem target, bool beginWithStart, bool padEnd)
        {
            var filteredList = new List<AbstractBuffEvent>();
            foreach (long buffID in buffIDs)
            {
                filteredList.AddRange(GetFilteredList(combatData, buffID, target, beginWithStart, padEnd));
            }
            return filteredList;
        }

        internal static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, IEnumerable<long> buffIDs, AbstractSingleActor target, bool beginWithStart, bool padEnd)
        {
            return GetFilteredList(combatData, buffIDs, target.AgentItem, beginWithStart, padEnd);
        }

        internal static bool AtLeastOnePlayerAlive(CombatData combatData, FightData fightData, long timeToCheck, IReadOnlyCollection<AgentItem> playerAgents)
        {
            int playerDeadOrDCCount = 0;
            foreach (AgentItem playerAgent in playerAgents)
            {
                var deads = new List<Segment>();
                var downs = new List<Segment>();
                var dcs = new List<Segment>();
                playerAgent.GetAgentStatus(deads, downs, dcs, combatData, fightData);
                if (deads.Any(x => x.ContainsPoint(timeToCheck)))
                {
                    playerDeadOrDCCount++;
                }
                else if (dcs.Any(x => x.ContainsPoint(timeToCheck)))
                {
                    playerDeadOrDCCount++;
                }
            }
            if (playerDeadOrDCCount == playerAgents.Count)
            {
                return false;
            }
            return true;
        }


        internal delegate bool ChestAgentChecker(AgentItem agent);

        internal static bool FindChestGadget(ArcDPSEnums.ChestID chestID, AgentData agentData, IReadOnlyList<CombatItem> combatData, Point3D chestPosition, ChestAgentChecker chestChecker)
        {
            if (chestID == ArcDPSEnums.ChestID.None)
            {
                return false;
            }
            AgentItem chest = combatData.Where(evt =>
            {
                if (evt.IsStateChange != ArcDPSEnums.StateChange.Position)
                {
                    return false;
                }
                AgentItem agent = agentData.GetAgent(evt.SrcAgent, evt.Time);
                if (agent.Type != AgentItem.AgentType.Gadget)
                {
                    return false;
                }
                Point3D position = AbstractMovementEvent.GetPoint3D(evt.DstAgent, evt.Value);
                if (position.Distance2DToPoint(chestPosition) < InchDistanceThreshold)
                {
                    return true;
                }
                return false;
            }
            ).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).FirstOrDefault(x => chestChecker(x));
            if (chest != null)
            {
                chest.OverrideID(chestID);
                return true;
            }
            return false;
        }

        internal static string AddNameSuffixBasedOnInitialPosition(AbstractSingleActor target, IReadOnlyList<CombatItem> combatData, IReadOnlyCollection<(string, Point3D)> positionData, float maxDiff = InchDistanceThreshold)
        {
            CombatItem positionEvt = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target.AgentItem) && x.IsStateChange == ArcDPSEnums.StateChange.Position);
            if (positionEvt != null)
            {
                Point3D position = AbstractMovementEvent.GetPoint3D(positionEvt.DstAgent, 0);
                foreach ((string suffix, Point3D expectedPosition) in positionData)
                {
                    if (position.Distance2DToPoint(expectedPosition) < maxDiff)
                    {
                        target.OverrideName(target.Character + " " + suffix);
                        return suffix;
                    }
                }
            }
            return null;
        }
    }
}
