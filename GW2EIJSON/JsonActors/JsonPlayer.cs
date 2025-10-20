namespace GW2EIJSON;

/// <summary>
/// Class representing a player
/// </summary>
public class JsonPlayer : JsonActor
{
    /// <summary>
    /// Account name of the player
    /// </summary>
    public string? Account;

    /// <summary>
    /// Group of the player
    /// </summary>
    public int Group;

    /// <summary>
    /// Indicates if a player has a commander tag
    /// </summary>
    public bool HasCommanderTag;
    /// <summary>
    /// If <see cref="HasCommanderTag"/> is true, will contain the states of the tag. \n
    /// Array of int[2] that represents the commander tag status \n
    /// Array[i][0] will be the time at which player got tag. \n
    /// Array[i][1] will be the time at which player lost tag.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? CommanderTagStates;

    /// <summary>
    /// Profession of the player
    /// </summary>
    public string? Profession;

    /// <summary>
    /// Indicates that the JsonPlayer is actually a friendly NPC
    /// </summary>
    public bool FriendlyNPC;

    /// <summary>
    /// Indicates that the JsonPlayer is not actually part of the squad
    /// </summary>
    public bool NotInSquad;

    /// <summary>
    /// Guild id to be used to fetch from the official API
    /// </summary>
    public string? GuildID;

    /// <summary>
    /// Only relevant for instance logs. \n
    /// If true, indicates that the player has changed subgroups or specs during the log. A JsonPlayer representing each time frame will be present in the Json. Each instance will also have the same <see cref="JsonActor.InstanceID"/>
    /// </summary>
    public bool IsEnglobed;

    /// <summary>
    /// Weapons of the player \n
    /// 0-1 are the first land set, 1-2 are the second land set \n
    /// 3-4 are the first aquatic set, 5-6 are the second aquatic set \n
    /// When unknown, 'Unknown' value will appear \n
    /// If 2 handed weapon even indices will have "2Hand" as value \n
    /// DEPRECATED, use <see cref="WeaponSets"/> instead.\n
    /// In case where multiple weapon sets exist, this will be equal to the last one.
    /// </summary>
    public IReadOnlyList<string>? Weapons;


    /// <summary>
    /// Weapons of the player \n*
    /// </summary>
    public IReadOnlyList<JsonWeaponSet>? WeaponSets;

    /// <summary>
    /// Array of int[2] that represents the number of active clones \n
    /// Array[i][0] will be the time, Array[i][1] will be the number of clones present from Array[i][0] to Array[i+1][0] \n
    /// If i corresponds to the last element that means the status did not change for the remainder of the log \n
    /// Only relevant for clone summoning capable specs.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? ActiveClones;

    /// <summary>
    /// Array of int[2] that represents the number of active ranger pets \n
    /// Array[i][0] will be the time, Array[i][1] will be the number of ranger pets present from Array[i][0] to Array[i+1][0] \n
    /// If i corresponds to the last element that means the status did not change for the remainder of the log \n
    /// Only relevant for rangers.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? ActiveRangerPets;

    /// <summary>
    /// Array of Total DPS stats \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    /// <seealso cref="JsonStatistics.JsonDPS"/>
    public IReadOnlyList<IReadOnlyList<JsonStatistics.JsonDPS>>? DpsTargets;


    /// <summary>
    /// Array of int representing 1S damage taken points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<int>>? DamageTaken1S;
    /// <summary>
    /// Array of int representing 1S power damage taken points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<int>>? PowerDamageTaken1S;
    /// <summary>
    /// Array of int representing 1S condition damage taken points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<int>>? ConditionDamageTaken1S;

    /// <summary>
    /// Array of double representing 1S breakbar damage taken points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<double>?>? BreakbarDamageTaken1S;

    /// <summary>
    /// Array of int representing 1S damage points \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>>? TargetDamage1S;

    /// <summary>
    /// Array of int representing 1S power damage points \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>>? TargetPowerDamage1S;

