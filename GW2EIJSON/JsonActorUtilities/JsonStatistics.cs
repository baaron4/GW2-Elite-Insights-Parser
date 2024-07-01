

namespace GW2EIJSON
{
    /// <summary>
    /// Class representing general statistics
    /// </summary>
    public static class JsonStatistics
    {
        /// <summary>
        /// Defensive stats
        /// </summary>
        public class JsonDefensesAll
        {

            /// <summary>
            /// Total damage taken
            /// </summary>
            public long DamageTaken { get; set; }
            /// <summary>
            /// Total condition damage taken
            /// </summary>
            public long ConditionDamageTaken { get; set; }
            /// <summary>
            /// Total power damage taken
            /// </summary>
            public long PowerDamageTaken { get; set; }
            /// <summary>
            /// Total strike damage taken
            /// </summary>
            public long StrikeDamageTaken { get; set; }
            /// <summary>
            /// Total life leech damage taken
            /// </summary>
            public long LifeLeechDamageTaken { get; set; }
            /// <summary>
            /// Total damage taken while downed
            /// </summary>
            public long DownedDamageTaken { get; set; }

            /// <summary>
            /// Total breakbar damage taken
            /// </summary>
            public double BreakbarDamageTaken { get; set; }

            /// <summary>
            /// Number of blocks
            /// </summary>
            public int BlockedCount { get; set; }

            /// <summary>
            /// Number of evades
            /// </summary>
            public int EvadedCount { get; set; }

            /// <summary>
            /// Number of misses
            /// </summary>
            public int MissedCount { get; set; }

            /// <summary>
            /// Number of dodges
            /// </summary>
            public int DodgeCount { get; set; }

            /// <summary>
            /// Number of time an incoming attack was negated by invul
            /// </summary>
            public int InvulnedCount { get; set; }

            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int DamageBarrier { get; set; }

            /// <summary>
            /// Number of time interrupted
            /// </summary>
            public int InterruptedCount { get; set; }

            /// <summary>
            /// Number of time downed
            /// </summary>
            public int DownCount { get; set; }

            /// <summary>
            /// Time passed in downstate
            /// </summary>
            public long DownDuration { get; set; }

            /// <summary>
            /// Number of time died
            /// </summary>
            public int DeadCount { get; set; }

            /// <summary>
            /// Time passed in dead state
            /// </summary>
            public long DeadDuration { get; set; }

            /// <summary>
            /// Number of time disconnected
            /// </summary>
            public int DcCount { get; set; }

            /// <summary>
            /// Time passed in disconnected state
            /// </summary>
            public long DcDuration { get; set; }
            /// <summary>
            /// Number of time boons were stripped
            /// </summary>
            public int BoonStrips { get; set; }
            /// <summary>
            /// Total duration of boons stripped
            /// </summary>
            public double BoonStripsTime { get; set; }
            /// <summary>
            /// Number of time conditions were cleansed
            /// </summary>
            public int ConditionCleanses { get; set; }
            /// <summary>
            /// Total duration of conditions cleansed
            /// </summary>
            public double ConditionCleansesTime { get; set; }


            public JsonDefensesAll()
            {

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
            public int Dps { get; set; }

            /// <summary>
            /// Total damage
            /// </summary>
            public int Damage { get; set; }

            /// <summary>
            /// Total condi dps
            /// </summary>
            public int CondiDps { get; set; }

            /// <summary>
            /// Total condi damage
            /// </summary>
            public int CondiDamage { get; set; }

            /// <summary>
            /// Total power dps
            /// </summary>
            public int PowerDps { get; set; }

            /// <summary>
            /// Total power damage
            /// </summary>
            public int PowerDamage { get; set; }

            /// <summary>
            /// Total breakbar damage
            /// </summary>
            public double BreakbarDamage { get; set; }

            /// <summary>
            /// Total actor only dps
            /// </summary>
            public int ActorDps { get; set; }

            /// <summary>
            /// Total actor only damage
            /// </summary>
            public int ActorDamage { get; set; }

            /// <summary>
            /// Total actor only condi dps
            /// </summary>
            public int ActorCondiDps { get; set; }

            /// <summary>
            /// Total actor only condi damage
            /// </summary>
            public int ActorCondiDamage { get; set; }

            /// <summary>
            /// Total actor only power dps
            /// </summary>
            public int ActorPowerDps { get; set; }

            /// <summary>
            /// Total actor only power damage
            /// </summary>
            public int ActorPowerDamage { get; set; }

            /// <summary>
            /// Total actor only breakbar damage
            /// </summary>
            public double ActorBreakbarDamage { get; set; }


            public JsonDPS()
            {

            }

        }

        /// <summary>
        /// Gameplay stats
        /// </summary>
        public class JsonGameplayStats
        {

            /// <summary>
            /// Number of damage hit
            /// </summary>
            public int TotalDamageCount { get; set; }
            /// <summary>
            /// Total damage
            /// </summary>
            public int TotalDmg { get; set; }

            /// <summary>
            /// Number of direct damage hit
            /// </summary>
            public int DirectDamageCount { get; set; }
            /// <summary>
            /// Total direct damage
            /// </summary>
            public int DirectDmg { get; set; }

            /// <summary>
            /// Number of connected direct damage hit
            /// </summary>
            public int ConnectedDirectDamageCount { get; set; }
            /// <summary>
            /// Total connected direct damage
            /// </summary>
            public int ConnectedDirectDmg { get; set; }

            /// <summary>
            /// Number of connected damage hit
            /// </summary>
            public int ConnectedDamageCount { get; set; }
            /// <summary>
            /// Total connected damage
            /// </summary>
            public int ConnectedDmg { get; set; }

