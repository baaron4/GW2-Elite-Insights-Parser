using Newtonsoft.Json;

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
            [JsonProperty]
            /// <summary>
            /// Total damage taken
            /// </summary>
            public long DamageTaken { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total breakbar damage taken
            /// </summary>
            public double BreakbarDamageTaken { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of blocks
            /// </summary>
            public int BlockedCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of evades
            /// </summary>
            public int EvadedCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of misses
            /// </summary>
            public int MissedCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of dodges
            /// </summary>
            public int DodgeCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time an incoming attack was negated by invul
            /// </summary>
            public int InvulnedCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int DamageBarrier { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time interrupted
            /// </summary>
            public int InterruptedCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time downed
            /// </summary>
            public int DownCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Time passed in downstate
            /// </summary>
            public long DownDuration { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time died
            /// </summary>
            public int DeadCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Time passed in dead state
            /// </summary>
            public long DeadDuration { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time disconnected
            /// </summary>
            public int DcCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Time passed in disconnected state
            /// </summary>
            public long DcDuration { get; set; }

            [JsonConstructor]
            internal JsonDefensesAll()
            {

            }
        }

        /// <summary>
        /// DPS stats
        /// </summary>
        public class JsonDPS
        {
            [JsonProperty]
            /// <summary>
            /// Total dps
            /// </summary>
            public int Dps { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total damage
            /// </summary>
            public int Damage { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total condi dps
            /// </summary>
            public int CondiDps { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total condi damage
            /// </summary>
            public int CondiDamage { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total power dps
            /// </summary>
            public int PowerDps { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total power damage
            /// </summary>
            public int PowerDamage { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total breakbar damage
            /// </summary>
            public double BreakbarDamage { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only dps
            /// </summary>
            public int ActorDps { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only damage
            /// </summary>
            public int ActorDamage { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only condi dps
            /// </summary>
            public int ActorCondiDps { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only condi damage
            /// </summary>
            public int ActorCondiDamage { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only power dps
            /// </summary>
            public int ActorPowerDps { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only power damage
            /// </summary>
            public int ActorPowerDamage { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only breakbar damage
            /// </summary>
            public double ActorBreakbarDamage { get; set; }

            [JsonConstructor]
            internal JsonDPS()
            {

            }

        }

        /// <summary>
        /// Gameplay stats
        /// </summary>
        public class JsonGameplayStats
        {
            [JsonProperty]
            /// <summary>
            /// Number of damage hit
            /// </summary>
            public int TotalDamageCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of direct damage hit
            /// </summary>
            public int DirectDamageCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of connected direct damage hit
            /// </summary>
            public int ConnectedDirectDamageCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of critable hit
            /// </summary>
            public int CritableDirectDamageCount { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of crit
            /// </summary>
            public int CriticalRate { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total critical damage
            /// </summary>
            public int CriticalDmg { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of hits while flanking
            /// </summary>
            public int FlankingRate { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of glanced hits
            /// </summary>
            public int GlanceRate { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of missed hits
            /// </summary>
            public int Missed { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of evaded hits
            /// </summary>
            public int Evaded { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of blocked hits
            /// </summary>
            public int Blocked { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of hits that interrupted a skill
            /// </summary>
            public int Interrupts { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of hits against invulnerable targets
            /// </summary>
            public int Invulned { get; set; }
            /// <summary>
            /// Number of times killed target
            /// </summary>
            public int Killed { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of times downed target
            /// </summary>
            public int Downed { get; set; }

            [JsonConstructor]
            internal JsonGameplayStats()
            {

            }
        }

        /// <summary>
        /// Gameplay stats
        /// </summary>
        public class JsonGameplayStatsAll : JsonGameplayStats
        {
            [JsonProperty]
            /// <summary>
            /// Number of time you interrupted your cast
            /// </summary>
            public int Wasted { get; set; }
            [JsonProperty]
            /// <summary>
            /// Time wasted by interrupting your cast
            /// </summary>
            public double TimeWasted { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time you skipped an aftercast
            /// </summary>
            public int Saved { get; set; }
            [JsonProperty]
            /// <summary>
            /// Time saved while skipping aftercast
            /// </summary>
            public double TimeSaved { get; set; }
            [JsonProperty]
            /// <summary>
            /// Distance to the epicenter of the squad
            /// </summary>
            public double StackDist { get; set; }
            [JsonProperty]
            /// <summary>
            /// Distance to the commander of the squad. Only when a player with commander tag is present
            /// </summary>
            public double DistToCom { get; set; }
            [JsonProperty]
            /// <summary>
            /// Average amount of boons
            /// </summary>
            public double AvgBoons { get; set; }
            [JsonProperty]
            /// <summary>
            /// Average amount of boons over active time
            /// </summary>
            public double AvgActiveBoons { get; set; }
            [JsonProperty]
            /// <summary>
            /// Average amount of conditions
            /// </summary>
            public double AvgConditions { get; set; }
            [JsonProperty]
            /// <summary>
            /// Average amount of conditions over active time
            /// </summary>
            public double AvgActiveConditions { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time a weapon swap happened
            /// </summary>
            public int SwapCount { get; set; }

            [JsonConstructor]
            internal JsonGameplayStatsAll()
            {

            }
        }

        /// <summary>
        /// Support stats
        /// </summary>
        public class JsonPlayerSupport
        {
            [JsonProperty]
            /// <summary>
            /// Number of time ressurected someone
            /// </summary>
            public long Resurrects { get; set; }
            [JsonProperty]
            /// <summary>
            /// Time passed on ressurecting
            /// </summary>
            public double ResurrectTime { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time a condition was removed, self excluded
            /// </summary>
            public long CondiCleanse { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total time of condition removed, self excluded
            /// </summary>
            public double CondiCleanseTime { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time a condition was removed from self
            /// </summary>
            public long CondiCleanseSelf { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total time of condition removed from self
            /// </summary>
            public double CondiCleanseTimeSelf { get; set; }
            [JsonProperty]
            /// <summary>
            /// Number of time a boon was removed
            /// </summary>
            public long BoonStrips { get; set; }
            [JsonProperty]
            /// <summary>
            /// Total time of boons removed from self
            /// </summary>
            public double BoonStripsTime { get; set; }

            [JsonConstructor]
            internal JsonPlayerSupport()
            {

            }
        }
    }
}
