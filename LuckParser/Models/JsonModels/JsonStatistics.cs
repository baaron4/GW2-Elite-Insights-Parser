using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonStatistics
    {
        /// <summary>
        /// Defensive stats
        /// </summary>
        public class JsonDefenses
        {
            /// <summary>
            /// Total damage taken
            /// </summary>
            public long DamageTaken;
            /// <summary>
            /// Number of blocks
            /// </summary>
            public int BlockedCount;
            /// <summary>
            /// Number of evades
            /// </summary>
            public int EvadedCount;
            /// <summary>
            /// Number of dodges
            /// </summary>
            public int DodgeCount;
            /// <summary>
            /// Number of time an incoming attack was negated by invul
            /// </summary>
            public int InvulnedCount;
            /// <summary>
            /// Damage negated by invul
            /// </summary>
            public int DamageInvulned;
            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int DamageBarrier;
            /// <summary>
            /// Number of time interrupted
            /// </summary>
            public int InterruptedCount;
            /// <summary>
            /// Number of time downed
            /// </summary>
            public int DownCount;
            /// <summary>
            /// Time passed in downstate
            /// </summary>
            public int DownDuration;
            /// <summary>
            /// Number of time died
            /// </summary>
            public int DeadCount;
            /// <summary>
            /// Time passed in dead state
            /// </summary>
            public int DeadDuration;
            /// <summary>
            /// Number of time disconnected
            /// </summary>
            public int DcCount;
            /// <summary>
            /// Time passed in disconnected state
            /// </summary>
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

        /// <summary>
        /// DPS stats
        /// </summary>
        public class JsonDPS
        {
            /// <summary>
            /// Total dps
            /// </summary>
            public int Dps;
            /// <summary>
            /// Total damage
            /// </summary>
            public int Damage;
            /// <summary>
            /// Total condi dps
            /// </summary>
            public int CondiDps;
            /// <summary>
            /// Total condi damage
            /// </summary>
            public int CondiDamage;
            /// <summary>
            /// Total power dps
            /// </summary>
            public int PowerDps;
            /// <summary>
            /// Total power damage
            /// </summary>
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

        /// <summary>
        /// Gameplay stats
        /// </summary>
        public class JsonStats
        {
            /// <summary>
            /// Number of direct damage hit
            /// </summary>
            public int DirectDamageCount;
            /// <summary>
            /// Number of critable hit
            /// </summary>
            public int CritableDirectDamageCount;
            /// <summary>
            /// Number of crit
            /// </summary>
            public int CriticalRate;
            /// <summary>
            /// Total critical damage
            /// </summary>
            public int CriticalDmg;
            /// <summary>
            /// Number of hits above 90% hp
            /// </summary>
            public int ScholarRate;
            /// <summary>
            /// Scholar rune bonus damage
            /// </summary>
            public int ScholarDmg;
            /// <summary>
            /// Number of hits against a target below 50% hp
            /// </summary>
            public int EagleRate;
            /// <summary>
            /// Eagle rune bonus damage
            /// </summary>
            public int EagleDmg;
            /// <summary>
            /// Number of hits while moving
            /// </summary>
            public int MovingRate;
            /// <summary>
            /// Seaweed salad buff bonus damage
            /// </summary>
            public int MovingDamage;
            /// <summary>
            /// Number of hits while flanking
            /// </summary>
            public int FlankingRate;
            /// <summary>
            /// Thief rune bonus damage
            /// </summary>
            public int FlankingDmg;
            /// <summary>
            /// Number of glanced hits
            /// </summary>
            public int GlanceRate;
            /// <summary>
            /// Number of missed hits
            /// </summary>
            public int Missed;
            /// <summary>
            /// Number of hits that interrupted a skill
            /// </summary>
            public int Interrupts;
            /// <summary>
            /// Number of hits against invulnerable targets
            /// </summary>
            public int Invulned;
            /// <summary>
            /// Total direct damage
            /// </summary>
            public int DirectDamage;

            public JsonStats(Statistics.FinalStats stats)
            {
                DirectDamageCount = stats.DirectDamageCount;
                CritableDirectDamageCount = stats.CritableDirectDamageCount;
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
                DirectDamage = stats.DirectDamage;
            }

            public JsonStats(Statistics.FinalStatsAll stats)
            {
                DirectDamageCount = stats.DirectDamageCount;
                CritableDirectDamageCount = stats.CritableDirectDamageCount;
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
                DirectDamage = stats.DirectDamage;
            }
        }

        public class JsonStatsAll : JsonStats
        {
            /// <summary>
            /// Number of time you interrupted your cast
            /// </summary>
            public int Wasted;
            /// <summary>
            /// Time wasted by interrupting your cast
            /// </summary>
            public double TimeWasted;
            /// <summary>
            /// Number of time you skipped an aftercast
            /// </summary>
            public int Saved;
            /// <summary>
            /// Time saved while skipping aftercast
            /// </summary>
            public double TimeSaved;
            /// <summary>
            /// Distance to the epicenter of the squad
            /// </summary>
            public double StackDist;
            /// <summary>
            /// Average amount of boons
            /// </summary>
            public double AvgBoons;
            /// <summary>
            /// Average amount of conditions
            /// </summary>
            public double AvgConditions;
            /// <summary>
            /// Number of time a weapon swap happened
            /// </summary>
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

        /// <summary>
        /// Support stats
        /// </summary>
        public class JsonSupport
        {
            /// <summary>
            /// Number of time ressurected someone
            /// </summary>
            public int Resurrects;
            /// <summary>
            /// Time passed on ressurecting
            /// </summary>
            public double ResurrectTime;
            /// <summary>
            /// Number of time a condition was removed
            /// </summary>
            public int CondiCleanse;
            /// <summary>
            /// Total time of condition removed
            /// </summary>
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
