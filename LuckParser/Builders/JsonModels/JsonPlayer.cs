using System.Collections.Generic;
using static LuckParser.Builders.JsonModels.JsonStatistics;

namespace LuckParser.Builders.JsonModels
{
    public class JsonPlayer : JsonActor
    {      
        /// <summary>
        /// Account name of the player
        /// </summary>
        public string Account;
        /// <summary>
        /// Group of the player
        /// </summary>
        public int Group;
        /// <summary>
        /// Profession of the player
        /// </summary>
        public string Profession;
        /// <summary>
        /// Weapons of the player \n
        /// 0-1 are the first land set, 1-2 are the second land set \n
        /// 3-4 are the first aquatic set, 5-6 are the second aquatic set \n
        /// When unknown, 'Unknown' value will appear \n
        /// If 2 handed weapon even indices will have "2Hand" as value
        /// </summary>
        public string[] Weapons;
        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public JsonDPS[][] DpsTargets;
        /// <summary>
        /// Array of int representing 1S damage points \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public List<int>[][] TargetDamage1S;
        /// <summary>
        /// Per Target Damage distribution array \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[][] TargetDamageDist;
        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonStatsAll"/>
        public JsonStatsAll[] StatsAll;
        /// <summary>
        /// Stats against targets  \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonStats"/>
        public JsonStats[][] StatsTargets;
        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDefenses"/>
        public JsonDefenses[] Defenses;
        /// <summary>
        /// Support stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonSupport"/>
        public JsonSupport[] Support;
        /// <summary>
        /// Damage modifiers against all
        /// </summary>
        /// <seealso cref="JsonBuffDamageModifierData"/>
        public List<JsonBuffDamageModifierData> DamageModifiers;
        /// <summary>
        /// Damage modifiers against targets \n
        /// Length == # of targets
        /// </summary>
        /// <seealso cref="JsonBuffDamageModifierData"/>
        public List<JsonBuffDamageModifierData>[] DamageModifiersTarget;
        /// <summary>
        /// List of buff status on uptimes + states \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsUptime> BuffUptimes;
        /// <summary>
        /// List of buff status on self generation  \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> SelfBuffs;
        /// <summary>
        /// List of buff status on group generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> GroupBuffs;
        /// <summary>
        /// List of buff status on off group generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> OffGroupBuffs;
        /// <summary>
        /// List of buff status on squad generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> SquadBuffs;
        /// <summary>
        /// List of buff status on uptimes + states on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsUptime> BuffUptimesActive;
        /// <summary>
        /// List of buff status on self generation on active time  \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> SelfBuffsActive;
        /// <summary>
        /// List of buff status on group generation on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> GroupBuffsActive;
        /// <summary>
        /// List of buff status on off group generation on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> OffGroupBuffsActive;
        /// <summary>
        /// List of buff status on squad generation on active time\n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsGeneration> SquadBuffsActive;
        /// <summary>
        /// List of death recaps \n
        /// Length == number of death
        /// </summary>
        /// <seealso cref="JsonDeathRecap"/>
        public List<JsonDeathRecap> DeathRecap;
        /// <summary>
        /// List of used consumables
        /// </summary>
        /// <seealso cref="JsonConsumable"/>
        public List<JsonConsumable> Consumables;
        /// <summary>
        /// List of time during which the player was active (not dead and not dc) \n
        /// Length == number of phases
        /// </summary>
        public List<long> ActiveTimes;
    }
}
