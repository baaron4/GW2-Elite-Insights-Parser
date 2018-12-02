using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonPlayer
    {      
        public string Character;
        public string Account;
        public uint Condition;
        public uint Concentration;
        public uint Healing;
        public uint Toughness;
        public int Group;
        public string Profession;
        public string[] Weapons;
        public JsonDps DpsAll;
        public JsonDps[] DpsTargets;
        public List<int> Dps1s;
        public List<int>[] TargetDps1s;
        public Dictionary<string, JsonDamageDist>[] TotalDamageDist;
        public Dictionary<string, JsonDamageDist>[][] TargetDamageDist;
        public Dictionary<string, JsonDamageDist>[] TotalDamageTaken;
        public JsonStatsAll StatsAll;
        public JsonStats[] StatsTargets;
        public List<int[]> AvgConditionsStates;
        public List<int[]> AvgBoonsStates;
        public JsonDefenses Defenses;
        public JsonSupport Support;
        public Dictionary<string, List<JsonSkill>> Rotation;
        public Dictionary<string, JsonBuffs> SelfBuffs;
        public Dictionary<string, JsonBuffs> GroupBuffs;
        public Dictionary<string, JsonBuffs> OffGroupBuffs;
        public Dictionary<string, JsonBuffs> SquadBuffs;
        public List<ParseModels.Player.DeathRecap> DeathRecap;
        public List<JsonMinions> Minions;
        public List<JsonConsumable> Consumables;
    }
}
