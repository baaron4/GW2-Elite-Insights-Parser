using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;
using static GW2EIJSON.JsonStatistics;

namespace GW2EIBuilders.JsonModels
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
            var jsonDPS = new JsonDPS();
            jsonDPS.Dps = stats.Dps;
            jsonDPS.Damage = stats.Damage;
            jsonDPS.CondiDps = stats.CondiDps;
            jsonDPS.CondiDamage = stats.CondiDamage;
            jsonDPS.PowerDps = stats.PowerDps;
            jsonDPS.PowerDamage = stats.PowerDamage;
            jsonDPS.BreakbarDamage = stats.BreakbarDamage;

            jsonDPS.ActorDps = stats.ActorDps;
            jsonDPS.ActorDamage = stats.ActorDamage;
            jsonDPS.ActorCondiDps = stats.ActorCondiDps;
            jsonDPS.ActorCondiDamage = stats.ActorCondiDamage;
            jsonDPS.ActorPowerDps = stats.ActorPowerDps;
            jsonDPS.ActorPowerDamage = stats.ActorPowerDamage;
            jsonDPS.ActorBreakbarDamage = stats.ActorBreakbarDamage;

            return jsonDPS;
        }

        private static void FillJsonGamePlayStats(JsonGameplayStats jsonGameplayStats, FinalGameplayStats stats)
        {
            jsonGameplayStats.TotalDamageCount = stats.TotalDamageCount;
            jsonGameplayStats.DirectDamageCount = stats.DirectDamageCount;
            jsonGameplayStats.ConnectedDirectDamageCount = stats.ConnectedDirectDamageCount;
            jsonGameplayStats.CritableDirectDamageCount = stats.CritableDirectDamageCount;
            jsonGameplayStats.CriticalRate = stats.CriticalCount;
            jsonGameplayStats.CriticalDmg = stats.CriticalDmg;
            jsonGameplayStats.FlankingRate = stats.FlankingCount;
            jsonGameplayStats.GlanceRate = stats.GlanceCount;
            jsonGameplayStats.Missed = stats.Missed;
            jsonGameplayStats.Blocked = stats.Blocked;
            jsonGameplayStats.Evaded = stats.Evaded;
            jsonGameplayStats.Interrupts = stats.Interrupts;
            jsonGameplayStats.Invulned = stats.Invulned;
            jsonGameplayStats.Killed = stats.Killed;
            jsonGameplayStats.Downed = stats.Downed;
        }

        public static JsonGameplayStats BuildJsonGameplayStats(FinalGameplayStats stats)
        {
            var jsonGameplayStats = new JsonGameplayStats();
            FillJsonGamePlayStats(jsonGameplayStats, stats);
            return jsonGameplayStats;
        }

        public static JsonGameplayStatsAll BuildJsonGameplayStatsAll(FinalGameplayStatsAll stats)
        {
            var jsonGameplayStatsAll = new JsonGameplayStatsAll();
            FillJsonGamePlayStats(jsonGameplayStatsAll, stats);
            jsonGameplayStatsAll.Wasted = stats.Wasted;
            jsonGameplayStatsAll.TimeWasted = stats.TimeWasted;
            jsonGameplayStatsAll.Saved = stats.Saved;
            jsonGameplayStatsAll.TimeSaved = stats.TimeSaved;
            jsonGameplayStatsAll.StackDist = stats.StackDist;
            jsonGameplayStatsAll.DistToCom = stats.DistToCom;
            jsonGameplayStatsAll.AvgBoons = stats.AvgBoons;
            jsonGameplayStatsAll.AvgActiveBoons = stats.AvgActiveBoons;
            jsonGameplayStatsAll.AvgConditions = stats.AvgConditions;
            jsonGameplayStatsAll.AvgActiveConditions = stats.AvgActiveConditions;
            jsonGameplayStatsAll.SwapCount = stats.SwapCount;
            return jsonGameplayStatsAll;
        }


        public static JsonPlayerSupport BuildJsonPlayerSupport(FinalPlayerSupport stats)
        {
            var jsonPlayerSupport = new JsonPlayerSupport();
            jsonPlayerSupport.Resurrects = stats.Resurrects;
            jsonPlayerSupport.ResurrectTime = stats.ResurrectTime;
            jsonPlayerSupport.CondiCleanse = stats.CondiCleanse;
            jsonPlayerSupport.CondiCleanseTime = stats.CondiCleanseTime;
            jsonPlayerSupport.CondiCleanseSelf = stats.CondiCleanseSelf;
            jsonPlayerSupport.CondiCleanseTimeSelf = stats.CondiCleanseTimeSelf;
            jsonPlayerSupport.BoonStrips = stats.BoonStrips;
            jsonPlayerSupport.BoonStripsTime = stats.BoonStripsTime;
            return jsonPlayerSupport;
        }
    }
}
