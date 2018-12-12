using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonPlayer
    {      
        public string character;
        public string account;
        public uint condition;
        public uint concentration;
        public uint healing;
        public uint toughness;
        public int group;
        public string profession;
        public string[] weapons;
        public JsonDps dpsAll;
        public JsonDps[] dpsTargets;
        public List<int>[] damage1S;
        public List<int>[][] targetDamage1S;
        public Dictionary<string, JsonDamageDist>[] totalDamageDist;
        public Dictionary<string, JsonDamageDist>[][] targetDamageDist;
        public Dictionary<string, JsonDamageDist>[] totalDamageTaken;
        public JsonStatsAll statsAll;
        public JsonStats[] statsTargets;
        public List<int[]> avgConditionsStates;
        public List<int[]> avgBoonsStates;
        public JsonDefenses defenses;
        public JsonSupport support;
        public Dictionary<string, List<JsonSkill>> totation;
        public Dictionary<string, JsonBuffs> selfBuffs;
        public Dictionary<string, JsonBuffs> groupBuffs;
        public Dictionary<string, JsonBuffs> offGroupBuffs;
        public Dictionary<string, JsonBuffs> squadBuffs;
        public List<ParseModels.Player.DeathRecap> deathRecap;
        public List<JsonMinions> minions;
        public List<JsonConsumable> consumables;
    }
}
