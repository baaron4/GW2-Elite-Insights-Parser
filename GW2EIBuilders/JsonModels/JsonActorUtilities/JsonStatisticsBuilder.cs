using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;
using static GW2EIJSON.JsonStatistics;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities
{
    /// <summary>
    /// Class representing general statistics
    /// </summary>
    internal static class JsonStatisticsBuilder
    {
        public static JsonDefensesAll BuildJsonDefensesAll(FinalDefensesAll defStats)
        {
            var jsonDefensesAll = new JsonDefensesAll();
            jsonDefensesAll.DamageTaken = defStats.DamageTaken;
            jsonDefensesAll.BreakbarDamageTaken = defStats.BreakbarDamageTaken;
            jsonDefensesAll.BlockedCount = defStats.BlockedCount;
            jsonDefensesAll.DodgeCount = defStats.DodgeCount;
            jsonDefensesAll.MissedCount = defStats.MissedCount;
            jsonDefensesAll.EvadedCount = defStats.EvadedCount;
            jsonDefensesAll.InvulnedCount = defStats.InvulnedCount;
            jsonDefensesAll.DamageBarrier = defStats.DamageBarrier;
            jsonDefensesAll.InterruptedCount = defStats.InterruptedCount;
            jsonDefensesAll.DownCount = defStats.DownCount;
            jsonDefensesAll.DownDuration = defStats.DownDuration;
            jsonDefensesAll.DeadCount = defStats.DeadCount;
            jsonDefensesAll.DeadDuration = defStats.DeadDuration;
            jsonDefensesAll.DcCount = defStats.DcCount;
            jsonDefensesAll.DcDuration = defStats.DcDuration;
            jsonDefensesAll.BoonStrips = defStats.BoonStrips;
            jsonDefensesAll.BoonStripsTime = defStats.BoonStripsTime;
            jsonDefensesAll.ConditionCleanses = defStats.ConditionCleanses;
            jsonDefensesAll.ConditionCleansesTime = defStats.ConditionCleansesTime;
            return jsonDefensesAll;
        }


        public static JsonDPS BuildJsonDPS(FinalDPS dpsStats)
        {
            var jsonDPS = new JsonDPS
            {
                Dps = dpsStats.Dps,
                Damage = dpsStats.Damage,
                CondiDps = dpsStats.CondiDps,
                CondiDamage = dpsStats.CondiDamage,
                PowerDps = dpsStats.PowerDps,
                PowerDamage = dpsStats.PowerDamage,
                BreakbarDamage = dpsStats.BreakbarDamage,

                ActorDps = dpsStats.ActorDps,
                ActorDamage = dpsStats.ActorDamage,
                ActorCondiDps = dpsStats.ActorCondiDps,
                ActorCondiDamage = dpsStats.ActorCondiDamage,
                ActorPowerDps = dpsStats.ActorPowerDps,
                ActorPowerDamage = dpsStats.ActorPowerDamage,
                ActorBreakbarDamage = dpsStats.ActorBreakbarDamage
            };

            return jsonDPS;
        }

        private static void FillJsonGamePlayStats(JsonGameplayStats jsonGameplayStats, FinalOffensiveStats offStats)
        {
            jsonGameplayStats.TotalDamageCount = offStats.TotalDamageCount;
            jsonGameplayStats.DirectDamageCount = offStats.DirectDamageCount;
            jsonGameplayStats.ConnectedDirectDamageCount = offStats.ConnectedDirectDamageCount;
            jsonGameplayStats.ConnectedDamageCount = offStats.ConnectedDamageCount;
            jsonGameplayStats.CritableDirectDamageCount = offStats.CritableDirectDamageCount;
            jsonGameplayStats.CriticalRate = offStats.CriticalCount;
            jsonGameplayStats.CriticalDmg = offStats.CriticalDmg;
            jsonGameplayStats.FlankingRate = offStats.FlankingCount;
            jsonGameplayStats.GlanceRate = offStats.GlanceCount;
            jsonGameplayStats.AgainstMovingRate = offStats.AgainstMovingCount;
            jsonGameplayStats.Missed = offStats.Missed;
            jsonGameplayStats.Blocked = offStats.Blocked;
            jsonGameplayStats.Evaded = offStats.Evaded;
            jsonGameplayStats.Interrupts = offStats.Interrupts;
            jsonGameplayStats.Invulned = offStats.Invulned;
            jsonGameplayStats.Killed = offStats.Killed;
            jsonGameplayStats.Downed = offStats.Downed;
            jsonGameplayStats.DownContribution = offStats.DownContribution;
        }

        public static JsonGameplayStats BuildJsonGameplayStats(FinalOffensiveStats offStats)
        {
            var jsonGameplayStats = new JsonGameplayStats();
            FillJsonGamePlayStats(jsonGameplayStats, offStats);
            return jsonGameplayStats;
        }

        public static JsonGameplayStatsAll BuildJsonGameplayStatsAll(FinalGameplayStats gameStats, FinalOffensiveStats offStats)
        {
            var jsonGameplayStatsAll = new JsonGameplayStatsAll
            {
                Wasted = gameStats.Wasted,
                TimeWasted = gameStats.TimeWasted,
                Saved = gameStats.Saved,
                TimeSaved = gameStats.TimeSaved,
                StackDist = gameStats.StackDist,
                DistToCom = gameStats.DistToCom,
                AvgBoons = gameStats.AvgBoons,
                AvgActiveBoons = gameStats.AvgActiveBoons,
                AvgConditions = gameStats.AvgConditions,
                AvgActiveConditions = gameStats.AvgActiveConditions,
                SwapCount = gameStats.SwapCount,
                SkillCastUptime = gameStats.SkillCastUptime,
                SkillCastUptimeNoAA = gameStats.SkillCastUptimeNoAA,
            };
            FillJsonGamePlayStats(jsonGameplayStatsAll, offStats);
            return jsonGameplayStatsAll;
        }


        public static JsonPlayerSupport BuildJsonPlayerSupport(FinalToPlayersSupport playerToPlayerStats)
        {
            var jsonPlayerSupport = new JsonPlayerSupport
            {
                Resurrects = playerToPlayerStats.Resurrects,
                ResurrectTime = playerToPlayerStats.ResurrectTime,
                CondiCleanse = playerToPlayerStats.CondiCleanse,
                CondiCleanseTime = playerToPlayerStats.CondiCleanseTime,
                CondiCleanseSelf = playerToPlayerStats.CondiCleanseSelf,
                CondiCleanseTimeSelf = playerToPlayerStats.CondiCleanseTimeSelf,
                BoonStrips = playerToPlayerStats.BoonStrips,
                BoonStripsTime = playerToPlayerStats.BoonStripsTime
            };
            return jsonPlayerSupport;
        }
    }
}
