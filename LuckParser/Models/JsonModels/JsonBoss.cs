using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonBoss
    {
        public string Name;
        public ushort Id;
        public int TotalHealth;
        public double FinalHealth;
        public double HealthPercentBurned;
        public int FirstAware;
        public int LastAware;
        public JsonDps Dps;
        public double[] AvgBoons;
        public double[] AvgConditions;
        public List<int> Dps1s;
        public uint HitboxHeight;
        public uint HitboxWidth;
        public Dictionary<long, List<JsonSkill>> Rotation;
        public int[] TotalPersonalDamage;
        public List<JsonMinions> Minions;
        public Dictionary<long, JsonBossBuffs> Conditions;
    }
}
