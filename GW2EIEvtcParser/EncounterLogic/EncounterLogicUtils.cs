﻿using System.Numerics;
using System.Threading.Tasks;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal static class EncounterLogicUtils
{

    internal static bool TargetHPPercentUnderThreshold(int targetID, long time, CombatData combatData, IReadOnlyList<SingleActor> targets, double expectedInitialPercent = 100.0)
    {
        SingleActor? target = targets.FirstOrDefault(x => x.IsSpecies(targetID));
        if (target == null)
        {
            // If tracked target is missing, then 0% hp
            return true;
        }
        long minTime = Math.Max(target.FirstAware, time);
        HealthUpdateEvent? hpUpdate = combatData.GetHealthUpdateEvents(target.AgentItem).FirstOrDefault(x => x.Time >= minTime && (x.Time > target.FirstAware + 100 || x.HealthPercent > 0));
        var targetTotalHP = target.GetHealth(combatData);
        if (hpUpdate == null || targetTotalHP < 0)
        {
            // If for some reason hp events are missing, we can't decide
            return false;
        }
        var damagingPlayers = new HashSet<AgentItem>(combatData.GetDamageTakenData(target.AgentItem).Where(x => x.CreditedFrom.IsPlayer).Select(x => x.CreditedFrom));
        long damageDoneWithinOneSec = combatData.GetDamageTakenData(target.AgentItem).Where(x => x.Time >= time && x.Time <= time + 1000).Sum(x => x.HealthDamage);
        double damageThreshold = Math.Max(damagingPlayers.Count * 80000, 2 * damageDoneWithinOneSec);
        double threshold = (expectedInitialPercent / 100.0 - damageThreshold / targetTotalHP) * 100;
        return hpUpdate.HealthPercent < threshold - 2;
    }

    internal static bool TargetHPPercentUnderThreshold(TargetID targetID, long time, CombatData combatData, IReadOnlyList<SingleActor> targets, double expectedInitialPercent = 100.0)
    {
        return TargetHPPercentUnderThreshold((int)targetID, time, combatData, targets, expectedInitialPercent);
    }

    internal static void NegateDamageAgainstBarrier(CombatData combatData, AgentData agentData, IEnumerable<TargetID> targetIDs)
    {
        var agentItems = new List<AgentItem>();
        foreach (var targetID in targetIDs)
        {
            agentItems.AddRange(agentData.GetNPCsByID(targetID));
        }
        var dmgEvts = new List<HealthDamageEvent>();
        foreach (AgentItem agentItem in agentItems)
        {
            dmgEvts.AddRange(combatData.GetDamageTakenData(agentItem));
        }
        foreach (HealthDamageEvent de in dmgEvts)
        {
            if (de.ShieldDamage > 0)
            {
                de.NegateShieldDamage();
            }
        }
    }

    internal static ErrorEvent? GetConfusionDamageMissingMessage(EvtcVersionEvent evtcVersion)
    {
        if (evtcVersion.Build > ArcDPSBuilds.ProperConfusionDamageSimulation)
        {
            return null;
        }
        return new("Missing confusion damage");
    }
    internal static List<BuffEvent> GetBuffApplyRemoveSequence(CombatData combatData, long buffID, AgentItem target, bool beginWithApply, bool addDummyRemoveAllEventAtEnd)
    {
        bool needStart = beginWithApply;
        var main = combatData.GetBuffDataByIDByDst(buffID, target).Where(x => (x is BuffApplyEvent || x is BuffRemoveAllEvent)).ToList();
        var filtered = new List<BuffEvent>();
        for (int i = 0; i < main.Count; i++)
        {
            BuffEvent c = main[i];
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
        if (addDummyRemoveAllEventAtEnd && filtered.Count != 0 && filtered.Last() is BuffApplyEvent)
        {
            BuffEvent last = filtered.Last();
            filtered.Add(new BuffRemoveAllEvent(_unknownAgent, last.To, target.LastAware, int.MaxValue, last.BuffSkill, IFF.Unknown, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
        }
        return filtered;
    }

    internal static IEnumerable<BuffEvent> GetBuffApplyRemoveSequence(CombatData combatData, long buffID, SingleActor target, bool beginWithApply, bool addDummyRemoveAllEventAtEnd)
    {
        return GetBuffApplyRemoveSequence(combatData, buffID, target.AgentItem, beginWithApply, addDummyRemoveAllEventAtEnd);
    }

    internal static IEnumerable<BuffEvent> GetBuffApplyRemoveSequence(CombatData combatData, IEnumerable<long> buffIDs, AgentItem target, bool beginWithApply, bool addDummyRemoveAllEventAtEnd)
    {
        return buffIDs.SelectMany(buffID => GetBuffApplyRemoveSequence(combatData, buffID, target, beginWithApply, addDummyRemoveAllEventAtEnd));
    }

    internal static IEnumerable<BuffEvent> GetBuffApplyRemoveSequence(CombatData combatData, IEnumerable<long> buffIDs, SingleActor target, bool beginWithApply, bool addDummyRemoveAllEventAtEnd)
    {
        return GetBuffApplyRemoveSequence(combatData, buffIDs, target.AgentItem, beginWithApply, addDummyRemoveAllEventAtEnd);
    }

    internal static bool AtLeastOnePlayerAlive(CombatData combatData, FightData fightData, long timeToCheck, IReadOnlyCollection<AgentItem> playerAgents)
    {
        int playerDeadOrDCCount = 0;
        foreach (AgentItem playerAgent in playerAgents)
        {
            if (timeToCheck < playerAgent.FirstAware || timeToCheck > playerAgent.LastAware)
            {
                playerDeadOrDCCount++;
            }
            else
            {
                var statusEvents = new List<StatusEvent>();
                statusEvents.AddRange(combatData.GetAliveEvents(playerAgent));
                statusEvents.AddRange(combatData.GetDownEvents(playerAgent));
                statusEvents.AddRange(combatData.GetDeadEvents(playerAgent));
                statusEvents.AddRange(combatData.GetSpawnEvents(playerAgent));
                statusEvents.AddRange(combatData.GetDespawnEvents(playerAgent));
                statusEvents.SortByTime();
                var lastStatus = statusEvents.LastOrDefault(x => x.Time <= timeToCheck + ServerDelayConstant);
                if (lastStatus is DeadEvent || lastStatus is DespawnEvent)
                {
                    playerDeadOrDCCount++;
                }
            }
        }
        if (playerDeadOrDCCount == playerAgents.Count)
        {
            return false;
        }
        return true;
    }

    internal static void NumericallyRenameSpecies(IEnumerable<SingleActor> targets, HashSet<int> ignoredSpecies)
    {
        var targetsByID = targets.Where(x => !x.AgentItem.IsPlayer).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var pair in targetsByID)
        {
            if (ignoredSpecies.Contains(pair.Key))
            {
                continue;
            }
            if (pair.Value.Count > 1)
            {
                var count = 1;
                foreach (var target in pair.Value)
                {
                    target.OverrideName(target.Character + " " + count++);
                }
            }
        }
    }


    internal delegate bool ChestAgentChecker(AgentItem agent);

    internal static AgentItem? FindChestGadget(ChestID chestID, AgentData agentData, IReadOnlyList<CombatItem> combatData, Vector3 chestPosition, ChestAgentChecker? chestChecker = null)
    {
        if (chestID == ChestID.None)
        {
            return null;
        }

        var positions = combatData.Where(evt => {
            if (evt.IsStateChange != StateChange.Position)
            {
                return false;
            }

            AgentItem agent = agentData.GetAgent(evt.SrcAgent, evt.Time);
            if (agent.Type != AgentItem.AgentType.Gadget)
            {
                return false;
            }

            var position = MovementEvent.GetPoint3D(evt);
            if ((position - chestPosition).XY().Length() < InchDistanceThreshold)
            {
                return true;
            }

            return false;
        });

        var velocities = combatData.Where(evt => {
            if (evt.IsStateChange == StateChange.Velocity)
            {
                AgentItem agent = agentData.GetAgent(evt.SrcAgent, evt.Time);
                if (agent.Type != AgentItem.AgentType.Gadget)
                {
                    return false;
                }
                return positions.Any(x => x.SrcMatchesAgent(agent));
            }
            return false;
        });

        var candidates = positions.Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Distinct().ToList();
        // Remove all candidates who moved, chests can not move
        candidates.RemoveAll(candidate => velocities.Where(evt => evt.SrcMatchesAgent(candidate)).Any(evt => MovementEvent.GetPoint3D(evt).Length() >= 1e-6));
        var chest = candidates.FirstOrDefault(x => chestChecker == null || chestChecker(x));
        if (chest != null)
        {
            chest.OverrideID(chestID, agentData);
            return chest;
        }
        return null;
    }

    internal static string? AddNameSuffixBasedOnInitialPosition(SingleActor target, IReadOnlyList<CombatItem> combatData, IReadOnlyCollection<(string, Vector2)> positionData, float maxDiff = InchDistanceThreshold)
    {
        var positionEvt = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target.AgentItem) && x.IsStateChange == StateChange.Position);
        if (positionEvt != null)
        {
            var position = MovementEvent.GetPoint3D(positionEvt).XY();
            foreach (var (suffix, expectedPosition) in positionData)
            {
                if ((position - expectedPosition).Length() < maxDiff)
                {
                    target.OverrideName(target.Character + " " + suffix);
                    return suffix;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Compute the cast duration while the target has <see cref="Quickness"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="actor">Actor casting.</param>
    /// <param name="startCastTime">Starting time of the cast.</param>
    /// <param name="castDuration">Duration of the cast.</param>
    /// <returns>The duration of the cast.</returns>
    internal static double ComputeCastTimeWithQuickness(ParsedEvtcLog log, SingleActor actor, long startCastTime, long castDuration)
    {
        long expectedEndCastTime = startCastTime + castDuration;
        Segment? quickness = actor.GetBuffStatus(log, Quickness, startCastTime, expectedEndCastTime).FirstOrNull((in Segment x) => x.Value == 1);
        if (quickness != null)
        {
            long quicknessTimeDuringCast = Math.Min(expectedEndCastTime, quickness.Value.End) - Math.Max(startCastTime, quickness.Value.Start);
            return castDuration - quicknessTimeDuringCast + (quicknessTimeDuringCast * 0.66);
        }
        return 0;
    }

    /// <summary>
    /// Compute the cast duration while the target has <see cref="MistlockInstabilitySugarRush"/>.
    /// </summary>
    /// <param name="castDuration">Duration of the cast.</param>
    /// <returns>The duration of the cast.</returns>
    internal static double ComputeCastTimeWithSugarRush(long castDuration)
    {
        return castDuration * 0.8;
    }

    /// <summary>
    /// Compute the cast duration while the target has <see cref="Quickness"/> and <see cref="MistlockInstabilitySugarRush"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="actor">Actor casting.</param>
    /// <param name="startCastTime">Starting time of the cast.</param>
    /// <param name="castDuration">Duration of the cast.</param>
    /// <returns>The duration of the cast.</returns>
    internal static double ComputeCastTimeWithQuicknessAndSugarRush(ParsedEvtcLog log, SingleActor actor, long startCastTime, long castDuration)
    {
        long expectedEndCastTime = startCastTime + castDuration;
        Segment? quickness = actor.GetBuffStatus(log, Quickness, startCastTime, expectedEndCastTime).FirstOrNull((in Segment x) => x.Value == 1);
        if (quickness != null)
        {
            long quicknessTimeDuringCast = Math.Min(expectedEndCastTime, quickness.Value.End) - Math.Max(startCastTime, quickness.Value.Start);
            double castTimeWithSugarRush = ComputeCastTimeWithSugarRush(castDuration);
            return castTimeWithSugarRush - quicknessTimeDuringCast + (quicknessTimeDuringCast * 0.66 / 0.8);
        }
        return 0;
    }

    /// <summary>
    /// Compute the end time of a cast.<br></br>
    /// If <paramref name="buffID"/> is present before the end of the cast, return the <paramref name="buffID"/> application time.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="actor">Actor casting.</param>
    /// <param name="buffID">Buff application that ends the cast earlier than the expected duration.</param>
    /// <param name="startCastTime">Starting time of the cast.</param>
    /// <param name="castDuration">Duration of the cast.</param>
    /// <returns>The end time of the cast.</returns>
    internal static long ComputeEndCastTimeByBuffApplication(ParsedEvtcLog log, SingleActor actor, long buffID, long startCastTime, long castDuration)
    {
        long end = startCastTime + castDuration;
        Segment? segment = actor.GetBuffStatus(log, buffID, startCastTime, end).FirstOrNull((in Segment x) => x.Value > 0);
        if (segment != null)
        {
            return segment.Value.Start;
        }
        return end;
    }


    /// <summary>
    /// Matches an effect to another effect by proximity and filters out additional effects.
    /// </summary>
    /// <param name="startEffects">List of the initial effects for the positions.</param>
    /// <param name="endEffects">List of the final effects for the positions.</param>
    /// <returns>Filtered list with matched <paramref name="startEffects"/>, <paramref name="endEffects"/> and distance between them.</returns>
    internal static List<(EffectEvent endEffect, EffectEvent startEffect, float distance)> MatchEffectToEffect(IEnumerable<EffectEvent> startEffects, IEnumerable<EffectEvent> endEffects)
    {
        var matchedEffects = new List<(EffectEvent, EffectEvent, float)>(); //TODO(Rennorb) @perf
        foreach (EffectEvent startEffect in startEffects)
        {
            var candidateEffectEvents = endEffects.Where(x => x.Time > startEffect.Time + 200 && Math.Abs(x.Time - startEffect.Time) < 10000);
            if (candidateEffectEvents.Any())
            {
                EffectEvent matchedEffect = candidateEffectEvents.MinBy(x => (x.Position - startEffect.Position).LengthSquared()); //TODO(Rennorb) @perf
                float minimalDistance = (matchedEffect.Position - startEffect.Position).Length();
                matchedEffects.Add((matchedEffect, startEffect, minimalDistance));
            }
        }

        var filteredPairings = matchedEffects
            .GroupBy(p => p.Item1) // Group by aoe
            .SelectMany(group =>
            {
                var minDistance = group.Min(p => p.Item3); // Find minimal distance in each group
                return group.Where(p => Math.Abs(p.Item3 - minDistance) < float.Epsilon); // Filter by minimal distance
            })
            .ToList();

        return filteredPairings;
    }
    internal static int AddSortIDWithOffset(Dictionary<TargetID, int> toFill, IReadOnlyDictionary<TargetID, int> toFillFrom, int inputOffset)
    {
        int offset = 0;
        foreach (var pair in toFillFrom)
        {
            toFill[pair.Key] = pair.Value + inputOffset;
            offset = pair.Value + inputOffset;
        }
        return offset;
    }
}
