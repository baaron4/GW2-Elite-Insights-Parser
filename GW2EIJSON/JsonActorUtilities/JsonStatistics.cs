

namespace GW2EIJSON;

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
        public int DamageTaken;
        /// <summary>
        /// Number of damage taken events
        /// </summary>
        public int DamageTakenCount;
        /// <summary>
        /// Total condition damage taken
        /// </summary>
        public int ConditionDamageTaken;
        /// <summary>
        /// Number of condition damage taken events
        /// </summary>
        public int ConditionDamageTakenCount;
        /// <summary>
        /// Total power damage taken
        /// </summary>
        public int PowerDamageTaken;
        /// <summary>
        /// Number of power damage taken events
        /// </summary>
        public int PowerDamageTakenCount;
        /// <summary>
        /// Total strike damage taken
        /// </summary>
        public int StrikeDamageTaken;
        /// <summary>
        /// Number of strike damage taken events
        /// </summary>
        public int StrikeDamageTakenCount;
        /// <summary>
        /// Total life leech damage taken
        /// </summary>
        public int LifeLeechDamageTaken;
        /// <summary>
        /// Number of life leech damage taken event
        /// </summary>
        public int LifeLeechDamageTakenCount;
        /// <summary>
        /// Total damage taken while downed
        /// </summary>
        public int DownedDamageTaken;
        /// <summary>
        /// Number of damage taken while downed event
        /// </summary>
        public int DownedDamageTakenCount;
        /// <summary>
        /// Total barrier damage taken
        /// </summary>
        public long DamageBarrier;
        /// <summary>
        /// Number of barrier damage taken events
        /// </summary>
        public int DamageBarrierCount;
        /// <summary>
        /// Total breakbar damage taken
        /// </summary>
        public double BreakbarDamageTaken;
        /// <summary>
        /// Number of breakbar damage taken events
        /// </summary>
        public int BreakbarDamageTakenCount;

        /// <summary>
        /// Number of blocks
        /// </summary>
        public int BlockedCount;
        /// <summary>
        /// Number of evades
        /// </summary>
        public int EvadedCount;
        /// <summary>
        /// Number of misses
        /// </summary>
        public int MissedCount;
        /// <summary>
        /// Number of dodges
        /// </summary>
        public int DodgeCount;
        /// <summary>
        /// Number of time an incoming attack was negated by invul
        /// </summary>
        public int InvulnedCount;
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
        public long DownDuration;
        /// <summary>
        /// Number of time died
        /// </summary>
        public int DeadCount;
        /// <summary>
        /// Time passed in dead state
        /// </summary>
        public long DeadDuration;
        /// <summary>
        /// Number of time disconnected
        /// </summary>
        public int DcCount;
        /// <summary>
        /// Time passed in disconnected state
        /// </summary>
        public long DcDuration;

        /// <summary>
        /// Number of time boons were stripped
        /// </summary>
        public int BoonStrips;
        /// <summary>
        /// Total duration of boons stripped
        /// </summary>
        public double BoonStripsTime;
        /// <summary>
        /// Number of time conditions were cleansed
        /// </summary>
        public int ConditionCleanses;
        /// <summary>
        /// Total duration of conditions cleansed
        /// </summary>
        public double ConditionCleansesTime;

        /// <summary>
        /// Number of time crowd controlled
        /// </summary>
        public int ReceivedCrowdControl;
        /// <summary>
        /// Total crowd control duration received in ms
        /// </summary>
        public double ReceivedCrowdControlDuration;
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

        /// <summary>
        /// Total breakbar damage
        /// </summary>
        public double BreakbarDamage;

        /// <summary>
        /// Total actor only dps
        /// </summary>
        public int ActorDps;

        /// <summary>
        /// Total actor only damage
        /// </summary>
        public int ActorDamage;

        /// <summary>
        /// Total actor only condi dps
        /// </summary>
        public int ActorCondiDps;

        /// <summary>
        /// Total actor only condi damage
        /// </summary>
        public int ActorCondiDamage;

        /// <summary>
        /// Total actor only power dps
        /// </summary>
        public int ActorPowerDps;

        /// <summary>
        /// Total actor only power damage
        /// </summary>
        public int ActorPowerDamage;

        /// <summary>
        /// Total actor only breakbar damage
        /// </summary>
        public double ActorBreakbarDamage;
    }

    /// <summary>
    /// Gameplay stats
    /// </summary>
    public class JsonGameplayStats
    {
        /// <summary>
        /// Number of damage hit
        /// </summary>
        public int TotalDamageCount;
        /// <summary>
        /// Total damage
        /// </summary>
        public int TotalDmg;

        /// <summary>
        /// Number of direct damage hit
        /// </summary>
        public int DirectDamageCount;
        /// <summary>
        /// Total direct damage
        /// </summary>
        public int DirectDmg;

        /// <summary>
        /// Number of connected damage hit
        /// </summary>
        public int ConnectedDamageCount;
        /// <summary>
        /// Total connected damage
        /// </summary>
        public int ConnectedDmg;

        /// <summary>
        /// Number of connected direct damage hit
        /// </summary>
        public int ConnectedDirectDamageCount;
        /// <summary>
        /// Total connected direct damage
        /// </summary>
        public int ConnectedDirectDmg;

        /// <summary>
        /// Number of connected power damage hit
        /// </summary>
        public int ConnectedPowerCount;
        /// <summary>
        /// Total connected power damage
        /// </summary>
        public int ConnectedPowerDamage;
        /// <summary>
        /// Number of connected power damage hit while above 90% hp
        /// </summary>
        public int ConnectedPowerAbove90HPCount;
        /// <summary>
        /// Total connected power damage while above 90% hp
        /// </summary>
        public int ConnectedPowerAbove90HPDamage;

        /// <summary>
        /// Number of connected life leech damage hit
        /// </summary>
        public int ConnectedLifeLeechCount;
        /// <summary>
        /// Total connected life leech damage
        /// </summary>
        public int ConnectedLifeLeechDamage;

        /// <summary>
        /// Number of connected condition damage hit
        /// </summary>
        public int ConnectedConditionCount;
        /// <summary>
        /// Total connected condition damage
        /// </summary>
        public int ConnectedConditionDamage;
        /// <summary>
        /// Number of connected condition damage hit while above 90% hp
        /// </summary>
        public int ConnectedConditionAbove90HPCount;
        /// <summary>
        /// Total connected condition damage while above 90% hp
        /// </summary>
        public int ConnectedConditionAbove90HPDamage;

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
        /// Number of hits while flanking
        /// </summary>
        public int FlankingRate;
        /// <summary>
        /// Number of hits while target was moving
        /// </summary>
        public int AgainstMovingRate;
        /// <summary>
        /// Number of glanced hits
        /// </summary>
        public int GlanceRate;
        /// <summary>
        /// Number of missed hits
        /// </summary>
        public int Missed;
        /// <summary>
        /// Number of evaded hits
        /// </summary>
        public int Evaded;
        /// <summary>
        /// Number of blocked hits
        /// </summary>
        public int Blocked;
        /// <summary>
        /// Number of hits that interrupted a skill
        /// </summary>
        public int Interrupts;
        /// <summary>
        /// Number of hits against invulnerable targets
        /// </summary>
        public int Invulned;

        /// <summary>
        /// Number of times killed target
        /// </summary>
        public int Killed;
        /// <summary>
        /// Number of times downed target
        /// </summary>
        public int Downed;

        /// <summary>
        /// Number of times a skill hits while target is downed is downed
        /// </summary>
        public int AgainstDownedCount;
        /// <summary>
        /// Damage done against downed target
        /// </summary>
        public int AgainstDownedDamage;

        /// <summary>
        /// Relevant for WvW, defined as the sum of damage done from 90% to down that led to a death
        /// </summary>
        public int DownContribution;
        /// <summary>
        /// Relevant for WvW, defined as the number of CC applied from 90% to down that led to a death
        /// </summary>
        public int AppliedCrowdControlDownContribution;
        /// <summary>
        /// Relevant for WvW, defined as the total duration of CC applied from 90% to down that led to a death, in ms
        /// </summary>
        public double AppliedCrowdControlDurationDownContribution;

        /// <summary>
        /// Number of time applied a cc.
        /// </summary>
        public int AppliedCrowdControl;
        /// <summary>
        /// Total crowd control duration inflicted in ms
        /// </summary>
        public double AppliedCrowdControlDuration;
    }

    /// <summary>
    /// Gameplay stats
    /// </summary>
    public class JsonGameplayStatsAll : JsonGameplayStats
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
        /// Distance to the commander of the squad. Only when a player with commander tag is present
        /// </summary>
        public double DistToCom;

        /// <summary>
        /// Average amount of boons
        /// </summary>
        public double AvgBoons;

        /// <summary>
        /// Average amount of boons over active time
        /// </summary>
        public double AvgActiveBoons;

        /// <summary>
        /// Average amount of conditions
        /// </summary>
        public double AvgConditions;

        /// <summary>
        /// Average amount of conditions over active time
        /// </summary>
        public double AvgActiveConditions;

        /// <summary>
        /// Number of time a weapon swap happened
        /// </summary>
        public int SwapCount;

        /// <summary>
        /// % of time in combat spent in animation
        /// </summary>
        public double SkillCastUptime;

        /// <summary>
        /// % of time in combat spent in animation, excluding auto attack skills
        /// </summary>
        public double SkillCastUptimeNoAA;
    }

    /// <summary>
    /// Support stats
    /// </summary>
    public class JsonPlayerSupport
    {

        /// <summary>
        /// Number of time ressurected someone
        /// </summary>
        public long Resurrects;

        /// <summary>
        /// Time passed on ressurecting
        /// </summary>
        public double ResurrectTime;

        /// <summary>
        /// Number of time a condition was cleansed on a squad mate
        /// </summary>
        public long CondiCleanse;

        /// <summary>
        /// Total duration of condition cleansed on a squad mate
        /// </summary>
        public double CondiCleanseTime;

        /// <summary>
        /// Number of time a condition was cleansed from self
        /// </summary>
        public long CondiCleanseSelf;

        /// <summary>
        /// Total duration of condition cleansed from self
        /// </summary>
        public double CondiCleanseTimeSelf;

        /// <summary>
        /// Number of time a boon was stripped
        /// </summary>
        public long BoonStrips;

        /// <summary>
        /// Total duration of boons stripped from self
        /// </summary>
        public double BoonStripsTime;
        /// <summary>
        /// Number of time stun was broken, by self or others
        /// </summary>
        public int StunBreak;
        /// <summary>
        /// Removed stun duration in s.
        /// </summary>
        public double RemovedStunDuration;
    }
}
