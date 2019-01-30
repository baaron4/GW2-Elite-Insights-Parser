using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.JsonModels.JsonStatistics;

namespace LuckParser.Models.JsonModels
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
        /// Weapons of the player
        /// Length == 4 if wep swap, 2 otherwise
        /// When 2 handed weapon even indices will have "2Hand" as value
        /// </summary>
        public string[] Weapons;
        public JsonDPS[][] DpsTargets;
        public List<int>[][] TargetDamage1S;
        /// <summary>
        /// Per Target Damage distribution array
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[][] TargetDamageDist;
        public JsonStatsAll[] StatsAll;
        public JsonStats[][] StatsTargets;
        public JsonDefenses[] Defenses;
        public JsonSupport[] Support;
        public Dictionary<string, List<JsonBuffDamageModifierData>> DamageModifiers;
        public Dictionary<string, List<JsonBuffDamageModifierData>>[] DamageModifiersTarget;
        /// <summary>
        /// List of buff status on self (uptime + self generation)
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffs"/>
        public List<JsonBuffs> SelfBuffs;
        /// <summary>
        /// List of buff status on group generation
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffs"/>
        public List<JsonBuffs> GroupBuffs;
        /// <summary>
        /// List of buff status on off group generation
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffs"/>
        public List<JsonBuffs> OffGroupBuffs;
        /// <summary>
        /// List of buff status on squad generation
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonBuffs"/>
        public List<JsonBuffs> SquadBuffs;
        /// <summary>
        /// List of death recaps
        /// Length == number of death
        /// </summary>
        /// <seealso cref="JsonDeathRecap"/>
        public List<JsonDeathRecap> DeathRecap;
        /// <summary>
        /// List of used consumables
        /// </summary>
        /// <seealso cref="JsonConsumable"/>
        public List<JsonConsumable> Consumables;
    }
}
