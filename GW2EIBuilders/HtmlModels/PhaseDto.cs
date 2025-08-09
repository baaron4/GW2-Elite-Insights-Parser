using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels;

using GameplayStatDataItem = List<double>;
/*(
    double timeWasted, // 0
    int wasted, // 1
    double timeSaved, // 2
    int saved, // 3
    int swap, // 4
    double distToStack, // 5
    double distToCom, // 6
    double castUptime, // 7
    double castUptimeNoAA // 8
);
*/

using OffensiveStatDataItem = List<double>;
/*(
    int directDamageCount, // 0
    int critableDirectDamageCount,
    int criticalCount,
    int criticalDamage,
    int flanking,
    int glance, // 5
    int missed,
    int interrupts,
    int invulned,
    int evaded,
    int blocked, // 10
    int connectedDirectDamageCount,
    int killed,
    int downed,
    int againstMoving,
    int connectedDamageCount, // 15
    int totalDamageCount,
    int downContribution,
    int connectedDamage,
    int connectedDirectDamage,
    int connectedPowerCount, // 20
    int connectedPowerAbove90HPCount,
    int connectedConditionCount,
    int connectedConditionAbove90HPCount,
    int againstDownedCount,
    int againstDownedDamage, // 25
    int totalDamage,
    int appliedCrowdControl,
    double appliedCrowdControlDuration
);*/

using DPSStatDataItem = List<double>;
/*(
    int damage, // 0
    int powerDamage,
    int conditionDamage,
    double breakbarDamage
);*/

using DefensiveStatDataItem = object[];
/*(
    int damageTaken, // 0
    int damageBarrier,
    int missedCount,
    int interruptCount,
    int invulnedCount,
    int evadedCound, // 5
    int blockedCount,
    int dodgeCount,
    int conditionCleanses,
    double conditionCleansesTime,
    int boonStrips, // 10,
    double boonStripsTime,
    int downCount,
    string downTooltip,
    int deadCount,
    string deadTooltip, // 15
    int downedDamageTaken,
    int receivedCrowdControl,
    double receivedCrowdControlDuration
);*/

using SupportStatDataItem = List<double>;
/*(
    int conditionCleanse, // 0
    double conditionCleanseTime,
    int conditionCleanseSelf,
    double conditionCleanseSelfTime,
    int boonStrips,
    double boonStripsTime, // 5
    int ressurects,
    double ressurectsTime,
    int stunBreak,
    double removedStunDuration
);*/

//TODO(Rennorb) @perf: IF we wanted more performance we could try to just get rid of this json data step all together.
// It should be doable to just merge it with existing structures, as to not need to copy everything..
// If this is reasonably possible it should give time savings around 20-30%
internal class PhaseDto
{
    public string? Name;
    public long Duration;
    public double Start;
    public double End;
    public int Type;
    public string? NameNoMode;
    public string? Icon;
    public string? Mode;
    public string? EncounterDuration;
    public string? StartStatus;
    public bool? Success;

    public List<int> Targets;
    public List<int> TargetPriorities;
    public bool BreakbarPhase;

    public List<DPSStatDataItem> DpsStats;
    public List<List<DPSStatDataItem>> DpsStatsTargets;
    public List<List<OffensiveStatDataItem>> OffensiveStatsTargets;
    public List<OffensiveStatDataItem> OffensiveStats;
    public List<GameplayStatDataItem> GameplayStats;
    public List<DefensiveStatDataItem> DefStats;
    public List<SupportStatDataItem> SupportStats;

    public BuffsContainerDto BuffsStatContainer;
    public BuffVolumesContainerDto BuffVolumesStatContainer;

    public List<DamageModData> DmgModifiersCommon;
    public List<DamageModData> DmgModifiersItem;
    public List<DamageModData> DmgModifiersPers;


    public List<DamageModData> DmgIncModifiersCommon;
    public List<DamageModData> DmgIncModifiersItem;
    public List<DamageModData> DmgIncModifiersPers;


    public List<List<int[]>> MechanicStats;
    public List<List<int[]>> EnemyMechanicStats;
    public List<long> PlayerActiveTimes;

