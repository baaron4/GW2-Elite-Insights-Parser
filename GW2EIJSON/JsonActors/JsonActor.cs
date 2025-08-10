using System.Collections.Generic;

namespace GW2EIJSON;

/// <summary>
/// Base class for Players and NPCs
/// </summary>
/// <seealso cref="JsonPlayer"/> 
/// <seealso cref="JsonNPC"/>
public abstract class JsonActor
{
    /// <summary>
    /// Name of the actor
    /// </summary>
    public string? Name;
    /// <summary>
    /// Time at which actor started being tracked
    /// </summary>
    public int FirstAware;
    /// <summary>
    /// Time at which target ended being tracked
    /// </summary>
    public int LastAware;
    /// <summary>
    /// Total health of the actor. -1 if information is missing (ex: players)
    /// </summary>
    public int TotalHealth;
    /// <summary>
    /// Condition damage score
    /// </summary>
    public uint Condition;
    /// <summary>
    /// Concentration score
    /// </summary>
    public uint Concentration;
    /// <summary>
    /// Healing Power score
    /// </summary>
    public uint Healing;
    /// <summary>
    /// Toughness score
    /// </summary>
    public uint Toughness;
    /// <summary>
    /// Height of the hitbox, please not that the center of the box is at the feet of the agent
    /// </summary>
    public uint HitboxHeight;
    /// <summary>
    /// Width of the hitbox
    /// </summary>
    public uint HitboxWidth;
    /// <summary>
    /// ID of the actor in the instance
    /// </summary>
    public ushort InstanceID;

    /// <summary>
    /// The team ID of the actor. \n
    /// Mainly useful for WvW logs to differentiate targets from different servers. \n
    /// In PvE logs, the situation is always a simple Friend vs Foe situation. \
    /// If value is equal to 0 then no information regarding TeamID was present for given actor.
    /// </summary>
    public ulong TeamID;

    /// <summary>
    /// List of minions
    /// </summary>
    /// <seealso cref="JsonMinions"/>
    public IReadOnlyList<JsonMinions>? Minions;

    /// <summary>
    /// Indicates that the JsonActor does not exist in reality
    /// </summary>
    public bool IsFake;


    /// <summary>
    /// Array of Total DPS stats \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonStatistics.JsonDPS"/>
    public IReadOnlyList<JsonStatistics.JsonDPS>? DpsAll;

    /// <summary>
    /// Stats against all  \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonStatistics.JsonGameplayStatsAll"/>
    public IReadOnlyList<JsonStatistics.JsonGameplayStatsAll>? StatsAll;

    /// <summary>
    /// Defensive stats \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonStatistics.JsonDefensesAll"/>
    public IReadOnlyList<JsonStatistics.JsonDefensesAll>? Defenses;

    /// <summary>
    /// Total Damage distribution array \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonDamageDist"/>
    public IReadOnlyList<IReadOnlyList<JsonDamageDist>>? TotalDamageDist;

    /// <summary>
    /// Damage taken array
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonDamageDist"/>
    public IReadOnlyList<IReadOnlyList<JsonDamageDist>>? TotalDamageTaken;

    /// <summary>
    /// Rotation data
    /// </summary>
    /// <seealso cref="JsonRotation"/>
    public IReadOnlyList<JsonRotation>? Rotation;

    /// <summary>
    /// Array of int representing 1S damage points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<int>>? Damage1S;
    /// <summary>
    /// Array of int representing 1S power damage points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<int>>? PowerDamage1S;
    /// <summary>
    /// Array of int representing 1S condition damage points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<int>>? ConditionDamage1S;

    /// <summary>
    /// Array of double representing 1S breakbar damage points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<double>?>? BreakbarDamage1S;

    /// <summary>
    /// Array of int[2] that represents the number of conditions \n
    /// Array[i][0] will be the time, Array[i][1] will be the number of conditions present from Array[i][0] to Array[i+1][0] \n
    /// If i corresponds to the last element that means the status did not change for the remainder of the log \n
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? ConditionsStates;

    /// <summary>
    /// Array of int[2] that represents the number of boons \n
    /// Array[i][0] will be the time, Array[i][1] will be the number of boons present from Array[i][0] to Array[i+1][0] \n
    /// If i corresponds to the last element that means the status did not change for the remainder of the log
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? BoonsStates;

    /// <summary>
    /// Array of int[2] that represents the number of active combat minions \n
    /// Array[i][0] will be the time, Array[i][1] will be the number of active combat minions present from Array[i][0] to Array[i+1][0] \n
    /// If i corresponds to the last element that means the status did not change for the remainder of the log
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? ActiveCombatMinions;

    /// <summary>
    /// Array of double[2] that represents the health status of the actor \n
    /// Array[i][0] will be the time, Array[i][1] will be health % \n
    /// If i corresponds to the last element that means the health did not change for the remainder of the log \n
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? HealthPercents;
    /// <summary>
    /// Array of double[2] that represents the barrier status of the actor \n
    /// Array[i][0] will be the time, Array[i][1] will be barrier % \n
    /// If i corresponds to the last element that means the health did not change for the remainder of the log \n
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? BarrierPercents;
    /// <summary>
    /// Contains combat replay related data
    /// </summary>
    /// <seealso cref="JsonActorCombatReplayData"/>
    public JsonActorCombatReplayData? CombatReplayData;
}
