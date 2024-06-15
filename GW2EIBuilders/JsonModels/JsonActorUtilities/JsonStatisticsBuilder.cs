using GW2EIEvtcParser.EIData;
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
            var jsonDefensesAll = new JsonDefensesAll
            {
                DamageTaken = defStats.DamageTaken,
                ConditionDamageTaken = defStats.ConditionDamageTaken,
                StrikeDamageTaken = defStats.StrikeDamageTaken,
                LifeLeechDamageTaken = defStats.LifeLeechDamageTaken,
                PowerDamageTaken = defStats.PowerDamageTaken,
                DownedDamageTaken = defStats.DownedDamageTaken,
                BreakbarDamageTaken = defStats.BreakbarDamageTaken,
                BlockedCount = defStats.BlockedCount,
                DodgeCount = defStats.DodgeCount,
                MissedCount = defStats.MissedCount,
                EvadedCount = defStats.EvadedCount,
                InvulnedCount = defStats.InvulnedCount,
                DamageBarrier = defStats.DamageBarrier,
                InterruptedCount = defStats.InterruptedCount,
                DownCount = defStats.DownCount,
                DownDuration = defStats.DownDuration,
                DeadCount = defStats.DeadCount,
                DeadDuration = defStats.DeadDuration,
                DcCount = defStats.DcCount,
                DcDuration = defStats.DcDuration,
                BoonStrips = defStats.BoonStrips,
                BoonStripsTime = defStats.BoonStripsTime,
                ConditionCleanses = defStats.ConditionCleanses,
                ConditionCleansesTime = defStats.ConditionCleansesTime
            };
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
            jsonGameplayStats.TotalDmg = offStats.TotalDmg;
            jsonGameplayStats.DirectDamageCount = offStats.DirectDamageCount;
            jsonGameplayStats.DirectDmg = offStats.DirectDmg;
            jsonGameplayStats.ConnectedDirectDamageCount = offStats.ConnectedDirectDamageCount;
            jsonGameplayStats.ConnectedDirectDmg = offStats.ConnectedDirectDmg;
            jsonGameplayStats.ConnectedDamageCount = offStats.ConnectedDamageCount;
            jsonGameplayStats.ConnectedDmg = offStats.ConnectedDmg;
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
            jsonGameplayStats.ConnectedConditionCount = offStats.ConnectedConditionCount;
            jsonGameplayStats.ConnectedConditionAbove90HPCount = offStats.ConnectedConditionAbove90HPCount;
            jsonGameplayStats.ConnectedPowerAbove90HPCount = offStats.ConnectedPowerAbove90HPCount;
            jsonGameplayStats.ConnectedPowerCount = offStats.ConnectedPowerCount;
            jsonGameplayStats.AgainstDownedCount = offStats.AgainstDownedCount;
            jsonGameplayStats.AgainstDownedDamage = offStats.AgainstDownedDamage;
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
