using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
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
            public long DamageTaken { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of blocks
            /// </summary>
            public int BlockedCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of evades
            /// </summary>
            public int EvadedCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of misses
            /// </summary>
            public int MissedCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of dodges
            /// </summary>
            public int DodgeCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time an incoming attack was negated by invul
            /// </summary>
            public int InvulnedCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int DamageBarrier { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time interrupted
            /// </summary>
            public int InterruptedCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time downed
            /// </summary>
            public int DownCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Time passed in downstate
            /// </summary>
            public long DownDuration { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time died
            /// </summary>
            public int DeadCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Time passed in dead state
            /// </summary>
            public long DeadDuration { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time disconnected
            /// </summary>
            public int DcCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Time passed in disconnected state
            /// </summary>
            public long DcDuration { get; internal set; }

            [JsonConstructor]
            internal JsonDefensesAll()
            {

            }

            internal JsonDefensesAll(FinalDefensesAll defenses)
            {
                DamageTaken = defenses.DamageTaken;
                BlockedCount = defenses.BlockedCount;
                DodgeCount = defenses.DodgeCount;
                MissedCount = defenses.MissedCount;
                EvadedCount = defenses.EvadedCount;
                InvulnedCount = defenses.InvulnedCount;
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
            [JsonProperty]
            /// <summary>
            /// Total dps
            /// </summary>
            public int Dps { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total damage
            /// </summary>
            public int Damage { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total condi dps
            /// </summary>
            public int CondiDps { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total condi damage
            /// </summary>
            public int CondiDamage { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total power dps
            /// </summary>
            public int PowerDps { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total power damage
            /// </summary>
            public int PowerDamage { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only dps
            /// </summary>
            public int ActorDps { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only damage
            /// </summary>
            public int ActorDamage { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only condi dps
            /// </summary>
            public int ActorCondiDps { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only condi damage
            /// </summary>
            public int ActorCondiDamage { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only power dps
            /// </summary>
            public int ActorPowerDps { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total actor only power damage
            /// </summary>
            public int ActorPowerDamage { get; internal set; }

            [JsonConstructor]
            internal JsonDPS()
            {

            }

            internal JsonDPS(FinalDPS stats)
            {
                Dps = stats.Dps;
                Damage = stats.Damage;
                CondiDps = stats.CondiDps;
                CondiDamage = stats.CondiDamage;
                PowerDps = stats.PowerDps;
                PowerDamage = stats.PowerDamage;

                ActorDps = stats.ActorDps;
                ActorDamage = stats.ActorDamage;
                ActorCondiDps = stats.ActorCondiDps;
                ActorCondiDamage = stats.ActorCondiDamage;
                ActorPowerDps = stats.ActorPowerDps;
                ActorPowerDamage = stats.ActorPowerDamage;
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
            public int TotalDamageCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of direct damage hit
            /// </summary>
            public int DirectDamageCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of connected direct damage hit
            /// </summary>
            public int ConnectedDirectDamageCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of critable hit
            /// </summary>
            public int CritableDirectDamageCount { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of crit
            /// </summary>
            public int CriticalRate { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total critical damage
            /// </summary>
            public int CriticalDmg { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of hits while flanking
            /// </summary>
            public int FlankingRate { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of glanced hits
            /// </summary>
            public int GlanceRate { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of missed hits
            /// </summary>
            public int Missed { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of evaded hits
            /// </summary>
            public int Evaded { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of blocked hits
            /// </summary>
            public int Blocked { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of hits that interrupted a skill
            /// </summary>
            public int Interrupts { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of hits against invulnerable targets
            /// </summary>
            public int Invulned { get; internal set; }
            /// <summary>
            /// Number of times killed target
            /// </summary>
            public int Killed { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of times downed target
            /// </summary>
            public int Downed { get; internal set; }

            [JsonConstructor]
            internal JsonGameplayStats()
            {

            }

            internal JsonGameplayStats(FinalGameplayStats stats)
            {
                TotalDamageCount = stats.TotalDamageCount;
                DirectDamageCount = stats.DirectDamageCount;
                ConnectedDirectDamageCount = stats.ConnectedDirectDamageCount;
                CritableDirectDamageCount = stats.CritableDirectDamageCount;
                CriticalRate = stats.CriticalCount;
                CriticalDmg = stats.CriticalDmg;
                FlankingRate = stats.FlankingCount;
                GlanceRate = stats.GlanceCount;
                Missed = stats.Missed;
                Blocked = stats.Blocked;
                Evaded = stats.Evaded;
                Interrupts = stats.Interrupts;
                Invulned = stats.Invulned;
                Killed = stats.Killed;
                Downed = stats.Downed;
            }

            internal JsonGameplayStats(FinalGameplayStatsAll stats) : this(stats as FinalGameplayStats)
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
            public int Wasted { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Time wasted by interrupting your cast
            /// </summary>
            public double TimeWasted { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time you skipped an aftercast
            /// </summary>
            public int Saved { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Time saved while skipping aftercast
            /// </summary>
            public double TimeSaved { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Distance to the epicenter of the squad
            /// </summary>
            public double StackDist { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Distance to the commander of the squad. Only when a player with commander tag is present
            /// </summary>
            public double DistToCom { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Average amount of boons
            /// </summary>
            public double AvgBoons { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Average amount of boons over active time
            /// </summary>
            public double AvgActiveBoons { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Average amount of conditions
            /// </summary>
            public double AvgConditions { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Average amount of conditions over active time
            /// </summary>
            public double AvgActiveConditions { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time a weapon swap happened
            /// </summary>
            public int SwapCount { get; internal set; }

            [JsonConstructor]
            internal JsonGameplayStatsAll()
            {

            }

            internal JsonGameplayStatsAll(FinalGameplayStatsAll stats) : base(stats)
            {
                Wasted = stats.Wasted;
                TimeWasted = stats.TimeWasted;
                Saved = stats.Saved;
                TimeSaved = stats.TimeSaved;
                StackDist = stats.StackDist;
                DistToCom = stats.DistToCom;
                AvgBoons = stats.AvgBoons;
                AvgActiveBoons = stats.AvgActiveBoons;
                AvgConditions = stats.AvgConditions;
                AvgActiveConditions = stats.AvgActiveConditions;
                SwapCount = stats.SwapCount;
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
            public long Resurrects { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Time passed on ressurecting
            /// </summary>
            public double ResurrectTime { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time a condition was removed, self excluded
            /// </summary>
            public long CondiCleanse { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total time of condition removed, self excluded
            /// </summary>
            public double CondiCleanseTime { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time a condition was removed from self
            /// </summary>
            public long CondiCleanseSelf { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total time of condition removed from self
            /// </summary>
            public double CondiCleanseTimeSelf { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Number of time a boon was removed
            /// </summary>
            public long BoonStrips { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Total time of boons removed from self
            /// </summary>
            public double BoonStripsTime { get; internal set; }

            [JsonConstructor]
            internal JsonPlayerSupport()
            {

            }

            internal JsonPlayerSupport(FinalPlayerSupport stats)
            {
                Resurrects = stats.Resurrects;
                ResurrectTime = stats.ResurrectTime;
                CondiCleanse = stats.CondiCleanse;
                CondiCleanseTime = stats.CondiCleanseTime;
                CondiCleanseSelf = stats.CondiCleanseSelf;
                CondiCleanseTimeSelf = stats.CondiCleanseTimeSelf;
                BoonStrips = stats.BoonStrips;
                BoonStripsTime = stats.BoonStripsTime;
            }
        }
    }
}