            /// <summary>
            /// Number of critable hit
            /// </summary>
            public int CritableDirectDamageCount { get; set; }

            /// <summary>
            /// Number of crit
            /// </summary>
            public int CriticalRate { get; set; }

            /// <summary>
            /// Total critical damage
            /// </summary>
            public int CriticalDmg { get; set; }

            /// <summary>
            /// Number of hits while flanking
            /// </summary>
            public int FlankingRate { get; set; }

            /// <summary>
            /// Number of hits while target was moving
            /// </summary>
            public int AgainstMovingRate { get; set; }

            /// <summary>
            /// Number of glanced hits
            /// </summary>
            public int GlanceRate { get; set; }

            /// <summary>
            /// Number of missed hits
            /// </summary>
            public int Missed { get; set; }

            /// <summary>
            /// Number of evaded hits
            /// </summary>
            public int Evaded { get; set; }

            /// <summary>
            /// Number of blocked hits
            /// </summary>
            public int Blocked { get; set; }

            /// <summary>
            /// Number of hits that interrupted a skill
            /// </summary>
            public int Interrupts { get; set; }

            /// <summary>
            /// Number of hits against invulnerable targets
            /// </summary>
            public int Invulned { get; set; }
            /// <summary>
            /// Number of times killed target
            /// </summary>
            public int Killed { get; set; }

            /// <summary>
            /// Number of times downed target
            /// </summary>
            public int Downed { get; set; }
            /// <summary>
            /// Relevant for WvW, defined as the sum of damage done from 90% to down that led to a death \n
            /// </summary>
            public int DownContribution { get; set; }

            /// <summary>
            /// Number of times a Power based damage skill hits
            /// </summary>
            public int ConnectedPowerCount { get; set; }
            /// <summary>
            /// Number of times a Power based damage skill hits while source is above 90% hp
            /// </summary>
            public int ConnectedPowerAbove90HPCount { get; set; }
            /// <summary>
            /// Number of times a Condition based damage skill hits
            /// </summary>
            public int ConnectedConditionCount { get; set; }
            /// <summary>
            /// Number of times a Condition based damage skill hits while source is above 90% hp
            /// </summary>
            public int ConnectedConditionAbove90HPCount { get; set; }
            /// <summary>
            /// Number of times a skill hits while target is downed is downed
            /// </summary>
            public int AgainstDownedCount { get; set; }
            /// <summary>
            /// Damage done against downed target
            /// </summary>
            public int AgainstDownedDamage { get; set; }


            public JsonGameplayStats()
            {

            }
        }

        /// <summary>
        /// Gameplay stats
        /// </summary>
        public class JsonGameplayStatsAll : JsonGameplayStats
        {

            /// <summary>
            /// Number of time you interrupted your cast
            /// </summary>
            public int Wasted { get; set; }

            /// <summary>
            /// Time wasted by interrupting your cast
            /// </summary>
            public double TimeWasted { get; set; }

            /// <summary>
            /// Number of time you skipped an aftercast
            /// </summary>
            public int Saved { get; set; }

            /// <summary>
            /// Time saved while skipping aftercast
            /// </summary>
            public double TimeSaved { get; set; }

            /// <summary>
            /// Distance to the epicenter of the squad
            /// </summary>
            public double StackDist { get; set; }

            /// <summary>
            /// Distance to the commander of the squad. Only when a player with commander tag is present
            /// </summary>
            public double DistToCom { get; set; }

            /// <summary>
            /// Average amount of boons
            /// </summary>
            public double AvgBoons { get; set; }

            /// <summary>
            /// Average amount of boons over active time
            /// </summary>
            public double AvgActiveBoons { get; set; }

            /// <summary>
            /// Average amount of conditions
            /// </summary>
            public double AvgConditions { get; set; }

            /// <summary>
            /// Average amount of conditions over active time
            /// </summary>
            public double AvgActiveConditions { get; set; }

            /// <summary>
            /// Number of time a weapon swap happened
            /// </summary>
            public int SwapCount { get; set; }

            /// <summary>
            /// % of time in combat spent in animation
            /// </summary>
            public double SkillCastUptime { get; set; }

            /// <summary>
            /// % of time in combat spent in animation, excluding auto attack skills
            /// </summary>
            public double SkillCastUptimeNoAA { get; set; }


            public JsonGameplayStatsAll()
            {

            }
        }

        /// <summary>
        /// Support stats
        /// </summary>
        public class JsonPlayerSupport
        {

            /// <summary>
            /// Number of time ressurected someone
            /// </summary>
            public long Resurrects { get; set; }

            /// <summary>
            /// Time passed on ressurecting
            /// </summary>
            public double ResurrectTime { get; set; }

            /// <summary>
            /// Number of time a condition was cleansed on a squad mate
            /// </summary>
            public long CondiCleanse { get; set; }

            /// <summary>
            /// Total duration of condition cleansed on a squad mate
            /// </summary>
            public double CondiCleanseTime { get; set; }

            /// <summary>
            /// Number of time a condition was cleansed from self
            /// </summary>
            public long CondiCleanseSelf { get; set; }

            /// <summary>
            /// Total duration of condition cleansed from self
            /// </summary>
            public double CondiCleanseTimeSelf { get; set; }

            /// <summary>
            /// Number of time a boon was stripped
            /// </summary>
            public long BoonStrips { get; set; }

            /// <summary>
            /// Total duration of boons stripped from self
            /// </summary>
            public double BoonStripsTime { get; set; }


            public JsonPlayerSupport()
            {

            }
        }
    }
}
