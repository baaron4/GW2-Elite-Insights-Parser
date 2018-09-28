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
        public JsonDps Dps;
        public double[] AvgBoons;
        public double[] AvgConditions;
        public uint HitboxHeight;
        public uint HitboxWidth;
        public List<JsonSkill> Rotation;
        public int[] TotalPersonalDamage;
        public Dictionary<string, JsonBossBuffs> Conditions;
    }
}
