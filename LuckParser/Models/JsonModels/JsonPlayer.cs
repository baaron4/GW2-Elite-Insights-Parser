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
        public string Account;
        public int Group;
        public string Profession;
        public string[] Weapons;
        public JsonDPS[][] DpsTargets;
        public List<int>[][] TargetDamage1S;
        public Dictionary<string, JsonDamageDist>[][] TargetDamageDist;
        public JsonStatsAll[] StatsAll;
        public JsonStats[][] StatsTargets;
        public JsonDefenses[] Defenses;
        public JsonSupport[] Support;
        public Dictionary<string, List<JsonExtraBoonData>> DamageModifiers;
        public Dictionary<string, List<JsonExtraBoonData>>[] DamageModifiersTarget;
        public Dictionary<string, JsonBuffs> SelfBuffs;
        public Dictionary<string, JsonBuffs> GroupBuffs;
        public Dictionary<string, JsonBuffs> OffGroupBuffs;
        public Dictionary<string, JsonBuffs> SquadBuffs;
        public List<JsonDeathRecap> DeathRecap;
        public List<JsonConsumable> Consumables;
    }
}
