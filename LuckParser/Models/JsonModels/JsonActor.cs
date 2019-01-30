using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.JsonModels.JsonStatistics;

namespace LuckParser.Models.JsonModels
{
    /// <summary>
    /// Base class for Players and Targets
    /// </summary>
    /// <seealso cref="JsonPlayer"/> 
    /// <seealso cref="JsonTarget"/>
    public abstract class JsonActor
    {

        /// <summary>
        /// Name of the actor
        /// </summary>
        public string Name;
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
        /// Height of the hitbox
        /// </summary>
        public uint HitboxHeight;
        /// <summary>
        /// Width of the hitbox
        /// </summary>
        public uint HitboxWidth;
        /// <summary>
        /// List of minions
        /// </summary>
        /// <seealso cref="JsonMinions"/>
        public List<JsonMinions> Minions;

        /// <summary>
        /// Array of Total DPS stats
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public JsonDPS[] DpsAll;
        /// <summary>
        /// Total Damage distribution array
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[] TotalDamageDist;
        /// <summary>
        /// Damage taken array
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[] TotalDamageTaken;
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation;
        /// <summary>
        /// Array of int representing 1S damage points
        /// Length == # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public List<int>[] Damage1S;
        /// <summary>
        /// Array of int[2] that represents the number of conditions status
        /// Value[i][0] will be the time, value[i][1] will be the number of conditions present from value[i][0] to value[i+1][0]
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> ConditionsStates;
        /// <summary>
        /// Array of int[2] that represents the number of boons status
        /// Value[i][0] will be the time, value[i][1] will be the number of boons present from value[i][0] to value[i+1][0]
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> BoonsStates;
    }
}