    public List<double>? MarkupLines;
    public List<AreaLabelDto>? MarkupAreas;
    public List<int>? SubPhases;

    public PhaseDto(PhaseData phase, IReadOnlyList<PhaseData> phases, ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> persBuffDict,
        IReadOnlyList<OutgoingDamageModifier> commonOutDamageModifiers, IReadOnlyList<OutgoingDamageModifier> itemOutDamageModifiers, IReadOnlyDictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> persOutDamageModDict,
        IReadOnlyList<IncomingDamageModifier> commonIncDamageModifiers, IReadOnlyList<IncomingDamageModifier> itemIncDamageModifiers, IReadOnlyDictionary<Spec, IReadOnlyList<IncomingDamageModifier>> persIncDamageModDict)
    {
        Name          = phase.Name;
        Duration      = phase.DurationInMS;
        Start         = phase.Start / 1000.0;
        End           = phase.End / 1000.0;
        BreakbarPhase = phase.BreakbarPhase;
        Type = (int)phase.Type;
        if (phase is PhaseDataWithMetaData phaseWithMetaData)
        {
            NameNoMode = phaseWithMetaData.NameNoMode;
            Icon = phaseWithMetaData.Icon;
            Success = phaseWithMetaData.Success;
            EncounterDuration = phaseWithMetaData.DurationString;
            switch (phaseWithMetaData.Mode)
            {
                case FightData.EncounterMode.Unknown:
                    Mode = "Unknown";
                    break;
                case FightData.EncounterMode.Story:
                    Mode = "Story Mode";
                    break;
                case FightData.EncounterMode.Normal:
                    // TODO support emboldened properly
                    Mode = log.FightData.Logic.GetInstanceBuffs(log).Any(x => x.buff.ID == SkillIDs.Emboldened) ? "Emboldened Normal Mode" : "Normal Mode";
                    break;
                case FightData.EncounterMode.CM:
                case FightData.EncounterMode.CMNoName:
                    Mode = "Challenge Mode";
                    break;
                case FightData.EncounterMode.LegendaryCM:
                    Mode = "Legendary Challenge Mode";
                    break;
                default:
                    break;
            }
            switch (phaseWithMetaData.StartStatus)
            {
                case FightData.EncounterStartStatus.Normal:
                    break;
                case FightData.EncounterStartStatus.NotSet:
                    break;
                case FightData.EncounterStartStatus.Late:
                    StartStatus = "Late Start";
                    break;
                case FightData.EncounterStartStatus.NoPreEvent:
                    StartStatus = "No Pre-Event";
                    break;
                default:
                    break;
            }
        }

        var allTargets = phase.Targets;
        Targets          = new(allTargets.Count);
        TargetPriorities = new(allTargets.Count);
        foreach (var pair in allTargets)
        {
            var target = pair.Key;
            Targets.Add(log.FightData.Logic.Targets.IndexOf(target));
            TargetPriorities.Add((int)pair.Value.Priority);
        }

        PlayerActiveTimes = new(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            PlayerActiveTimes.Add(actor.GetActiveDuration(log, phase.Start, phase.End));
        }

        // add phase markup
        
        if (!BreakbarPhase)
        {
            MarkupLines = new(phases.Count);
            MarkupAreas = new(phases.Count);
            for (int j = 1; j < phases.Count; j++)
            {
                PhaseData curPhase = phases[j];
                if (curPhase.Start < phase.Start || curPhase.End > phase.End ||
                    (curPhase == phase) || !curPhase.CanBeASubPhaseOf(phase))
                {
                    continue;
                }

                SubPhases ??= new List<int>(phases.Count);
                SubPhases.Add(j);

                long start = curPhase.Start - phase.Start;
                long end = curPhase.End - phase.Start;
                if (curPhase.DrawStart)
                {
                    MarkupLines.Add(start / 1000.0);
                }

                if (curPhase.DrawEnd)
                {
                    MarkupLines.Add(end / 1000.0);
                }

                var phaseArea = new AreaLabelDto
                {
                    Start = start / 1000.0,
                    End = end / 1000.0,
                    Label = curPhase.DrawLabel ? curPhase.Name : null,
                    Highlight = curPhase.DrawArea
                };

                MarkupAreas.Add(phaseArea);
            }
        }

        if (MarkupAreas?.Count == 0)
        {
            MarkupAreas = null;
        }

        if (MarkupLines?.Count == 0)
        {
            MarkupLines = null;
        }

        BuffsStatContainer       = new BuffsContainerDto(phase, log, persBuffDict);
        BuffVolumesStatContainer = new BuffVolumesContainerDto(phase, log, persBuffDict);
        
        DpsStats              = BuildDPSData(log, phase);
        DpsStatsTargets       = BuildDPSTargetsData(log, phase);
        OffensiveStatsTargets = BuildOffensiveStatsTargetsData(log, phase);
        OffensiveStats        = BuildOffensiveStatsData(log, phase);
        GameplayStats         = BuildGameplayStatsData(log, phase);
        DefStats              = BuildDefenseData(log, phase);
        SupportStats          = BuildSupportData(log, phase);
        
        DmgModifiersCommon    = DamageModData.BuildOutgoingDmgModifiersData(log, phase, commonOutDamageModifiers);
        DmgModifiersItem      = DamageModData.BuildOutgoingDmgModifiersData(log, phase, itemOutDamageModifiers);
        DmgModifiersPers      = DamageModData.BuildPersonalOutgoingDmgModifiersData(log, phase, persOutDamageModDict);
        DmgIncModifiersCommon = DamageModData.BuildIncomingDmgModifiersData(log, phase, commonIncDamageModifiers);
        DmgIncModifiersItem   = DamageModData.BuildIncomingDmgModifiersData(log, phase, itemIncDamageModifiers);
        DmgIncModifiersPers   = DamageModData.BuildPersonalIncomingDmgModifiersData(log, phase, persIncDamageModDict);
        MechanicStats         = MechanicDto.BuildPlayerMechanicData(log, phase);
        EnemyMechanicStats    = MechanicDto.BuildEnemyMechanicData(log, phase);
    }

