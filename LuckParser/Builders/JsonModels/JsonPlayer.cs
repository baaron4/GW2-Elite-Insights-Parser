using System.Collections.Generic;
using static LuckParser.Builders.JsonModels.JsonStatistics;

namespace LuckParser.Builders.JsonModels
{
    public class JsonPlayer : JsonActor
    {
        /// <summary>
        /// Account name of the player
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Group of the player
        /// </summary>
        public int Group { get; set; }
        /// <summary>
        /// Profession of the player
        /// </summary>
        public string Profession { get; set; }
        /// <summary>
        /// Weapons of the player \n
        /// 0-1 are the first land set, 1-2 are the second land set \n
        /// 3-4 are the first aquatic set, 5-6 are the second aquatic set \n
        /// When unknown, 'Unknown' value will appear \n
        /// If 2 handed weapon even indices will have "2Hand" as value
        /// </summary>
        public string[] Weapons { get; set; }
        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public JsonDPS[][] DpsTargets { get; set; }
        /// <summary>
        /// Array of int representing 1S damage points \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public List<int>[][] TargetDamage1S { get; set; }
        /// <summary>
        /// Per Target Damage distribution array \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[][] TargetDamageDist { get; set; }
        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonStatsAll"/>
        public JsonStatsAll[] StatsAll { get; set; }
        /// <summary>
        /// Stats against targets  \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonStats"/>
        public JsonStats[][] StatsTargets { get; set; }
        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDefenses"/>
        public JsonDefenses[] Defenses { get; set; }
        /// <summary>
        /// Support stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonSupport"/>
        public JsonSupport[] Support { get; set; }
        /// <summary>
        /// Damage modifiers against all
        /// </summary>
        /// <seealso cref="JsonBuffDamageModifierData"/>
        public List<JsonBuffDamageModifierData> DamageModifiers { get; set; }
        /// <summary>
        /// Damage modifiers against targets \n
        /// Length == # of targets
        /// </summary>
        /// <seealso cref="JsonBuffDamageModifierData"/>
        public List<JsonBuffDamageModifierData>[] DamageModifiersTarget { get; set; }
        /// <summary>
        /// List of buff status on uptimes + states \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsUptime> BuffUptimes { get; set; }
        /// <summary>
        /// List of buff status on self generation  \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> SelfBuffs { get; set; }
        /// <summary>
        /// List of buff status on group generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> GroupBuffs { get; set; }
        /// <summary>
        /// List of buff status on off group generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> OffGroupBuffs { get; set; }
        /// <summary>
        /// List of buff status on squad generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> SquadBuffs { get; set; }
        /// <summary>
        /// List of buff status on uptimes + states on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsUptime> BuffUptimesActive { get; set; }
        /// <summary>
        /// List of buff status on self generation on active time  \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> SelfBuffsActive { get; set; }
        /// <summary>
        /// List of buff status on group generation on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> GroupBuffsActive { get; set; }
        /// <summary>
        /// List of buff status on off group generation on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> OffGroupBuffsActive { get; set; }
        /// <summary>
        /// List of buff status on squad generation on active time\n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> SquadBuffsActive { get; set; }
        /// <summary>
        /// List of death recaps \n
        /// Length == number of death
        /// </summary>
        /// <seealso cref="JsonDeathRecap"/>
        public List<JsonDeathRecap> DeathRecap { get; set; }
        /// <summary>
        /// List of used consumables
        /// </summary>
        /// <seealso cref="JsonConsumable"/>
        public List<JsonConsumable> Consumables { get; set; }
        /// <summary>
        /// List of time during which the player was active (not dead and not dc) \n
        /// Length == number of phases
        /// </summary>
        public List<long> ActiveTimes { get; set; }
    }
}
