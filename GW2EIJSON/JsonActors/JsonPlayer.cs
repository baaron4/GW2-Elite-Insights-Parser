using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GW2EIJSON
{
    /// <summary>
    /// Class representing a player
    /// </summary>
    public class JsonPlayer : JsonActor
    {
        [JsonProperty]
        /// <summary>
        /// Account name of the player
        /// </summary>
        public string Account { get; set; }
        [JsonProperty]
        /// <summary>
        /// Group of the player
        /// </summary>
        public int Group { get; set; }
        [JsonProperty]
        /// <summary>
        /// Indicates if a player has a commander tag
        /// </summary>
        public bool HasCommanderTag { get; set; }
        [JsonProperty]
        /// <summary>
        /// Profession of the player
        /// </summary>
        public string Profession { get; set; }
        [JsonProperty]
        /// <summary>
        /// Indicates that the JsonPlayer is actually a friendly NPC
        /// </summary>
        public bool FriendlyNPC { get; set; }
        [JsonProperty]
        /// <summary>
        /// Weapons of the player \n
        /// 0-1 are the first land set, 1-2 are the second land set \n
        /// 3-4 are the first aquatic set, 5-6 are the second aquatic set \n
        /// When unknown, 'Unknown' value will appear \n
        /// If 2 handed weapon even indices will have "2Hand" as value
        /// </summary>
        public IReadOnlyList<string> Weapons { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonStatistics.JsonDPS"/>
        public IReadOnlyList<IReadOnlyList<JsonStatistics.JsonDPS>> DpsTargets { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of int representing 1S damage points \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>> TargetDamage1S { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of int representing 1S power damage points \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>> TargetPowerDamage1S { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of int representing 1S condition damage points \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>> TargetConditionDamage1S { get; set; }
        [JsonProperty]
        /// <summary>
        /// Array of double representing 1S breakbar damage points \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<double>>> TargetBreakbarDamage1S { get; set; }
        [JsonProperty]
        /// <summary>
        /// Per Target Damage distribution array \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<JsonDamageDist>>> TargetDamageDist { get; set; }
        [JsonProperty]
        /// <summary>
        /// Stats against targets  \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonStatistics.JsonGameplayStats"/>
        public IReadOnlyList<IReadOnlyList<JsonStatistics.JsonGameplayStats>> StatsTargets { get; set; }
        [JsonProperty]
        /// <summary>
        /// Support stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonStatistics.JsonPlayerSupport"/>
        public IReadOnlyList<JsonStatistics.JsonPlayerSupport> Support { get; set; }
        [JsonProperty]
        /// <summary>
        /// Damage modifiers against all
        /// </summary>
        /// <seealso cref="JsonDamageModifierData"/>
        public IReadOnlyList<JsonDamageModifierData> DamageModifiers { get; set; }
        [JsonProperty]
        /// <summary>
        /// Damage modifiers against targets \n
        /// Length == # of targets
        /// </summary>
        /// <seealso cref="JsonDamageModifierData"/>
        public IReadOnlyList<IReadOnlyList<JsonDamageModifierData>> DamageModifiersTarget { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public IReadOnlyList<JsonBuffsUptime> BuffUptimes { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on self generation  \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public IReadOnlyList<JsonPlayerBuffsGeneration> SelfBuffs { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on group generation
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsGeneration"/>
        public IReadOnlyList<JsonPlayerBuffsGeneration> GroupBuffs { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on off group generation
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsGeneration"/>
        public IReadOnlyList<JsonPlayerBuffsGeneration> OffGroupBuffs { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on squad generation
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsGeneration"/>
        public IReadOnlyList<JsonPlayerBuffsGeneration> SquadBuffs { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on active time
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public IReadOnlyList<JsonBuffsUptime> BuffUptimesActive { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on self generation on active time
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public IReadOnlyList<JsonPlayerBuffsGeneration> SelfBuffsActive { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on group generation on active time
        /// <seealso cref="JsonPlayerBuffsGeneration"/>
        public IReadOnlyList<JsonPlayerBuffsGeneration> GroupBuffsActive { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on off group generation on active time
        /// <seealso cref="JsonPlayerBuffsGeneration"/>
        public IReadOnlyList<JsonPlayerBuffsGeneration> OffGroupBuffsActive { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status on squad generation on active time
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsGeneration"/>
        public IReadOnlyList<JsonPlayerBuffsGeneration> SquadBuffsActive { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of death recaps \n
        /// Length == number of death
        /// </summary>
        /// <seealso cref="JsonDeathRecap"/>
        public IReadOnlyList<JsonDeathRecap> DeathRecap { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of used consumables
        /// </summary>
        /// <seealso cref="JsonConsumable"/>
        public IReadOnlyList<JsonConsumable> Consumables { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of time during which the player was active (not dead and not dc) \n
        /// Length == number of phases
        /// </summary>
        public IReadOnlyList<long> ActiveTimes { get; set; }

        [JsonConstructor]
        public JsonPlayer()
        {

        }
    }
}