    // helper methods

    private static GameplayStatDataItem GetGameplayStatData(GameplayStatistics stats)
    {
        return [
                stats.SkillAnimationInterruptedDuration,
                stats.SkillAnimationInterruptedCount,
                stats.SkillAnimationAfterCastInterruptedDuration,
                stats.SkillAnimationAfterCastInterruptedCount,
                stats.WeaponSwapCount,
                Math.Round(stats.DistanceToCenterOfSquad, 2),
                Math.Round(stats.DistanceToCommander, 2),
                stats.SkillCastUptime,
                stats.SkillCastUptimeNoAutoAttack
        ];
    }

    private static OffensiveStatDataItem GetOffensiveStatData(OffensiveStatistics stats)
    {
        return [
                stats.DirectDamageEventCount,
                stats.CritableDirectDamageCount,
                stats.CriticalDamageCount,
                stats.CriticalDamage,
                stats.FlankingCount,
                stats.GlancingCount,
                stats.MissedCount,
                stats.InterruptCount,
                stats.InvulnedCount,
                stats.EvadedCount,
                stats.BlockedCount,
                stats.DirectDamageCount,
                stats.KilledCount,
                stats.DownedCount,
                stats.AgainstMovingCount,
                stats.DamageCount,
                stats.TotalDamageEventCount,
                stats.DownContribution,
                stats.Damage,
                stats.DirectDamage,
                stats.PowerDamageCount,
                stats.PowerDamageAbove90HPCount,
                stats.ConditionDamageCount,
                stats.ConditionDamageAbove90HPCount,
                stats.AgainstDownedDamageCount,
                stats.AgainstDownedDamage,
                stats.TotalDamageEventDamage,
                stats.AppliedCrowdControl,
                stats.AppliedCrowdControlDuration
            ];
    }

    private static DPSStatDataItem GetDPSStatData(DamageStatistics dpsAll)
    {
        return [
                dpsAll.Damage,
                dpsAll.PowerDamage,
                dpsAll.ConditionDamage,
                dpsAll.BreakbarDamage
            ];
    }

