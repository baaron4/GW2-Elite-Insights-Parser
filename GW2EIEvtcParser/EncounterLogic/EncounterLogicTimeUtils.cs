﻿using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal static class EncounterLogicTimeUtils
{

    internal static long GetGenericFightOffset(FightData fightData)
    {
        return fightData.LogStart;
    }

    internal static long GetPostLogStartNPCUpdateDamageEventTime(FightData fightData, AgentData agentData, IReadOnlyList<CombatItem> combatData, long upperLimit, AgentItem? mainTarget)
    {
        if (mainTarget == null)
        {
            throw new MissingKeyActorsException("Main target not found");
        }
        CombatItem? firstDamageEvent = combatData.FirstOrDefault(x => (x.SrcMatchesAgent(mainTarget) || x.DstMatchesAgent(mainTarget)) && x.IsDamagingDamage() && x.Time >= upperLimit - ParserHelper.ServerDelayConstant);
        if (firstDamageEvent != null)
        {
            return firstDamageEvent.Time;
        }
        return upperLimit;
    }

    internal static long GetFirstDamageEventTime(FightData fightData, AgentData agentData, IReadOnlyList<CombatItem> combatData, AgentItem? mainTarget)
    {
        if (mainTarget == null)
        {
            throw new MissingKeyActorsException("Main target not found");
        }
        CombatItem? firstDamageEvent = combatData.FirstOrDefault(x => (x.SrcMatchesAgent(mainTarget) || x.DstMatchesAgent(mainTarget)) && x.IsDamagingDamage());
        if (firstDamageEvent != null)
        {
            return firstDamageEvent.Time;
        }
        return mainTarget.FirstAware;
    }

    internal static long GetEnterCombatTime(FightData fightData, AgentData agentData, IReadOnlyList<CombatItem> combatData, long upperLimit, int[] ids, ulong agent)
    {
        long start = long.MaxValue;
        foreach (int id in ids)
        {
            AgentItem target = (agentData.GetNPCsByIDAndAgent(id, agent).FirstOrDefault() ?? agentData.GetNPCsByID(id).FirstOrDefault(x => x.InAwareTimes(upperLimit))) ?? throw new MissingKeyActorsException("Main target not found");
            upperLimit = GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, upperLimit, target);
            CombatItem? enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.EnterCombat && x.SrcMatchesAgent(target) && x.Time <= upperLimit + ParserHelper.TimeThresholdConstant);
            if (enterCombat != null)
            {
                CombatItem? exitCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.ExitCombat && x.SrcMatchesAgent(target) && x.Time <= enterCombat.Time);
                if (exitCombat != null)
                {
                    start = Math.Min(target.FirstAware, start);
                }
                else
                {
                    start = Math.Min(enterCombat.Time, start);
                }
            }
            else
            {
                start = Math.Min(target.FirstAware, start);
            }
        }
        if (start == long.MaxValue)
        {
            return fightData.FightStart;
        }
        return start;
    }

    internal static long GetEnterCombatTime(FightData fightData, AgentData agentData, IReadOnlyList<CombatItem> combatData, long upperLimit, int id, ulong agent)
    {
        return GetEnterCombatTime(fightData, agentData, combatData, upperLimit, [id], agent);
    }

    internal static long GetFightOffsetForTarget(FightData fightData, AgentItem target)
    {
        return Math.Max(target.FirstAware, GetGenericFightOffset(fightData));
    }

    internal static long GetFightOffsetBySpawn(FightData fightData, List<CombatItem> combatData, AgentItem target)
    {
        long start = GetFightOffsetForTarget(fightData, target);
        CombatItem? spawn = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.Spawn && x.SrcMatchesAgent(target) && x.Time <= start + ParserHelper.TimeThresholdConstant);
        if (spawn != null)
        {
            return Math.Max(start, spawn.Time);
        }
        return start;
    }

    internal static long GetFightOffsetByInvulnStart(FightData fightData, List<CombatItem> combatData, AgentItem target, long invulnID)
    {
        long start = GetFightOffsetForTarget(fightData, target);
        CombatItem? invulnRemove = combatData.FirstOrDefault(x => x.SkillID == invulnID && x.IsBuffRemove == BuffRemove.All && x.SrcMatchesAgent(target) && x.Time >= start);
        if (invulnRemove != null)
        {
            CombatItem? invulnApply = combatData.FirstOrDefault(x => x.SkillID == invulnID && x.DstMatchesAgent(target) && x.IsBuffApply() && x.Time >= start);
            if (invulnApply != null && invulnApply.Time <= invulnRemove.Time)
            {
                // check if invuln was applied at start rather than already present (otherwise could be a late start)
                // this is a faster check for targets that get their starting invuln applied as they spawn or as the encounter starts
                if (invulnApply.IsStateChange == StateChange.None && invulnApply.Time <= target.FirstAware + ParserHelper.TimeThresholdConstant)
                {
                    return invulnRemove.Time;
                }

                // try to verify via target combat enter after invuln apply
                CombatItem? enterCombat = GetEnterCombatForStart(combatData, target, start);
                if (enterCombat != null && enterCombat.Time + ParserHelper.ServerDelayConstant >= invulnApply.Time)
                {
                    return invulnRemove.Time;
                }
            }
            else
            {
                // no matching apply recorded, try to verify via combat enter after invuln remove
                CombatItem? enterCombat = GetEnterCombatForStart(combatData, target, start);
                if (enterCombat != null && enterCombat.Time >= invulnRemove.Time)
                {
                    return invulnRemove.Time;
                }
            }
        }
        return start;
    }

    internal static CombatItem? GetEnterCombatForStart(List<CombatItem> combatData, AgentItem target, long start)
    {
        CombatItem? enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.EnterCombat && x.SrcMatchesAgent(target) && x.Time >= start);
        CombatItem? exitCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.ExitCombat && x.SrcMatchesAgent(target) && x.Time >= start);
        if (enterCombat != null && (exitCombat == null || exitCombat.Time > enterCombat.Time))
        {
            return enterCombat;
        }
        return null;
    }

    internal static void SetSuccessByCombatExit(IEnumerable<SingleActor> targets, CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        if (!targets.Any())
        {
            return;
        }
        var targetExits = new List<ExitCombatEvent>();
        var lastTargetDamages = new List<HealthDamageEvent>();
        foreach (SingleActor t in targets)
        {
            EnterCombatEvent? enterCombat = combatData.GetEnterCombatEvents(t.AgentItem).LastOrDefault();
            ExitCombatEvent? exitCombat;
            if (enterCombat != null)
            {
                exitCombat = combatData.GetExitCombatEvents(t.AgentItem).Where(x => x.Time > enterCombat.Time).LastOrDefault();
            }
            else
            {
                exitCombat = combatData.GetExitCombatEvents(t.AgentItem).LastOrDefault();
            }
            HealthDamageEvent? lastDamage = combatData.GetDamageTakenData(t.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Any(x.From.IsMasterOrSelf));
            if (exitCombat == null || lastDamage == null ||
                combatData.GetAnimatedCastData(t.AgentItem).Any(x => x.Time > exitCombat.Time + ParserHelper.ServerDelayConstant) ||
                combatData.GetDamageData(t.AgentItem).Any(x => x.Time > exitCombat.Time + ParserHelper.ServerDelayConstant))
            {
                return;
            }
            targetExits.Add(exitCombat);
            lastTargetDamages.Add(lastDamage);
        }
        ExitCombatEvent? lastTargetExit = targetExits.Count > 0 ? targetExits.MaxBy(x => x.Time) : null;
        HealthDamageEvent? lastDamageTaken = lastTargetDamages.Count > 0 ? lastTargetDamages.MaxBy(x => x.Time) : null;
        // Make sure the last damage has been done before last combat exit
        if (lastTargetExit != null && lastDamageTaken != null && lastTargetExit.Time + ParserHelper.TimeThresholdConstant >= lastDamageTaken.Time)
        {
            if (!AtLeastOnePlayerAlive(combatData, fightData, lastTargetExit.Time, playerAgents))
            {
                return;
            }
            fightData.SetSuccess(true, lastDamageTaken.Time);
        }
    }

    internal static void SetSuccessByChestGadget(ChestID chestID, AgentData agentData, FightData fightData)
    {
        AgentItem? chest = agentData.GetGadgetsByID(chestID).FirstOrDefault();
        if (chest != null)
        {
            fightData.SetSuccess(true, chest.FirstAware);
        }
    }

    internal static void SetSuccessByDeath(IEnumerable<SingleActor> targets, CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents, bool all)
    {
        if (!targets.Any())
        {
            return;
        }
        int success = 0;
        long maxTime = long.MinValue;
        foreach (SingleActor target in targets)
        {
            DeadEvent? killed = combatData.GetDeadEvents(target.AgentItem).LastOrDefault();
            if (killed != null)
            {
                long time = killed.Time;
                success++;
                HealthDamageEvent? lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Any(x.From.IsMasterOrSelf));
                if (lastDamageTaken != null)
                {
                    time = Math.Min(lastDamageTaken.Time, time);
                }
                maxTime = Math.Max(time, maxTime);
            }
        }
        if ((all && success == targets.Count()) || (!all && success > 0))
        {
            fightData.SetSuccess(true, maxTime);
        }
    }

}
