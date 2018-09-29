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
        public int[] TotalPersonalDamage;
        public int[][] TargetPersonalDamage;
        public JsonStatsAll StatsAll;
        public JsonStats[] StatsBoss;
        public JsonDefenses Defenses;
        public JsonSupport Support;
        public List<JsonSkill> Rotation;
        public Dictionary<string, JsonBuffs> SelfBoons;
        public Dictionary<string, JsonBuffs> GroupBoons;
        public Dictionary<string, JsonBuffs> OffGroupBoons;
        public Dictionary<string, JsonBuffs> SquadBoons;
        public List<JsonMinions> Minions;
    }
}
