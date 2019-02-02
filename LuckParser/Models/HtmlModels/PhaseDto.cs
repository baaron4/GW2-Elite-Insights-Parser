using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{
    
    public class PhaseDto
    {
        public string Name;
        [DefaultValue(null)]
        public long Duration;
        [DefaultValue(null)]
        public double Start;
        [DefaultValue(null)]
        public double End;
        public List<int> Targets = new List<int>();

        public List<List<object>> DpsStats;
        public List<List<List<object>>> DpsStatsTargets;
        public List<List<List<object>>> DmgStatsTargets;
        public List<List<object>> DmgStats;
        public List<List<object>> DefStats;
        public List<List<object>> HealStats;

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

        public List<List<object[]>> DmgModifiersCommon = new List<List<object[]>>();
        public List<List<List<object[]>>> DmgModifiersTargetsCommon = new List<List<List<object[]>>>();

        public List<List<BoonData>> TargetsCondiStats;
        public List<BoonData> TargetsCondiTotals;
        public List<BoonData> TargetsBoonTotals;

        public List<List<int[]>> MechanicStats;
        public List<List<int[]>> EnemyMechanicStats;

        public List<double> MarkupLines;
        public List<AreaLabelDto> MarkupAreas;
        public List<int> SubPhases;


        // helper methods

        public static List<object> GetDMGStatData(Statistics.FinalStatsAll stats)
        {
            List<object> data = new List<object>
                {
                    stats.PowerLoopCount,
                    stats.CritablePowerLoopCount,
                    stats.CriticalRate,
                    stats.CriticalDmg,

                    stats.ScholarRate,
                    stats.ScholarDmg,
                    stats.PowerDamage,

                    stats.MovingRate,
                    stats.MovingDamage,

                    stats.FlankingRate,

                    stats.GlanceRate,

                    stats.Missed,
                    stats.Interrupts,
                    stats.Invulned,

                    stats.EagleRate,
                    stats.EagleDmg,
                    stats.FlankingDmg, 
                    // commons
                    stats.TimeWasted,
                    stats.Wasted,

                    stats.TimeSaved,
                    stats.Saved,

                    stats.SwapCount,
                    Math.Round(stats.StackDist, 2)
                };
            return data;
        }

        public static List<object> GetDMGTargetStatData(Statistics.FinalStats stats)
        {
            List<object> data = new List<object>
                {
                    stats.PowerLoopCount,
                    stats.CritablePowerLoopCount,
                    stats.CriticalRate,
                    stats.CriticalDmg,

                    stats.ScholarRate,
                    stats.ScholarDmg,
                    stats.PowerDamage,

                    stats.MovingRate,
                    stats.MovingDamage,

                    stats.FlankingRate,

                    stats.GlanceRate,

                    stats.Missed,
                    stats.Interrupts,
                    stats.Invulned,

                    stats.EagleRate,
                    stats.EagleDmg,
                    stats.FlankingDmg
                };
            return data;
        }

        public static List<object> GetDPSStatData(Statistics.FinalDPS dpsAll)
        {
            List<object> data = new List<object>
                {
                    dpsAll.Damage,
                    dpsAll.Dps,
                    dpsAll.PowerDamage,
                    dpsAll.PowerDps,
                    dpsAll.CondiDamage,
                    dpsAll.CondiDps
                };
            return data;
        }

        public static List<object> GetSupportStatData(Statistics.FinalSupport support)
        {
            List<object> data = new List<object>()
                {
                    support.CondiCleanse,
                    support.CondiCleanseTime,
                    support.Resurrects,
                    support.ResurrectTime
                };
            return data;
        }

        public static List<object> GetDefenseStatData(Statistics.FinalDefenses defenses, PhaseData phase)
        {
            List<object> data = new List<object>
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
                TimeSpan downDuration = TimeSpan.FromMilliseconds(defenses.DownDuration);
                data.Add(defenses.DownCount);
                data.Add(downDuration.TotalSeconds + " seconds downed, " + Math.Round((downDuration.TotalMilliseconds / phase.GetDuration()) * 100, 1) + "% Downed");
            }
            else
            {
                data.Add(0);
                data.Add("0% downed");
            }

            if (defenses.DeadDuration > 0)
            {
                TimeSpan deathDuration = TimeSpan.FromMilliseconds(defenses.DeadDuration);
                data.Add(defenses.DeadCount);
                data.Add(deathDuration.TotalSeconds + " seconds dead, " + (100.0 - Math.Round((deathDuration.TotalMilliseconds / phase.GetDuration()) * 100, 1)) + "% Alive");
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
