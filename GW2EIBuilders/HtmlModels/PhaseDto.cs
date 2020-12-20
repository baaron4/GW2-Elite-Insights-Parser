using System;
using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

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
        public List<List<List<object>>> DmgStatsTargets { get; set; }
        public List<List<object>> DmgStats { get; set; }
        public List<List<object>> DefStats { get; set; }
        public List<List<object>> SupportStats { get; set; }
        // all
        public List<BuffData> BoonStats { get; set; }
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

        public List<BuffData> PersBuffStats { get; set; }

        // active
        public List<BuffData> BoonActiveStats { get; set; }
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

        public List<BuffData> PersBuffActiveStats { get; set; }

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

        public PhaseDto(PhaseData phaseData, IReadOnlyList<PhaseData> phases, ParsedEvtcLog log)
        {
            Name = phaseData.Name;
            Duration = phaseData.DurationInMS;
            Start = phaseData.Start / 1000.0;
            End = phaseData.End / 1000.0;
            BreakbarPhase = phaseData.BreakbarPhase;
            foreach (NPC target in phaseData.Targets)
            {
                Targets.Add(log.FightData.Logic.Targets.IndexOf(target));
            }
            PlayerActiveTimes = new List<long>();
            foreach (Player p in log.PlayerList)
            {
                PlayerActiveTimes.Add(phaseData.GetActorActiveDuration(p, log));
            }
            // add phase markup
            MarkupLines = new List<double>();
            MarkupAreas = new List<AreaLabelDto>();
            if (!BreakbarPhase)
            {
                for (int j = 1; j < phases.Count; j++)
                {
                    PhaseData curPhase = phases[j];
                    if (curPhase.Start < phaseData.Start || curPhase.End > phaseData.End ||
                        (curPhase.Start == phaseData.Start && curPhase.End == phaseData.End) || !curPhase.CanBeSubPhase)
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
        }


        // helper methods

        private static List<object> GetDMGStatData(FinalGameplayStatsAll stats)
        {
            List<object> data = GetDMGTargetStatData(stats);
            data.AddRange(new List<object>
                {
                    // commons
                    stats.TimeWasted, // 14
                    stats.Wasted, // 15

                    stats.TimeSaved, // 16
                    stats.Saved, // 17

                    stats.SwapCount, // 18
                    Math.Round(stats.StackDist, 2), // 19
                    Math.Round(stats.DistToCom, 2) // 20
                });
            return data;
        }

        private static List<object> GetDMGTargetStatData(FinalGameplayStats stats)
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

        private static List<object> GetSupportStatData(FinalPlayerSupport support)
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
            var data = new List<object>
                {
                    defenses.DamageTaken,
                    defenses.DamageBarrier,
                    defenses.BlockedCount,
                    defenses.InvulnedCount,
                    defenses.InterruptedCount,
                    defenses.EvadedCount,
                    defenses.DodgeCount,
                    defenses.MissedCount
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
        public static List<List<object>> BuildDPSData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<List<object>>(log.PlayerList.Count);
            foreach (Player player in log.PlayerList)
            {
                FinalDPS dpsAll = player.GetDPSAll(log, phaseIndex);
                list.Add(GetDPSStatData(dpsAll));
            }
            return list;
        }

        public static List<List<List<object>>> BuildDPSTargetsData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<List<List<object>>>(log.PlayerList.Count);
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];

            foreach (Player player in log.PlayerList)
            {
                var playerData = new List<List<object>>();

                foreach (NPC target in phase.Targets)
                {
                    playerData.Add(GetDPSStatData(player.GetDPSTarget(log, phaseIndex, target)));
                }
                list.Add(playerData);
            }
            return list;
        }

        public static List<List<object>> BuildDMGStatsData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<List<object>>();
            foreach (Player player in log.PlayerList)
            {
                FinalGameplayStatsAll stats = player.GetGameplayStats(log, phaseIndex);
                list.Add(GetDMGStatData(stats));
            }
            return list;
        }

        public static List<List<List<object>>> BuildDMGStatsTargetsData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<List<List<object>>>();

            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];

            foreach (Player player in log.PlayerList)
            {
                var playerData = new List<List<object>>();
                foreach (NPC target in phase.Targets)
                {
                    FinalGameplayStats statsTarget = player.GetGameplayStats(log, phaseIndex, target);
                    playerData.Add(GetDMGTargetStatData(statsTarget));
                }
                list.Add(playerData);
            }
            return list;
        }

        public static List<List<object>> BuildDefenseData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<List<object>>();

            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];

            foreach (Player player in log.PlayerList)
            {
                FinalDefensesAll defenses = player.GetDefenses(log, phaseIndex);
                list.Add(GetDefenseStatData(defenses, phase));
            }

            return list;
        }

        public static List<List<object>> BuildSupportData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<List<object>>();

            foreach (Player player in log.PlayerList)
            {
                FinalPlayerSupport support = player.GetPlayerSupport(log, phaseIndex);
                list.Add(GetSupportStatData(support));
            }
            return list;
        }
    }
}
