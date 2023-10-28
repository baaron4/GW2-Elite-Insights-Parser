using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels
{

    internal class PhaseDto
    {
        public string Name { get; set; }
        public long Duration { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public List<int> Targets { get; set; } = new List<int>();
        public bool BreakbarPhase { get; set; }

        public List<List<object>> DpsStats { get; set; }
        public List<List<List<object>>> DpsStatsTargets { get; set; }
        public List<List<List<object>>> OffensiveStatsTargets { get; set; }
        public List<List<object>> OffensiveStats { get; set; }
        public List<List<object>> GameplayStats { get; set; }
        public List<List<object>> DefStats { get; set; }
        public List<List<object>> SupportStats { get; set; }
        // all
        public List<BuffData> BoonStats { get; set; }
        public List<List<BuffData>> BoonDictionaries { get; set; }
        public List<BuffData> BoonGenSelfStats { get; set; }
        public List<BuffData> BoonGenGroupStats { get; set; }
        public List<BuffData> BoonGenOGroupStats { get; set; }
        public List<BuffData> BoonGenSquadStats { get; set; }

        public List<BuffData> OffBuffStats { get; set; }
        public List<BuffData> OffBuffGenSelfStats { get; set; }
        public List<BuffData> OffBuffGenGroupStats { get; set; }
        public List<BuffData> OffBuffGenOGroupStats { get; set; }
        public List<BuffData> OffBuffGenSquadStats { get; set; }

        public List<BuffData> SupBuffStats { get; set; }
        public List<BuffData> SupBuffGenSelfStats { get; set; }
        public List<BuffData> SupBuffGenGroupStats { get; set; }
        public List<BuffData> SupBuffGenOGroupStats { get; set; }
        public List<BuffData> SupBuffGenSquadStats { get; set; }

        public List<BuffData> DefBuffStats { get; set; }
        public List<BuffData> DefBuffGenSelfStats { get; set; }
        public List<BuffData> DefBuffGenGroupStats { get; set; }
        public List<BuffData> DefBuffGenOGroupStats { get; set; }
        public List<BuffData> DefBuffGenSquadStats { get; set; }

        public List<BuffData> ConditionsStats { get; set; }
        public List<BuffData> PersBuffStats { get; set; }
        public List<BuffData> GearBuffStats { get; set; }
        public List<BuffData> NourishmentStats { get; set; }
        public List<BuffData> EnhancementStats { get; set; }
        public List<BuffData> OtherConsumableStats { get; set; }
        public List<BuffData> DebuffStats { get; set; }

        // active
        public List<BuffData> BoonActiveStats { get; set; }
        public List<List<BuffData>> BoonActiveDictionaries { get; set; }
        public List<BuffData> BoonGenActiveSelfStats { get; set; }
        public List<BuffData> BoonGenActiveGroupStats { get; set; }
        public List<BuffData> BoonGenActiveOGroupStats { get; set; }
        public List<BuffData> BoonGenActiveSquadStats { get; set; }

        public List<BuffData> OffBuffActiveStats { get; set; }
        public List<BuffData> OffBuffGenActiveSelfStats { get; set; }
        public List<BuffData> OffBuffGenActiveGroupStats { get; set; }
        public List<BuffData> OffBuffGenActiveOGroupStats { get; set; }
        public List<BuffData> OffBuffGenActiveSquadStats { get; set; }

        public List<BuffData> SupBuffActiveStats { get; set; }
        public List<BuffData> SupBuffGenActiveSelfStats { get; set; }
        public List<BuffData> SupBuffGenActiveGroupStats { get; set; }
        public List<BuffData> SupBuffGenActiveOGroupStats { get; set; }
        public List<BuffData> SupBuffGenActiveSquadStats { get; set; }

        public List<BuffData> DefBuffActiveStats { get; set; }
        public List<BuffData> DefBuffGenActiveSelfStats { get; set; }
        public List<BuffData> DefBuffGenActiveGroupStats { get; set; }
        public List<BuffData> DefBuffGenActiveOGroupStats { get; set; }
        public List<BuffData> DefBuffGenActiveSquadStats { get; set; }

        public List<BuffData> ConditionsActiveStats { get; set; }
        public List<BuffData> PersBuffActiveStats { get; set; }
        public List<BuffData> GearBuffActiveStats { get; set; }
        public List<BuffData> DebuffActiveStats { get; set; }

        public List<DamageModData> DmgModifiersCommon { get; set; }
        public List<DamageModData> DmgModifiersItem { get; set; }
        public List<DamageModData> DmgModifiersPers { get; set; }

        public List<List<BuffData>> TargetsCondiStats { get; set; }
        public List<BuffData> TargetsCondiTotals { get; set; }
        public List<BuffData> TargetsBoonTotals { get; set; }

        public List<List<int[]>> MechanicStats { get; set; }
        public List<List<int[]>> EnemyMechanicStats { get; set; }
        public List<long> PlayerActiveTimes { get; set; }

        public List<double> MarkupLines { get; set; }
        public List<AreaLabelDto> MarkupAreas { get; set; }
        public List<int> SubPhases { get; set; }

        public PhaseDto(PhaseData phase, IReadOnlyList<PhaseData> phases, ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> persBuffDict, IReadOnlyList<DamageModifier> commonDamageModifiers, IReadOnlyList<DamageModifier> itemDamageModifiers, IReadOnlyDictionary<Spec, IReadOnlyList<DamageModifier>> persDamageModDict)
        {
            Name = phase.Name;
            Duration = phase.DurationInMS;
            Start = phase.Start / 1000.0;
            End = phase.End / 1000.0;
            BreakbarPhase = phase.BreakbarPhase;
            foreach (AbstractSingleActor target in phase.Targets)
            {
                Targets.Add(log.FightData.Logic.Targets.IndexOf(target));
            }
            PlayerActiveTimes = new List<long>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                PlayerActiveTimes.Add(actor.GetActiveDuration(log, phase.Start, phase.End));
            }
            // add phase markup
            MarkupLines = new List<double>();
            MarkupAreas = new List<AreaLabelDto>();
            if (!BreakbarPhase)
            {
                for (int j = 1; j < phases.Count; j++)
                {
                    PhaseData curPhase = phases[j];
                    if (curPhase.Start < phase.Start || curPhase.End > phase.End ||
                        (curPhase.Start == phase.Start && curPhase.End == phase.End) || !curPhase.CanBeSubPhase)
                    {
                        continue;
                    }
                    if (SubPhases == null)
                    {
                        SubPhases = new List<int>();
                    }
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
            if (MarkupAreas.Count == 0)
            {
                MarkupAreas = null;
            }

            if (MarkupLines.Count == 0)
            {
                MarkupLines = null;
            }
            StatisticsHelper statistics = log.StatisticsHelper;

            DpsStats = BuildDPSData(log, phase);
            DpsStatsTargets = BuildDPSTargetsData(log, phase);
            OffensiveStatsTargets = BuildOffensiveStatsTargetsData(log, phase);
            OffensiveStats = BuildOffensiveStatsData(log, phase);
            GameplayStats = BuildGameplayStatsData(log, phase);
            DefStats = BuildDefenseData(log, phase);
            SupportStats = BuildSupportData(log, phase);
            //
            BoonStats = BuffData.BuildBuffUptimeData(log, statistics.PresentBoons, phase);
            BoonDictionaries = BuffData.BuildBuffDictionariesData(log, statistics.PresentBoons, phase);
            OffBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentOffbuffs, phase);
            SupBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentSupbuffs, phase);
            DefBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentDefbuffs, phase);
            PersBuffStats = BuffData.BuildPersonalBuffUptimeData(log, persBuffDict, phase);
            GearBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentGearbuffs, phase);
            NourishmentStats = BuffData.BuildBuffUptimeData(log, statistics.PresentNourishements, phase);
            EnhancementStats = BuffData.BuildBuffUptimeData(log, statistics.PresentEnhancements, phase);
            OtherConsumableStats = BuffData.BuildBuffUptimeData(log, statistics.PresentOtherConsumables, phase);
            DebuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentDebuffs, phase);
            ConditionsStats = BuffData.BuildBuffUptimeData(log, statistics.PresentConditions, phase);
            BoonGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Self);
            BoonGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Group);
            BoonGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.OffGroup);
            BoonGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Squad);
            OffBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Self);
            OffBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Group);
            OffBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.OffGroup);
            OffBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Squad);
            SupBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Self);
            SupBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Group);
            SupBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.OffGroup);
            SupBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Squad);
            DefBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Self);
            DefBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Group);
            DefBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.OffGroup);
            DefBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Squad);
            //
            BoonActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentBoons, phase);
            BoonActiveDictionaries = BuffData.BuildActiveBuffDictionariesData(log, statistics.PresentBoons, phase);
            OffBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentOffbuffs, phase);
            SupBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentSupbuffs, phase);
            DefBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDefbuffs, phase);
            PersBuffActiveStats = BuffData.BuildActivePersonalBuffUptimeData(log, persBuffDict, phase);
            GearBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentGearbuffs, phase);
            DebuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDebuffs, phase);
            ConditionsActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentConditions, phase);
            BoonGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Self);
            BoonGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Group);
            BoonGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.OffGroup);
            BoonGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Squad);
            OffBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Self);
            OffBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Group);
            OffBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.OffGroup);
            OffBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Squad);
            SupBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Self);
            SupBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Group);
            SupBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.OffGroup);
            SupBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Squad);
            DefBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Self);
            DefBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Group);
            DefBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.OffGroup);
            DefBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Squad);
            //
            DmgModifiersCommon = DamageModData.BuildDmgModifiersData(log, phase, commonDamageModifiers);
            DmgModifiersItem = DamageModData.BuildDmgModifiersData(log, phase, itemDamageModifiers);
            DmgModifiersPers = DamageModData.BuildPersonalDmgModifiersData(log, phase, persDamageModDict);
            TargetsCondiStats = new List<List<BuffData>>();
            TargetsCondiTotals = new List<BuffData>();
            TargetsBoonTotals = new List<BuffData>();
            MechanicStats = MechanicDto.BuildPlayerMechanicData(log, phase);
            EnemyMechanicStats = MechanicDto.BuildEnemyMechanicData(log, phase);

            foreach (AbstractSingleActor target in phase.Targets)
            {
                TargetsCondiStats.Add(BuffData.BuildTargetCondiData(log, phase, target));
                TargetsCondiTotals.Add(BuffData.BuildTargetCondiUptimeData(log, phase, target));
                TargetsBoonTotals.Add(BuffData.BuildTargetBoonData(log, phase, target));
            }
        }

        private static bool HasBoons(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor target)
        {
            IReadOnlyDictionary<long, FinalActorBuffs> conditions = target.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
            foreach (Buff boon in log.StatisticsHelper.PresentBoons)
            {
                if (conditions.TryGetValue(boon.ID, out FinalActorBuffs uptime))
                {
                    if (uptime.Uptime > 0.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        // helper methods

        private static List<object> GetGameplayStatData(FinalGameplayStats stats)
        {
            var data = new List<object>
                {
                    // commons
                    stats.TimeWasted, // 0
                    stats.Wasted, // 1

                    stats.TimeSaved, // 2
                    stats.Saved, // 3

                    stats.SwapCount, // 4
                    Math.Round(stats.StackDist, 2), // 5
                    Math.Round(stats.DistToCom, 2), // 6
                    stats.SkillCastUptime, // 7
                    stats.SkillCastUptimeNoAA, // 8
                };
            return data;
        }

        private static List<object> GetOffensiveStatData(FinalOffensiveStats stats)
        {
            var data = new List<object>
                {
                    stats.DirectDamageCount, // 0
                    stats.CritableDirectDamageCount, // 1
                    stats.CriticalCount, // 2
                    stats.CriticalDmg, // 3

                    stats.FlankingCount, // 4

                    stats.GlanceCount, // 5

                    stats.Missed,// 6
                    stats.Interrupts, // 7
                    stats.Invulned, // 8
                    stats.Evaded,// 9
                    stats.Blocked,// 10
                    stats.ConnectedDirectDamageCount, // 11
                    stats.Killed, // 12
                    stats.Downed, // 13
                    stats.AgainstMovingCount, // 14
                    stats.ConnectedDamageCount, // 15
                    stats.TotalDamageCount, // 16
                    stats.DownContribution, // 17
                    stats.ConnectedDmg, // 18
                    stats.ConnectedDirectDmg, // 19

                    stats.ConnectedPowerCount, // 20
                    stats.ConnectedPowerAbove90HPCount, // 21
                    stats.ConnectedConditionCount, // 22
                    stats.ConnectedConditionAbove90HPCount, // 23
                    stats.AgainstDownedCount, // 24
                    stats.AgainstDownedDamage, // 25
                    stats.TotalDmg, // 26
                };
            return data;
        }

        private static List<object> GetDPSStatData(FinalDPS dpsAll)
        {
            var data = new List<object>
                {
                    dpsAll.Damage,
                    dpsAll.PowerDamage,
                    dpsAll.CondiDamage,
                    dpsAll.BreakbarDamage,
                };
            return data;
        }

        private static List<object> GetSupportStatData(FinalToPlayersSupport support)
        {
            var data = new List<object>()
                {
                    support.CondiCleanse,
                    support.CondiCleanseTime,
                    support.CondiCleanseSelf,
                    support.CondiCleanseTimeSelf,
                    support.BoonStrips,
                    support.BoonStripsTime,
                    support.Resurrects,
                    support.ResurrectTime
                };
            return data;
        }

        private static List<object> GetDefenseStatData(FinalDefensesAll defenses, PhaseData phase)
        {
            int downCount = 0;
            string downTooltip = "0% Downed";
            if (defenses.DownDuration > 0)
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
            var data = new List<object>
                {
                    defenses.DamageTaken, // 0
                    defenses.DamageBarrier,// 1
                    defenses.MissedCount,// 2
                    defenses.InterruptedCount,// 3
                    defenses.InvulnedCount,// 4
                    defenses.EvadedCount,// 5
                    defenses.BlockedCount,// 6
                    defenses.DodgeCount,// 7
                    defenses.ConditionCleanses,// 8
                    defenses.ConditionCleansesTime,// 9
                    defenses.BoonStrips,// 10
                    defenses.BoonStripsTime,// 11
                    downCount, // 12
                    downTooltip,// 13
                    deadCount,// 14
                    deadTooltip,// 15
                    defenses.DownedDamageTaken // 16
                };
            return data;
        }
        public static List<List<object>> BuildDPSData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>(log.Friendlies.Count);
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalDPS dpsAll = actor.GetDPSStats(log, phase.Start, phase.End);
                list.Add(GetDPSStatData(dpsAll));
            }
            return list;
        }

        public static List<List<List<object>>> BuildDPSTargetsData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<List<object>>>(log.Friendlies.Count);

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                var playerData = new List<List<object>>();

                foreach (AbstractSingleActor target in phase.Targets)
                {
                    playerData.Add(GetDPSStatData(actor.GetDPSStats(target, log, phase.Start, phase.End)));
                }
                list.Add(playerData);
            }
            return list;
        }

        public static List<List<object>> BuildGameplayStatsData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalGameplayStats stats = actor.GetGameplayStats(log, phase.Start, phase.End);
                list.Add(GetGameplayStatData(stats));
            }
            return list;
        }

        public static List<List<object>> BuildOffensiveStatsData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalOffensiveStats stats = actor.GetOffensiveStats(null, log, phase.Start, phase.End);
                list.Add(GetOffensiveStatData(stats));
            }
            return list;
        }

        public static List<List<List<object>>> BuildOffensiveStatsTargetsData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<List<object>>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                var playerData = new List<List<object>>();
                foreach (AbstractSingleActor target in phase.Targets)
                {
                    FinalOffensiveStats statsTarget = actor.GetOffensiveStats(target, log, phase.Start, phase.End);
                    playerData.Add(GetOffensiveStatData(statsTarget));
                }
                list.Add(playerData);
            }
            return list;
        }

        public static List<List<object>> BuildDefenseData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalDefensesAll defenses = actor.GetDefenseStats(log, phase.Start, phase.End);
                list.Add(GetDefenseStatData(defenses, phase));
            }

            return list;
        }

        public static List<List<object>> BuildSupportData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalToPlayersSupport support = actor.GetToPlayerSupportStats(log, phase.Start, phase.End);
                list.Add(GetSupportStatData(support));
            }
            return list;
        }
    }
}
