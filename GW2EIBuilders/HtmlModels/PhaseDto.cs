using System;
using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{

    public class PhaseDto
    {
        public string Name { get; internal set; }
        public long Duration { get; internal set; }
        public double Start { get; internal set; }
        public double End { get; internal set; }
        public List<int> Targets { get; internal set; } = new List<int>();
        public bool BreakbarPhase { get; internal set; }

        public List<List<object>> DpsStats { get; internal set; }
        public List<List<List<object>>> DpsStatsTargets { get; internal set; }
        public List<List<List<object>>> DmgStatsTargets { get; internal set; }
        public List<List<object>> DmgStats { get; internal set; }
        public List<List<object>> DefStats { get; internal set; }
        public List<List<object>> SupportStats { get; internal set; }
        // all
        public List<BuffData> BoonStats { get; internal set; }
        public List<BuffData> BoonGenSelfStats { get; internal set; }
        public List<BuffData> BoonGenGroupStats { get; internal set; }
        public List<BuffData> BoonGenOGroupStats { get; internal set; }
        public List<BuffData> BoonGenSquadStats { get; internal set; }

        public List<BuffData> OffBuffStats { get; internal set; }
        public List<BuffData> OffBuffGenSelfStats { get; internal set; }
        public List<BuffData> OffBuffGenGroupStats { get; internal set; }
        public List<BuffData> OffBuffGenOGroupStats { get; internal set; }
        public List<BuffData> OffBuffGenSquadStats { get; internal set; }

        public List<BuffData> SupBuffStats { get; internal set; }
        public List<BuffData> SupBuffGenSelfStats { get; internal set; }
        public List<BuffData> SupBuffGenGroupStats { get; internal set; }
        public List<BuffData> SupBuffGenOGroupStats { get; internal set; }
        public List<BuffData> SupBuffGenSquadStats { get; internal set; }

        public List<BuffData> DefBuffStats { get; internal set; }
        public List<BuffData> DefBuffGenSelfStats { get; internal set; }
        public List<BuffData> DefBuffGenGroupStats { get; internal set; }
        public List<BuffData> DefBuffGenOGroupStats { get; internal set; }
        public List<BuffData> DefBuffGenSquadStats { get; internal set; }

        public List<BuffData> PersBuffStats { get; internal set; }

        // active
        public List<BuffData> BoonActiveStats { get; internal set; }
        public List<BuffData> BoonGenActiveSelfStats { get; internal set; }
        public List<BuffData> BoonGenActiveGroupStats { get; internal set; }
        public List<BuffData> BoonGenActiveOGroupStats { get; internal set; }
        public List<BuffData> BoonGenActiveSquadStats { get; internal set; }

        public List<BuffData> OffBuffActiveStats { get; internal set; }
        public List<BuffData> OffBuffGenActiveSelfStats { get; internal set; }
        public List<BuffData> OffBuffGenActiveGroupStats { get; internal set; }
        public List<BuffData> OffBuffGenActiveOGroupStats { get; internal set; }
        public List<BuffData> OffBuffGenActiveSquadStats { get; internal set; }

        public List<BuffData> SupBuffActiveStats { get; internal set; }
        public List<BuffData> SupBuffGenActiveSelfStats { get; internal set; }
        public List<BuffData> SupBuffGenActiveGroupStats { get; internal set; }
        public List<BuffData> SupBuffGenActiveOGroupStats { get; internal set; }
        public List<BuffData> SupBuffGenActiveSquadStats { get; internal set; }

        public List<BuffData> DefBuffActiveStats { get; internal set; }
        public List<BuffData> DefBuffGenActiveSelfStats { get; internal set; }
        public List<BuffData> DefBuffGenActiveGroupStats { get; internal set; }
        public List<BuffData> DefBuffGenActiveOGroupStats { get; internal set; }
        public List<BuffData> DefBuffGenActiveSquadStats { get; internal set; }

        public List<BuffData> PersBuffActiveStats { get; internal set; }

        public List<DamageModData> DmgModifiersCommon { get; internal set; }
        public List<DamageModData> DmgModifiersItem { get; internal set; }
        public List<DamageModData> DmgModifiersPers { get; internal set; }

        public List<List<BuffData>> TargetsCondiStats { get; internal set; }
        public List<BuffData> TargetsCondiTotals { get; internal set; }
        public List<BuffData> TargetsBoonTotals { get; internal set; }

        public List<List<int[]>> MechanicStats { get; internal set; }
        public List<List<int[]>> EnemyMechanicStats { get; internal set; }
        public List<long> PlayerActiveTimes { get; internal set; }

        public List<double> MarkupLines { get; internal set; }
        public List<AreaLabelDto> MarkupAreas { get; internal set; }
        public List<int> SubPhases { get; internal set; }

        internal PhaseDto(PhaseData phaseData, List<PhaseData> phases, ParsedEvtcLog log)
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
        internal static List<List<object>> BuildDPSData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<List<object>>(log.PlayerList.Count);
            foreach (Player player in log.PlayerList)
            {
                FinalDPS dpsAll = player.GetDPSAll(log, phaseIndex);
                list.Add(GetDPSStatData(dpsAll));
            }
            return list;
        }

        internal static List<List<List<object>>> BuildDPSTargetsData(ParsedEvtcLog log, int phaseIndex)
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

        internal static List<List<object>> BuildDMGStatsData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<List<object>>();
            foreach (Player player in log.PlayerList)
            {
                FinalGameplayStatsAll stats = player.GetGameplayStats(log, phaseIndex);
                list.Add(GetDMGStatData(stats));
            }
            return list;
        }

        internal static List<List<List<object>>> BuildDMGStatsTargetsData(ParsedEvtcLog log, int phaseIndex)
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

        internal static List<List<object>> BuildDefenseData(ParsedEvtcLog log, int phaseIndex)
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

        internal static List<List<object>> BuildSupportData(ParsedEvtcLog log, int phaseIndex)
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
