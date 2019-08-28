using System;
using System.Collections.Generic;
using LuckParser.EIData;
using LuckParser.Models;
using LuckParser.Parser;

namespace LuckParser.Builders.HtmlModels
{

    public class PhaseDto
    {
        public string Name;
        public long Duration;
        public double Start;
        public double End;
        public List<int> Targets = new List<int>();

        public List<List<object>> DpsStats;
        public List<List<List<object>>> DpsStatsTargets;
        public List<List<List<object>>> DmgStatsTargets;
        public List<List<object>> DmgStats;
        public List<List<object>> DefStats;
        public List<List<object>> SupportStats;
        // all
        public List<BoonData> BoonStats;
        public List<BoonData> BoonGenSelfStats;
        public List<BoonData> BoonGenGroupStats;
        public List<BoonData> BoonGenOGroupStats;
        public List<BoonData> BoonGenSquadStats;

        public List<BoonData> OffBuffStats;
        public List<BoonData> OffBuffGenSelfStats;
        public List<BoonData> OffBuffGenGroupStats;
        public List<BoonData> OffBuffGenOGroupStats;
        public List<BoonData> OffBuffGenSquadStats;

        public List<BoonData> DefBuffStats;
        public List<BoonData> DefBuffGenSelfStats;
        public List<BoonData> DefBuffGenGroupStats;
        public List<BoonData> DefBuffGenOGroupStats;
        public List<BoonData> DefBuffGenSquadStats;

        public List<BoonData> PersBuffStats;

        // active
        public List<BoonData> BoonActiveStats;
        public List<BoonData> BoonGenActiveSelfStats;
        public List<BoonData> BoonGenActiveGroupStats;
        public List<BoonData> BoonGenActiveOGroupStats;
        public List<BoonData> BoonGenActiveSquadStats;

        public List<BoonData> OffBuffActiveStats;
        public List<BoonData> OffBuffGenActiveSelfStats;
        public List<BoonData> OffBuffGenActiveGroupStats;
        public List<BoonData> OffBuffGenActiveOGroupStats;
        public List<BoonData> OffBuffGenActiveSquadStats;

        public List<BoonData> DefBuffActiveStats;
        public List<BoonData> DefBuffGenActiveSelfStats;
        public List<BoonData> DefBuffGenActiveGroupStats;
        public List<BoonData> DefBuffGenActiveOGroupStats;
        public List<BoonData> DefBuffGenActiveSquadStats;

        public List<BoonData> PersBuffActiveStats;

        public List<DamageModData> DmgModifiersCommon;
        public List<DamageModData> DmgModifiersItem;
        public List<DamageModData> DmgModifiersPers;

        public List<List<BoonData>> TargetsCondiStats;
        public List<BoonData> TargetsCondiTotals;
        public List<BoonData> TargetsBoonTotals;

        public List<List<int[]>> MechanicStats;
        public List<List<int[]>> EnemyMechanicStats;
        public List<long> PlayerActiveTimes;

        public List<double> MarkupLines;
        public List<AreaLabelDto> MarkupAreas;
        public List<int> SubPhases;

        public PhaseDto(PhaseData phaseData, List<PhaseData> phases, ParsedLog log)
        {
            Name = phaseData.Name;
            Duration = phaseData.DurationInMS;
            Start = phaseData.Start / 1000.0;
            End = phaseData.End / 1000.0;
            foreach (Target target in phaseData.Targets)
            {
                Targets.Add(log.FightData.Logic.Targets.IndexOf(target));
            }
            PlayerActiveTimes = new List<long>();
            foreach (Player p in log.PlayerList)
            {
                PlayerActiveTimes.Add(phaseData.GetPlayerActiveDuration(p, log));
            }
            // add phase markup
            MarkupLines = new List<double>();
            MarkupAreas = new List<AreaLabelDto>();
            for (int j = 1; j < phases.Count; j++)
            {
                PhaseData curPhase = phases[j];
                if (curPhase.Start < phaseData.Start || curPhase.End > phaseData.End ||
                    (curPhase.Start == phaseData.Start && curPhase.End == phaseData.End))
                {
                    continue;
                }
                if (SubPhases == null)
                {
                    SubPhases = new List<int>();
                }
                SubPhases.Add(j);
                long start = curPhase.Start - phaseData.Start;
                long end = curPhase.End - phaseData.Start;
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
                    Label = curPhase.Name,
                    Highlight = curPhase.DrawArea
                };
                MarkupAreas.Add(phaseArea);
            }
            if (MarkupAreas.Count == 0)
            {
                MarkupAreas = null;
            }

            if (MarkupLines.Count == 0)
            {
                MarkupLines = null;
            }
        }


        // helper methods

        public static List<object> GetDMGStatData(Statistics.FinalStatsAll stats)
        {
            List<object> data = GetDMGTargetStatData(stats);
            data.AddRange(new List<object>
                {
                    // commons
                    stats.TimeWasted, // 9
                    stats.Wasted, // 10

                    stats.TimeSaved, // 11
                    stats.Saved, // 12

                    stats.SwapCount, // 13
                    Math.Round(stats.StackDist, 2) // 14
                });
            return data;
        }

        public static List<object> GetDMGTargetStatData(Statistics.FinalStats stats)
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
                    stats.Invulned // 8
                };
            return data;
        }

        public static List<object> GetDPSStatData(Statistics.FinalDPS dpsAll)
        {
            var data = new List<object>
                {
                    dpsAll.Damage,
                    dpsAll.PowerDamage,
                    dpsAll.CondiDamage,
                };
            return data;
        }

        public static List<object> GetSupportStatData(Statistics.FinalSupport support)
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

        public static List<object> GetDefenseStatData(Statistics.FinalDefenses defenses, PhaseData phase)
        {
            var data = new List<object>
                {
                    defenses.DamageTaken,
                    defenses.DamageBarrier,
                    defenses.BlockedCount,
                    defenses.InvulnedCount,
                    defenses.InterruptedCount,
                    defenses.EvadedCount,
                    defenses.DodgeCount
                };

            if (defenses.DownDuration > 0)
            {
                var downDuration = TimeSpan.FromMilliseconds(defenses.DownDuration);
                data.Add(defenses.DownCount);
                data.Add(downDuration.TotalSeconds + " seconds downed, " + Math.Round((downDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1) + "% Downed");
            }
            else
            {
                data.Add(0);
                data.Add("0% downed");
            }

            if (defenses.DeadDuration > 0)
            {
                var deathDuration = TimeSpan.FromMilliseconds(defenses.DeadDuration);
                data.Add(defenses.DeadCount);
                data.Add(deathDuration.TotalSeconds + " seconds dead, " + (100.0 - Math.Round((deathDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1)) + "% Alive");
            }
            else
            {
                data.Add(0);
                data.Add("100% Alive");
            }
            return data;
        }
    }
}
