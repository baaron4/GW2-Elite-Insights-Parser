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
        public static JsonDefensesAll BuildJsonDefensesAll(FinalDefensesAll defenses)
        {
            var jsonDefensesAll = new JsonDefensesAll();
            jsonDefensesAll.DamageTaken = defenses.DamageTaken;
            jsonDefensesAll.BreakbarDamageTaken = defenses.BreakbarDamageTaken;
            jsonDefensesAll.BlockedCount = defenses.BlockedCount;
            jsonDefensesAll.DodgeCount = defenses.DodgeCount;
            jsonDefensesAll.MissedCount = defenses.MissedCount;
            jsonDefensesAll.EvadedCount = defenses.EvadedCount;
            jsonDefensesAll.InvulnedCount = defenses.InvulnedCount;
            jsonDefensesAll.DamageBarrier = defenses.DamageBarrier;
            jsonDefensesAll.InterruptedCount = defenses.InterruptedCount;
            jsonDefensesAll.DownCount = defenses.DownCount;
            jsonDefensesAll.DownDuration = defenses.DownDuration;
            jsonDefensesAll.DeadCount = defenses.DeadCount;
            jsonDefensesAll.DeadDuration = defenses.DeadDuration;
            jsonDefensesAll.DcCount = defenses.DcCount;
            jsonDefensesAll.DcDuration = defenses.DcDuration;
            return jsonDefensesAll;
        }


        public static JsonDPS BuildJsonDPS(FinalDPS stats)
        {
            var jsonDPS = new JsonDPS
            {
                Dps = stats.Dps,
                Damage = stats.Damage,
                CondiDps = stats.CondiDps,
                CondiDamage = stats.CondiDamage,
                PowerDps = stats.PowerDps,
                PowerDamage = stats.PowerDamage,
                BreakbarDamage = stats.BreakbarDamage,

                ActorDps = stats.ActorDps,
                ActorDamage = stats.ActorDamage,
                ActorCondiDps = stats.ActorCondiDps,
                ActorCondiDamage = stats.ActorCondiDamage,
                ActorPowerDps = stats.ActorPowerDps,
                ActorPowerDamage = stats.ActorPowerDamage,
                ActorBreakbarDamage = stats.ActorBreakbarDamage
            };

            return jsonDPS;
        }

        private static void FillJsonGamePlayStats(JsonGameplayStats jsonGameplayStats, FinalOffensiveStats stats)
        {
            jsonGameplayStats.TotalDamageCount = stats.TotalDamageCount;
            jsonGameplayStats.DirectDamageCount = stats.DirectDamageCount;
            jsonGameplayStats.ConnectedDirectDamageCount = stats.ConnectedDirectDamageCount;
            jsonGameplayStats.ConnectedDamageCount = stats.ConnectedDamageCount;
            jsonGameplayStats.CritableDirectDamageCount = stats.CritableDirectDamageCount;
            jsonGameplayStats.CriticalRate = stats.CriticalCount;
            jsonGameplayStats.CriticalDmg = stats.CriticalDmg;
            jsonGameplayStats.FlankingRate = stats.FlankingCount;
            jsonGameplayStats.GlanceRate = stats.GlanceCount;
            jsonGameplayStats.AgainstMovingRate = stats.AgainstMovingCount;
            jsonGameplayStats.Missed = stats.Missed;
            jsonGameplayStats.Blocked = stats.Blocked;
            jsonGameplayStats.Evaded = stats.Evaded;
            jsonGameplayStats.Interrupts = stats.Interrupts;
            jsonGameplayStats.Invulned = stats.Invulned;
            jsonGameplayStats.Killed = stats.Killed;
            jsonGameplayStats.Downed = stats.Downed;
        }

        public static JsonGameplayStats BuildJsonGameplayStats(FinalOffensiveStats stats)
        {
            var jsonGameplayStats = new JsonGameplayStats();
            FillJsonGamePlayStats(jsonGameplayStats, stats);
            return jsonGameplayStats;
        }

        public static JsonGameplayStatsAll BuildJsonGameplayStatsAll(FinalGameplayStats stats, FinalOffensiveStats offStats)
        {
            var jsonGameplayStatsAll = new JsonGameplayStatsAll
            {
                Wasted = stats.Wasted,
                TimeWasted = stats.TimeWasted,
                Saved = stats.Saved,
                TimeSaved = stats.TimeSaved,
                StackDist = stats.StackDist,
                DistToCom = stats.DistToCom,
                AvgBoons = stats.AvgBoons,
                AvgActiveBoons = stats.AvgActiveBoons,
                AvgConditions = stats.AvgConditions,
                AvgActiveConditions = stats.AvgActiveConditions,
                SwapCount = stats.SwapCount,
                SkillCastUptime = stats.SkillCastUptime,
                SkillCastUptimeNoAA = stats.SkillCastUptimeNoAA,
            };
            FillJsonGamePlayStats(jsonGameplayStatsAll, offStats);
            return jsonGameplayStatsAll;
        }


        public static JsonPlayerSupport BuildJsonPlayerSupport(FinalToPlayersSupport stats)
        {
            var jsonPlayerSupport = new JsonPlayerSupport
            {
                Resurrects = stats.Resurrects,
                ResurrectTime = stats.ResurrectTime,
                CondiCleanse = stats.CondiCleanse,
                CondiCleanseTime = stats.CondiCleanseTime,
                CondiCleanseSelf = stats.CondiCleanseSelf,
                CondiCleanseTimeSelf = stats.CondiCleanseTimeSelf,
                BoonStrips = stats.BoonStrips,
                BoonStripsTime = stats.BoonStripsTime
            };
            return jsonPlayerSupport;
        }
    }
}
