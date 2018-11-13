using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonTarget
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
        public List<int[]> AvgConditionsStates;
        public List<int[]> AvgBoonsStates;
        public List<int> Dps1s;
        public uint HitboxHeight;
        public uint HitboxWidth;
        public Dictionary<long, List<JsonSkill>> Rotation;
        public List<JsonMinions> Minions;
        public Dictionary<long, JsonTargetBuffs> Buffs;
        public Dictionary<long, JsonDamageDist>[] TotalDamageDist;
        public Dictionary<long, JsonDamageDist>[] TotalDamageTaken;
    }
}
