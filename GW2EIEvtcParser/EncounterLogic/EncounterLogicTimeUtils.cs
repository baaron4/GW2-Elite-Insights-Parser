using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal static class EncounterLogicTimeUtils
    {

        internal static long GetGenericFightOffset(FightData fightData)
        {
            return fightData.LogStart;

        }

        internal static long GetPostLogStartNPCUpdateDamageEventTime(FightData fightData, AgentData agentData, IReadOnlyList<CombatItem> combatData, long upperLimit, AgentItem mainTarget)
        {
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target not found");
            }
            CombatItem firstDamageEvent = combatData.FirstOrDefault(x => (x.SrcMatchesAgent(mainTarget) || x.DstMatchesAgent(mainTarget)) && x.IsDamagingDamage() && x.Time >= upperLimit - ParserHelper.ServerDelayConstant);
            if (firstDamageEvent != null)
            {
                return firstDamageEvent.Time;
            }
            return upperLimit;
        }

        internal static long GetEnterCombatTime(FightData fightData, AgentData agentData, IReadOnlyList<CombatItem> combatData, long upperLimit, int id, ulong agent)
        {
            AgentItem mainTarget = agentData.GetNPCsByIDAndAgent(id, agent).FirstOrDefault() ?? agentData.GetNPCsByID(id).FirstOrDefault();
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target not found");
            }
            upperLimit = GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, upperLimit, mainTarget);
            CombatItem enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat && x.SrcMatchesAgent(mainTarget) && x.Time <= upperLimit + ParserHelper.TimeThresholdConstant);
            if (enterCombat != null)
            {
                CombatItem exitCombat = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.ExitCombat && x.SrcMatchesAgent(mainTarget) && x.Time <= enterCombat.Time);
                if (exitCombat != null)
                {
                    return mainTarget.FirstAware;
                }
                return enterCombat.Time;
            }
            return mainTarget.FirstAware;
        }

        internal static void SetSuccessByCombatExit(IReadOnlyList<AbstractSingleActor> targets, CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            if (targets.Count == 0)
            {
                return;
            }
            var targetExits = new List<ExitCombatEvent>();
            var lastTargetDamages = new List<AbstractHealthDamageEvent>();
            foreach (AbstractSingleActor t in targets)
            {
                EnterCombatEvent enterCombat = combatData.GetEnterCombatEvents(t.AgentItem).LastOrDefault();
                ExitCombatEvent exitCombat;
                if (enterCombat != null)
                {
                    exitCombat = combatData.GetExitCombatEvents(t.AgentItem).Where(x => x.Time > enterCombat.Time).LastOrDefault();
                }
                else
                {
                    exitCombat = combatData.GetExitCombatEvents(t.AgentItem).LastOrDefault();
                }
                AbstractHealthDamageEvent lastDamage = combatData.GetDamageTakenData(t.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (exitCombat == null || lastDamage == null ||
                    combatData.GetAnimatedCastData(t.AgentItem).Any(x => x.Time > exitCombat.Time + ParserHelper.ServerDelayConstant) ||
                    combatData.GetDamageData(t.AgentItem).Any(x => x.Time > exitCombat.Time + ParserHelper.ServerDelayConstant && playerAgents.Contains(x.To)))
                {
                    return;
                }
                targetExits.Add(exitCombat);
                lastTargetDamages.Add(lastDamage);
            }
            ExitCombatEvent lastTargetExit = targetExits.Count > 0 ? targetExits.MaxBy(x => x.Time) : null;
            AbstractHealthDamageEvent lastDamageTaken = lastTargetDamages.Count > 0 ? lastTargetDamages.MaxBy(x => x.Time) : null;
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

        internal static void SetSuccessByChestGadget(ArcDPSEnums.ChestID chestID, AgentData agentData, FightData fightData)
        {
            if (chestID == ArcDPSEnums.ChestID.None)
            {
                return;
            }
            AgentItem chest = agentData.GetGadgetsByID(chestID).FirstOrDefault();
            if (chest != null)
            {
                fightData.SetSuccess(true, chest.FirstAware);
            }
        }

        internal static void SetSuccessByDeath(IReadOnlyList<AbstractSingleActor> targets, CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents, bool all)
        {
            if (!targets.Any())
            {
                return;
            }
            int success = 0;
            long maxTime = long.MinValue;
            foreach (AbstractSingleActor target in targets)
            {
                DeadEvent killed = combatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                if (killed != null)
                {
                    long time = killed.Time;
                    success++;
                    AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTaken != null)
                    {
                        time = Math.Min(lastDamageTaken.Time, time);
                    }
                    maxTime = Math.Max(time, maxTime);
                }
            }
            if ((all && success == targets.Count) || (!all && success > 0))
            {
                fightData.SetSuccess(true, maxTime);
            }
        }

    }
}