    /// <summary>
    /// Array of int representing 1S condition damage points \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>>? TargetConditionDamage1S;

    /// <summary>
    /// Array of double representing 1S breakbar damage points \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<double>?>>? TargetBreakbarDamage1S;

    /// <summary>
    /// Per Target Damage distribution array \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    /// <seealso cref="JsonDamageDist"/>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<JsonDamageDist>>>? TargetDamageDist;

    /// <summary>
    /// Stats against targets  \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    /// <seealso cref="JsonStatistics.JsonGameplayStats"/>
    public IReadOnlyList<IReadOnlyList<JsonStatistics.JsonGameplayStats>>? StatsTargets;

    /// <summary>
    /// Support stats \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonStatistics.JsonPlayerSupport"/>
    public IReadOnlyList<JsonStatistics.JsonPlayerSupport>? Support;

    /// <summary>
    /// Damage modifiers against all
    /// </summary>
    /// <seealso cref="JsonDamageModifierData"/>
    public IReadOnlyList<JsonDamageModifierData>? DamageModifiers;

    /// <summary>
    /// Damage modifiers against targets \n
    /// Length == # of targets
    /// </summary>
    /// <seealso cref="JsonDamageModifierData"/>
    public IReadOnlyList<IReadOnlyList<JsonDamageModifierData>>? DamageModifiersTarget;

    /// <summary>
    /// Incoming damage modifiers from all
    /// </summary>
    /// <seealso cref="JsonDamageModifierData"/>
    public IReadOnlyList<JsonDamageModifierData>? IncomingDamageModifiers;

    /// <summary>
    /// Incoming damage modifiers from targets \n
    /// Length == # of targets
    /// </summary>
    /// <seealso cref="JsonDamageModifierData"/>
    public IReadOnlyList<IReadOnlyList<JsonDamageModifierData>>? IncomingDamageModifiersTarget;

    /// <summary>
    /// List of buff status
    /// </summary>
    /// <seealso cref="JsonBuffsUptime"/>
    public IReadOnlyList<JsonBuffsUptime>? BuffUptimes;

    /// <summary>
    /// List of buff status on self generation  \n
    /// Key is "'b' + id"
    /// </summary>
    /// <seealso cref="JsonPlayerBuffsGeneration"/>
    public IReadOnlyList<JsonPlayerBuffsGeneration>? SelfBuffs;

    /// <summary>
    /// List of buff status on group generation
    /// </summary>
    /// <seealso cref="JsonPlayerBuffsGeneration"/>
    public IReadOnlyList<JsonPlayerBuffsGeneration>? GroupBuffs;

    /// <summary>
    /// List of buff status on off group generation
    /// </summary>
    /// <seealso cref="JsonPlayerBuffsGeneration"/>
    public IReadOnlyList<JsonPlayerBuffsGeneration>? OffGroupBuffs;

    /// <summary>
    /// List of buff status on squad generation
    /// </summary>
    /// <seealso cref="JsonPlayerBuffsGeneration"/>
    public IReadOnlyList<JsonPlayerBuffsGeneration>? SquadBuffs;

    /// <summary>
    /// List of buff status on active time
    /// </summary>
    /// <seealso cref="JsonBuffsUptime"/>
    public IReadOnlyList<JsonBuffsUptime>? BuffUptimesActive;

    /// <summary>
    /// List of buff status on self generation on active time
    /// </summary>
    /// <seealso cref="JsonPlayerBuffsGeneration"/>
    public IReadOnlyList<JsonPlayerBuffsGeneration>? SelfBuffsActive;

    /// <summary>
    /// List of buff status on group generation on active time
    /// <seealso cref="JsonPlayerBuffsGeneration"/>
    public IReadOnlyList<JsonPlayerBuffsGeneration>? GroupBuffsActive;

    /// <summary>
    /// List of buff status on off group generation on active time
    /// <seealso cref="JsonPlayerBuffsGeneration"/>
    public IReadOnlyList<JsonPlayerBuffsGeneration>? OffGroupBuffsActive;

