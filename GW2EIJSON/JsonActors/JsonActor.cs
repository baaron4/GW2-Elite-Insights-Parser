using System.Collections.Generic;
using System.Linq;

namespace GW2EIJSON
{
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
        public string Name { get; set; }
        /// <summary>
        /// Total health of the actor. -1 if information is missing (ex: players)
        /// </summary>
        public int TotalHealth { get; set; }
        /// <summary>
        /// Condition damage score
        /// </summary>
        public uint Condition { get; set; }
        /// <summary>
        /// Concentration score
        /// </summary>
        public uint Concentration { get; set; }
        /// <summary>
        /// Healing Power score
        /// </summary>
        public uint Healing { get; set; }
        /// <summary>
        /// Toughness score
        /// </summary>
        public uint Toughness { get; set; }
        /// <summary>
        /// Height of the hitbox, please not that the center of the box is at the feet of the agent
        /// </summary>
        public uint HitboxHeight { get; set; }
        /// <summary>
        /// Width of the hitbox
        /// </summary>
        public uint HitboxWidth { get; set; }
        /// <summary>
        /// ID of the actor in the instance
        /// </summary>
        public ushort InstanceID { get; set; }

        /// <summary>
        /// The team ID of the actor. \n
        /// Mainly useful for WvW logs to differentiate targets from different servers. \n
        /// In PvE logs, the situation is always a simple Friend vs Foe situation. \
        /// If value is equal to 0 then no information regarding TeamID was present for given actor.
        /// </summary>
        public ulong TeamID { get; set; }
        
        /// <summary>
        /// List of minions
        /// </summary>
        /// <seealso cref="JsonMinions"/>
        public IReadOnlyList<JsonMinions> Minions { get; set; }

        /// <summary>
        /// Indicates that the JsonActor does not exist in reality
        /// </summary>
        public bool IsFake { get; set; }


        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonStatistics.JsonDPS"/>
        public IReadOnlyList<JsonStatistics.JsonDPS> DpsAll { get; set; }

        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonStatistics.JsonGameplayStatsAll"/>
        public IReadOnlyList<JsonStatistics.JsonGameplayStatsAll> StatsAll { get; set; }

        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonStatistics.JsonDefensesAll"/>
        public IReadOnlyList<JsonStatistics.JsonDefensesAll> Defenses { get; set; }
        
        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public IReadOnlyList<IReadOnlyList<JsonDamageDist>> TotalDamageDist { get; set; }
        
        /// <summary>
        /// Damage taken array
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public IReadOnlyList<IReadOnlyList<JsonDamageDist>> TotalDamageTaken { get; set; }
        
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public IReadOnlyList<JsonRotation> Rotation { get; set; }
        
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
        
        /// <summary>
        /// Array of double representing 1S breakbar damage points \n
        /// Length == # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<double>> BreakbarDamage1S { get; set; }
        
        /// <summary>
        /// Array of int[2] that represents the number of conditions \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of conditions present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> ConditionsStates { get; set; }
        
        /// <summary>
        /// Array of int[2] that represents the number of boons \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of boons present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> BoonsStates { get; set; }
        
        /// <summary>
        /// Array of int[2] that represents the number of active combat minions \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of active combat minions present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> ActiveCombatMinions { get; set; }
        
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
        /// <summary>
        /// Contains combat replay related data
        /// </summary>
        /// <seealso cref="JsonActorCombatReplayData"/>
        public JsonActorCombatReplayData CombatReplayData { get; set; }


        protected JsonActor()
        {

        }
    }
}
