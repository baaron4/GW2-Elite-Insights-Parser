using GW2EIEvtcParser.EIData;
using static GW2EIJSON.JsonStatistics;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities;

/// <summary>
/// Class representing general statistics
/// </summary>
internal static class JsonStatisticsBuilder
{
    public static JsonDefensesAll BuildJsonDefensesAll(DefenseAllStatistics defStats)
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
            ConditionCleansesTime = defStats.ConditionCleansesTime,
            ReceivedCrowdControl = defStats.ReceivedCrowdControl,
            ReceivedCrowdControlDuration = defStats.ReceivedCrowdControlDuration
        };
        return jsonDefensesAll;
    }


    public static JsonDPS BuildJsonDPS(DamageStatistics dpsStats)
    {
        var jsonDPS = new JsonDPS
        {
            Dps = dpsStats.DPS,
            Damage = dpsStats.Damage,
            CondiDps = dpsStats.ConditionDPS,
            CondiDamage = dpsStats.ConditionDamage,
            PowerDps = dpsStats.PowerDPS,
            PowerDamage = dpsStats.PowerDamage,
            BreakbarDamage = dpsStats.BreakbarDamage,

            ActorDps = dpsStats.ActorDPS,
            ActorDamage = dpsStats.ActorDamage,
            ActorCondiDps = dpsStats.ActorConditionDPS,
            ActorCondiDamage = dpsStats.ActorConditionDamage,
            ActorPowerDps = dpsStats.ActorPowerDPS,
            ActorPowerDamage = dpsStats.ActorPowerDamage,
            ActorBreakbarDamage = dpsStats.ActorBreakbarDamage
        };

        return jsonDPS;
    }

    private static void FillJsonGamePlayStats(JsonGameplayStats jsonGameplayStats, OffensiveStatistics offStats)
    {
        jsonGameplayStats.TotalDamageCount = offStats.TotalDamageEventCount;
        jsonGameplayStats.TotalDmg = offStats.TotalDamageEventDamage;
        jsonGameplayStats.DirectDamageCount = offStats.DirectDamageEventCount;
        jsonGameplayStats.DirectDmg = offStats.DirectDamageEventDamage;
        jsonGameplayStats.ConnectedDirectDamageCount = offStats.ConnectedDirectDamageCount;
        jsonGameplayStats.ConnectedDirectDmg = offStats.ConnectedDirectDmg;
        jsonGameplayStats.ConnectedDamageCount = offStats.DamageCount;
        jsonGameplayStats.ConnectedDmg = offStats.Damage;
        jsonGameplayStats.CritableDirectDamageCount = offStats.CritableDirectDamageCount;
        jsonGameplayStats.CriticalRate = offStats.CriticalCount;
        jsonGameplayStats.CriticalDmg = offStats.CriticalDamage;
        jsonGameplayStats.FlankingRate = offStats.FlankingCount;
        jsonGameplayStats.GlanceRate = offStats.GlancingCount;
        jsonGameplayStats.AgainstMovingRate = offStats.AgainstMovingCount;
        jsonGameplayStats.Missed = offStats.MissedCount;
        jsonGameplayStats.Blocked = offStats.BlockedCount;
        jsonGameplayStats.Evaded = offStats.EvadedCount;
        jsonGameplayStats.Interrupts = offStats.InterruptCount;
        jsonGameplayStats.Invulned = offStats.InvulnedCount;
        jsonGameplayStats.Killed = offStats.KilledCount;
        jsonGameplayStats.Downed = offStats.DownedCount;
        jsonGameplayStats.DownContribution = offStats.DownContribution;
        jsonGameplayStats.ConnectedConditionCount = offStats.ConditionCount;
        jsonGameplayStats.ConnectedConditionAbove90HPCount = offStats.ConditionAbove90HPCount;
        jsonGameplayStats.ConnectedPowerAbove90HPCount = offStats.PowerAbove90HPCount;
        jsonGameplayStats.ConnectedPowerCount = offStats.PowerCount;
        jsonGameplayStats.AgainstDownedCount = offStats.AgainstDownedCount;
        jsonGameplayStats.AgainstDownedDamage = offStats.AgainstDownedDamage;
        jsonGameplayStats.AppliedCrowdControl = offStats.AppliedCrowdControl;
        jsonGameplayStats.AppliedCrowdControlDuration = offStats.AppliedCrowdControlDuration;
    }

    public static JsonGameplayStats BuildJsonGameplayStats(OffensiveStatistics offStats)
    {
        var jsonGameplayStats = new JsonGameplayStats();
        FillJsonGamePlayStats(jsonGameplayStats, offStats);
        return jsonGameplayStats;
    }

    public static JsonGameplayStatsAll BuildJsonGameplayStatsAll(GameplayStatistics gameStats, OffensiveStatistics offStats)
    {
        var jsonGameplayStatsAll = new JsonGameplayStatsAll
        {
            Wasted = gameStats.SkillAnimationInterruptedCount,
            TimeWasted = gameStats.SkillAnimationInterruptedDuration,
            Saved = gameStats.SkillAnimationAfterCastInterruptedCount,
            TimeSaved = gameStats.SkillAnimationAfterCastInterruptedDuration,
            StackDist = gameStats.DistanceToCenterOfSquad,
            DistToCom = gameStats.DistanceToCommander,
            AvgBoons = gameStats.AverageBoons,
            AvgActiveBoons = gameStats.AverageActiveBoons,
            AvgConditions = gameStats.AverageConditions,
            AvgActiveConditions = gameStats.AverageActiveConditions,
            SwapCount = gameStats.WeaponSwapCount,
            SkillCastUptime = gameStats.SkillCastUptime,
            SkillCastUptimeNoAA = gameStats.SkillCastUptimeNoAutoAttack,
        };
        FillJsonGamePlayStats(jsonGameplayStatsAll, offStats);
        return jsonGameplayStatsAll;
    }


    public static JsonPlayerSupport BuildJsonAllySupport(SupportToAllyStatistics playerToPlayerStats)
    {
        var jsonPlayerSupport = new JsonPlayerSupport
        {
            Resurrects = playerToPlayerStats.ResurrectCount,
            ResurrectTime = playerToPlayerStats.ResurrectTime,
            CondiCleanse = playerToPlayerStats.ConditionCleanseCount,
            CondiCleanseTime = playerToPlayerStats.ConditionCleanseTime,
            CondiCleanseSelf = playerToPlayerStats.ConditionCleanseSelfCount,
            CondiCleanseTimeSelf = playerToPlayerStats.ConditionCleanseTimeSelf,
            BoonStrips = playerToPlayerStats.BoonStripCount,
            BoonStripsTime = playerToPlayerStats.BoonStripTime,
            StunBreak = playerToPlayerStats.StunBreakCount,
            RemovedStunDuration = playerToPlayerStats.RemovedStunDuration
        };
        return jsonPlayerSupport;
    }
}