    /// <summary>
    /// List of buff status on squad generation on active time
    /// </summary>
    /// <seealso cref="JsonPlayerBuffsGeneration"/>
    public IReadOnlyList<JsonPlayerBuffsGeneration>? SquadBuffsActive;


    /// <summary>
    /// List of volumes status
    /// </summary>
    /// <seealso cref="JsonBuffVolumes"/>
    public IReadOnlyList<JsonBuffVolumes>? BuffVolumes;
    /// <summary>
    /// List of buff volumes on self outgoing  \n
    /// Key is "'b' + id"
    /// </summary>
    /// <seealso cref="JsonPlayerBuffOutgoingVolumes"/>
    public IReadOnlyList<JsonPlayerBuffOutgoingVolumes>? SelfBuffVolumes;

    /// <summary>
    /// List of buff volumes on group outgoing
    /// </summary>
    /// <seealso cref="JsonPlayerBuffOutgoingVolumes"/>
    public IReadOnlyList<JsonPlayerBuffOutgoingVolumes>? GroupBuffVolumes;

    /// <summary>
    /// List of buff volumes on off group outgoing
    /// </summary>
    /// <seealso cref="JsonPlayerBuffOutgoingVolumes"/>
    public IReadOnlyList<JsonPlayerBuffOutgoingVolumes>? OffGroupBuffVolumes;

    /// <summary>
    /// List of buff volumes on squad outgoing
    /// </summary>
    /// <seealso cref="JsonPlayerBuffOutgoingVolumes"/>
    public IReadOnlyList<JsonPlayerBuffOutgoingVolumes>? SquadBuffVolumes;


    /// <summary>
    /// List of buff volumes on active time
    /// </summary>
    /// <seealso cref="JsonBuffVolumes"/>
    public IReadOnlyList<JsonBuffVolumes>? BuffVolumesActive;
    /// <summary>
    /// List of buff volumes on self outgoing on active time
    /// </summary>
    /// <seealso cref="JsonPlayerBuffOutgoingVolumes"/>
    public IReadOnlyList<JsonPlayerBuffOutgoingVolumes>? SelfBuffVolumesActive;

    /// <summary>
    /// List of buff volumes on group outgoing on active time
    /// <seealso cref="JsonPlayerBuffOutgoingVolumes"/>
    public IReadOnlyList<JsonPlayerBuffOutgoingVolumes>? GroupBuffVolumesActive;

    /// <summary>
    /// List of buff volumes on off group outgoing on active time
    /// <seealso cref="JsonPlayerBuffOutgoingVolumes"/>
    public IReadOnlyList<JsonPlayerBuffOutgoingVolumes>? OffGroupBuffVolumesActive;

    /// <summary>
    /// List of buff volumes on squad outgoing on active time
    /// </summary>
    /// <seealso cref="JsonPlayerBuffOutgoingVolumes"/>
    public IReadOnlyList<JsonPlayerBuffOutgoingVolumes>? SquadBuffVolumesActive;

    /// <summary>
    /// List of death recaps \n
    /// Length == number of death
    /// </summary>
    /// <seealso cref="JsonDeathRecap"/>
    public IReadOnlyList<JsonDeathRecap>? DeathRecap;

    /// <summary>
    /// List of used consumables
    /// </summary>
    /// <seealso cref="JsonConsumable"/>
    public IReadOnlyList<JsonConsumable>? Consumables;

    /// <summary>
    /// List of time during which the player was active (not dead and not dc) \n
    /// Length == number of phases
    /// </summary>
    public IReadOnlyList<long>? ActiveTimes;

    /// <summary>
    /// Healing stats data
    /// </summary>
    public EXTJsonPlayerHealingStats? EXTHealingStats;

    /// <summary>
    /// Barrier stats data
    /// </summary>
    public EXTJsonPlayerBarrierStats? EXTBarrierStats;
}