    private static SupportStatDataItem GetSupportStatData(SupportStatistics support)
    {
        return [
                support.ConditionCleanseCount,
                support.ConditionCleanseTime,
                support.ConditionCleanseSelfCount,
                support.ConditionCleanseTimeSelf,
                support.BoonStripCount,
                support.BoonStripTime,
                support.ResurrectCount,
                support.ResurrectTime,
                support.StunBreakCount,
                support.RemovedStunDuration
        ];
    }

    private static DefensiveStatDataItem GetDefenseStatData(DefenseAllStatistics defenses, PhaseData phase)
    {
        int downCount = 0;
        string downTooltip = "0% Downed";
        if (defenses.DownCount > 0)
        {
            var downDuration = TimeSpan.FromMilliseconds(defenses.DownDuration);
            downCount = (defenses.DownCount);
            downTooltip = (downDuration.TotalSeconds + " seconds downed, " + Math.Round((downDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1) + "% Downed");
        }
        int deadCount = 0;
        string deadTooltip = "100% Alive";
        if (defenses.DeadCount > 0)
        {
            var deathDuration = TimeSpan.FromMilliseconds(defenses.DeadDuration);
            deadCount = (defenses.DeadCount);
            deadTooltip = (deathDuration.TotalSeconds + " seconds dead, " + (100.0 - Math.Round((deathDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1)) + "% Alive");
        }
        return [
                defenses.DamageTaken, 
                defenses.DamageBarrier,
                defenses.MissedCount,
                defenses.InterruptedCount,
                defenses.InvulnedCount,
                defenses.EvadedCount,
                defenses.BlockedCount,
                defenses.DodgeCount,
                defenses.ConditionCleanses,
                defenses.ConditionCleansesTime,
                defenses.BoonStrips,
                defenses.BoonStripsTime,
                downCount,
                downTooltip,
                deadCount,
                deadTooltip,
                defenses.DownedDamageTaken, 
                defenses.ReceivedCrowdControl,
                defenses.ReceivedCrowdControlDuration
            ];
    }
    public static List<DPSStatDataItem> BuildDPSData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<DPSStatDataItem>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            DamageStatistics dpsAll = actor.GetDamageStats(log, phase.Start, phase.End);
            list.Add(GetDPSStatData(dpsAll));
        }
        return list;
    }

    public static List<List<DPSStatDataItem>> BuildDPSTargetsData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<List<DPSStatDataItem>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            var playerData = new List<DPSStatDataItem>(phase.Targets.Count);

            foreach (SingleActor target in phase.Targets.Keys)
            {
                playerData.Add(GetDPSStatData(actor.GetDamageStats(target, log, phase.Start, phase.End)));
            }
            list.Add(playerData);
        }
        return list;
    }

    public static List<GameplayStatDataItem> BuildGameplayStatsData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<GameplayStatDataItem>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            GameplayStatistics stats = actor.GetGameplayStats(log, phase.Start, phase.End);
            list.Add(GetGameplayStatData(stats));
        }
        return list;
    }

    public static List<OffensiveStatDataItem> BuildOffensiveStatsData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<OffensiveStatDataItem>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            OffensiveStatistics stats = actor.GetOffensiveStats(null, log, phase.Start, phase.End);
            list.Add(GetOffensiveStatData(stats));
        }
        return list;
    }

    public static List<List<OffensiveStatDataItem>> BuildOffensiveStatsTargetsData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<List<OffensiveStatDataItem>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            var playerData = new List<OffensiveStatDataItem>(phase.Targets.Count);
            foreach (SingleActor target in phase.Targets.Keys)
            {
                OffensiveStatistics statsTarget = actor.GetOffensiveStats(target, log, phase.Start, phase.End);
                playerData.Add(GetOffensiveStatData(statsTarget));
            }
            list.Add(playerData);
        }
        return list;
    }

    public static List<DefensiveStatDataItem> BuildDefenseData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<DefensiveStatDataItem>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            DefenseAllStatistics defenses = actor.GetDefenseStats(log, phase.Start, phase.End);
            list.Add(GetDefenseStatData(defenses, phase));
        }

        return list;
    }

    public static List<SupportStatDataItem> BuildSupportData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<SupportStatDataItem>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            SupportStatistics support = actor.GetToAllySupportStats(log, phase.Start, phase.End);
            list.Add(GetSupportStatData(support));
        }
        return list;
    }
}
