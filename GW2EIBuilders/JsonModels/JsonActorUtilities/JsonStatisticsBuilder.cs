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
            DamageTakenCount = defStats.DamageTakenCount,
            DamageBarrier = defStats.DamageBarrier,
            DamageBarrierCount = defStats.DamageBarrierCount,
            ConditionDamageTaken = defStats.ConditionDamageTaken,
            ConditionDamageTakenCount = defStats.ConditionDamageTakenCount,
            PowerDamageTaken = defStats.PowerDamageTaken,
            PowerDamageTakenCount = defStats.PowerDamageTakenCount,
            LifeLeechDamageTaken = defStats.LifeLeechDamageTaken,
            LifeLeechDamageTakenCount = defStats.LifeLeechDamageTakenCount,
            StrikeDamageTaken = defStats.StrikeDamageTaken,
            StrikeDamageTakenCount = defStats.StrikeDamageTakenCount,
            DownedDamageTaken = defStats.DownedDamageTaken,
            DownedDamageTakenCount = defStats.DownedDamageTakenCount,
            BreakbarDamageTaken = defStats.BreakbarDamageTaken,
            BreakbarDamageTakenCount = defStats.BreakbarDamageTakenCount,

            BlockedCount = defStats.BlockedCount,
            DodgeCount = defStats.DodgeCount,
            MissedCount = defStats.MissedCount,
            EvadedCount = defStats.EvadedCount,
            InvulnedCount = defStats.InvulnedCount,
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

        jsonGameplayStats.ConnectedDamageCount = offStats.DamageCount;
        jsonGameplayStats.ConnectedDmg = offStats.Damage;

        jsonGameplayStats.ConnectedDirectDamageCount = offStats.DirectDamageCount;
        jsonGameplayStats.ConnectedDirectDmg = offStats.DirectDamage;

        jsonGameplayStats.ConnectedPowerCount = offStats.PowerDamageCount;
        jsonGameplayStats.ConnectedPowerDamage = offStats.PowerDamage;
        jsonGameplayStats.ConnectedPowerAbove90HPCount = offStats.PowerDamageAbove90HPCount;
        jsonGameplayStats.ConnectedPowerAbove90HPDamage = offStats.PowerDamageAbove90HP;

        jsonGameplayStats.ConnectedLifeLeechCount = offStats.LifeLeechDamageCount;
        jsonGameplayStats.ConnectedLifeLeechDamage = offStats.LifeLeechDamage;

        jsonGameplayStats.ConnectedConditionCount = offStats.ConditionDamageCount;
        jsonGameplayStats.ConnectedConditionDamage = offStats.ConditionDamage;
        jsonGameplayStats.ConnectedConditionAbove90HPCount = offStats.ConditionDamageAbove90HPCount;
        jsonGameplayStats.ConnectedConditionAbove90HPDamage = offStats.ConditionDamageAbove90HP;

        jsonGameplayStats.CritableDirectDamageCount = offStats.CritableDirectDamageCount;
        jsonGameplayStats.CriticalRate = offStats.CriticalDamageCount;
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

        jsonGameplayStats.AgainstDownedCount = offStats.AgainstDownedDamageCount;
        jsonGameplayStats.AgainstDownedDamage = offStats.AgainstDownedDamage;

        jsonGameplayStats.DownContribution = offStats.DownContribution;
        jsonGameplayStats.AppliedCrowdControlDownContribution = offStats.AppliedCrowdControlDownContribution;
        jsonGameplayStats.AppliedCrowdControlDurationDownContribution = offStats.AppliedCrowdControlDurationDownContribution;

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
