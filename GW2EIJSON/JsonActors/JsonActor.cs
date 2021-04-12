using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GW2EIJSON
{
    /// <summary>
    /// Base class for Players and NPCs
    /// </summary>
    /// <seealso cref="JsonPlayer"/> 
    /// <seealso cref="JsonNPC"/>
    public abstract class JsonActor
    {

        [JsonProperty]
        /// <summary>
        /// Name of the actor
        /// </summary>
        public string Name { get; set; }
        [JsonProperty]
        /// <summary>
        /// Total health of the actor. -1 if information is missing (ex: players)
        /// </summary>
        public int TotalHealth { get; set; }
        [JsonProperty]
        /// <summary>
        /// Condition damage score
        /// </summary>
        public uint Condition { get; set; }
        [JsonProperty]
        /// <summary>
        /// Concentration score
        /// </summary>
        public uint Concentration { get; set; }
        [JsonProperty]
        /// <summary>
        /// Healing Power score
        /// </summary>
        public uint Healing { get; set; }
        [JsonProperty]
        /// <summary>
        /// Toughness score
        /// </summary>
        public uint Toughness { get; set; }
        [JsonProperty]
        /// <summary>
        /// Height of the hitbox
        /// </summary>
        public uint HitboxHeight { get; set; }
        [JsonProperty]
        /// <summary>
        /// Width of the hitbox
        /// </summary>
        public uint HitboxWidth { get; set; }
        [JsonProperty]
        /// <summary>
        /// ID of the actor in the instance
        /// </summary>
        public ushort InstanceID { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of minions
        /// </summary>
        /// <seealso cref="JsonMinions"/>
        public IReadOnlyList<JsonMinions> Minions { get; set; }
        [JsonProperty]

        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public IReadOnlyList<JsonStatistics.JsonDPS> DpsAll { get; set; }
        [JsonProperty]
        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonGameplayStatsAll"/>
        public IReadOnlyList<JsonStatistics.JsonGameplayStatsAll> StatsAll { get; set; }
        [JsonProperty]
        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDefensesAll"/>
        public IReadOnlyList<JsonStatistics.JsonDefensesAll> Defenses { get; set; }
        [JsonProperty]
        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public IReadOnlyList<IReadOnlyList<JsonDamageDist>> TotalDamageDist { get; set; }
        [JsonProperty]
        /// <summary>
        /// Damage taken array
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public IReadOnlyList<IReadOnlyList<JsonDamageDist>> TotalDamageTaken { get; set; }
        [JsonProperty]
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public IReadOnlyList<JsonRotation> Rotation { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of int representing 1S damage points \n
        /// Length == # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<int>> Damage1S { get; set; }
        /// <summary>
        /// Array of int representing 1S power damage points \n
        /// Length == # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<int>> PowerDamage1S { get; set; }
        /// <summary>
        /// Array of int representing 1S condition damage points \n
        /// Length == # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<int>> ConditionDamage1S { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of double representing 1S breakbar damage points \n
        /// Length == # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<double>> BreakbarDamage1S { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of int[2] that represents the number of conditions \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of conditions present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> ConditionsStates { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of int[2] that represents the number of boons \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of boons present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> BoonsStates { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of int[2] that represents the number of active combat minions \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of active combat minions present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> ActiveCombatMinions { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of double[2] that represents the health status of the actor \n
        /// Array[i][0] will be the time, Array[i][1] will be health % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public IReadOnlyList<IReadOnlyList<double>> HealthPercents { get; set; }
        /// <summary>
        /// Array of double[2] that represents the barrier status of the actor \n
        /// Array[i][0] will be the time, Array[i][1] will be barrier % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public IReadOnlyList<IReadOnlyList<double>> BarrierPercents { get; set; }


        [JsonConstructor]
        internal JsonActor()
        {

        }
    }
}
