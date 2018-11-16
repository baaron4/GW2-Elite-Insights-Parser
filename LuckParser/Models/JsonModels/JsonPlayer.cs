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
        public JsonDps[] DpsBoss;
        public List<int> Dps1s;
        public List<int>[] TargetDps1s;
        public Dictionary<long, JsonDamageDist>[] TotalDamageDist;
        public Dictionary<long, JsonDamageDist>[][] TargetDamageDist;
        public Dictionary<long, JsonDamageDist>[] TotalDamageTaken;
        public JsonStatsAll StatsAll;
        public JsonStats[] StatsBoss;
        public List<int[]> AvgConditionsStates;
        public List<int[]> AvgBoonsStates;
        public JsonDefenses Defenses;
        public JsonSupport Support;
        public Dictionary<long, List<JsonSkill>> Rotation;
        public Dictionary<long, JsonBuffs> SelfBuffs;
        public Dictionary<long, JsonBuffs> GroupBuffs;
        public Dictionary<long, JsonBuffs> OffGroupBuffs;
        public Dictionary<long, JsonBuffs> SquadBuffs;
        public List<JsonDeathRecap> DeathRecap;
        public List<JsonMinions> Minions;
        public List<JsonConsumable> Consumables;
    }
}
