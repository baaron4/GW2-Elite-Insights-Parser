using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonStatistics
    {
        public class JsonBuffsData
        {
            public double Uptime;
            public double Generation;
            public double Overstack;
            public double Wasted;
            public double UnknownExtension;
            public double Extension;
            public double Extended;
            public double Presence;

            public JsonBuffsData(Statistics.FinalBuffs stats)
            {
                Uptime = stats.Uptime;
                Generation = stats.Generation;
                Overstack = stats.Overstack;
                Wasted = stats.Wasted;
                UnknownExtension = stats.UnknownExtension;
                Extended = stats.Extended;
                Presence = stats.Presence;
            }

        }


        public class JsonTargetBuffsData
        {
            public double Uptime;
            public double Presence;
            public Dictionary<string, double> Generated;
            public Dictionary<string, double> Overstacked;
            public Dictionary<string, double> Wasted;
            public Dictionary<string, double> UnknownExtension;
            public Dictionary<string, double> Extension;
            public Dictionary<string, double> Extended;


            private static Dictionary<string, double> ConvertKeys(Dictionary<ParseModels.Player, double> toConvert)
            {
                Dictionary<string, double> res = new Dictionary<string, double>();
                foreach (var pair in toConvert)
                {
                    res[pair.Key.Character] = pair.Value;
                }
                return res;
            }

            public JsonTargetBuffsData(Statistics.FinalTargetBuffs stats)
            {
                Uptime = stats.Uptime;
                Presence = stats.Presence;
                Generated = ConvertKeys(stats.Generated);
                Overstacked = ConvertKeys(stats.Overstacked);
                Wasted = ConvertKeys(stats.Wasted);
                UnknownExtension = ConvertKeys(stats.UnknownExtension);
                Extension = ConvertKeys(stats.Extension);
                Extended = ConvertKeys(stats.Extended);
            }
        }

        public class JsonDefenses
        {
            public long DamageTaken;
            public int BlockedCount;
            public int EvadedCount;
            public int DodgeCount;
            public int InvulnedCount;
            public int DamageInvulned;
            public int DamageBarrier;
            public int InterruptedCount;
            public int DownCount;
            public int DownDuration;
            public int DeadCount;
            public int DeadDuration;
            public int DcCount;
            public int DcDuration;

            public JsonDefenses(Statistics.FinalDefenses defenses)
            {
                DamageTaken = defenses.DamageTaken;
                BlockedCount = defenses.BlockedCount;
                DodgeCount = defenses.DodgeCount;
                EvadedCount = defenses.EvadedCount;
                InvulnedCount = defenses.InvulnedCount;
                DamageInvulned = defenses.DamageInvulned;
                DamageBarrier = defenses.DamageBarrier;
                InterruptedCount = defenses.InterruptedCount;
                DownCount = defenses.DownCount;
                DownDuration = defenses.DownDuration;
                DeadCount = defenses.DeadCount;
                DeadDuration = defenses.DeadDuration;
                DcCount = defenses.DcCount;
                DcDuration = defenses.DcDuration;
            }
        }

        public class JsonDPS
        {
            public int Dps;
            public int Damage;
            public int CondiDps;
            public int CondiDamage;
            public int PowerDps;
            public int PowerDamage;

            public JsonDPS(Statistics.FinalDPS stats)
            {
                Dps = stats.Dps;
                Damage = stats.Damage;
                CondiDps = stats.CondiDps;
                CondiDamage = stats.CondiDamage;
                PowerDps = stats.PowerDps;
                PowerDamage = stats.PowerDamage;
            }

        }

        public class JsonStats
        {
            public int PowerLoopCount;
            public int CritablePowerLoopCount;
            public int CriticalRate;
            public int CriticalDmg;
            public int ScholarRate;
            public int ScholarDmg;
            public int EagleRate;
            public int EagleDmg;
            public int MovingRate;
            public int MovingDamage;
            public int FlankingDmg;
            public int FlankingRate;
            public int GlanceRate;
            public int Missed;
            public int Interrupts;
            public int Invulned;
            public int PowerDamage;

            public JsonStats(Statistics.FinalStats stats)
            {
                PowerLoopCount = stats.PowerLoopCount;
                CritablePowerLoopCount = stats.CritablePowerLoopCount;
                CriticalRate = stats.CriticalRate;
                CriticalDmg = stats.CriticalDmg;
                ScholarRate = stats.ScholarRate;
                ScholarDmg = stats.ScholarDmg;
                EagleRate = stats.EagleRate;
                EagleDmg = stats.EagleDmg;
                MovingRate = stats.MovingRate;
                MovingDamage = stats.MovingDamage;
                FlankingDmg = stats.FlankingDmg;
                FlankingRate = stats.FlankingRate;
                GlanceRate = stats.GlanceRate;
                Missed = stats.Missed;
                Interrupts = stats.Interrupts;
                Invulned = stats.Invulned;
                PowerDamage = stats.PowerDamage;
            }

            public JsonStats(Statistics.FinalStatsAll stats)
            {
                PowerLoopCount = stats.PowerLoopCount;
                CritablePowerLoopCount = stats.CritablePowerLoopCount;
                CriticalRate = stats.CriticalRate;
                CriticalDmg = stats.CriticalDmg;
                ScholarRate = stats.ScholarRate;
                ScholarDmg = stats.ScholarDmg;
                EagleRate = stats.EagleRate;
                EagleDmg = stats.EagleDmg;
                MovingRate = stats.MovingRate;
                MovingDamage = stats.MovingDamage;
                FlankingDmg = stats.FlankingDmg;
                FlankingRate = stats.FlankingRate;
                GlanceRate = stats.GlanceRate;
                Missed = stats.Missed;
                Interrupts = stats.Interrupts;
                Invulned = stats.Invulned;
                PowerDamage = stats.PowerDamage;
            }
        }

        public class JsonStatsAll : JsonStats
        {
            public int Wasted;
            public double TimeWasted;
            public int Saved;
            public double TimeSaved;
            public double StackDist;
            public double AvgBoons;
            public double AvgConditions;
            public int SwapCount;

            public JsonStatsAll(Statistics.FinalStatsAll stats) : base(stats)
            {
                Wasted = stats.Wasted;
                TimeWasted = stats.TimeWasted;
                Saved = stats.Saved;
                TimeSaved = stats.TimeSaved;
                StackDist = stats.StackDist;
                AvgBoons = stats.AvgBoons;
                AvgConditions = stats.AvgConditions;
                SwapCount = stats.SwapCount;
            }
        }

        public class JsonSupport
        {
            public int Resurrects;
            public double ResurrectTime;
            public int CondiCleanse;
            public double CondiCleanseTime;

            public JsonSupport(Statistics.FinalSupport stats)
            {
                Resurrects = stats.Resurrects;
                ResurrectTime = stats.ResurrectTime;
                CondiCleanse = stats.CondiCleanse;
                CondiCleanseTime = stats.CondiCleanseTime;
            }
        }
    }
}
