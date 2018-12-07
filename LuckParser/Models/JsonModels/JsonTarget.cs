using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonTarget
    {
        public string name;
        public ushort id;
        public int totalHealth;
        public double finalHealth;
        public double healthPercentBurned;
        public int firstAware;
        public int lastAware;
        public JsonDps dps;
        public double[] avgBoons;
        public double[] avgConditions;
        public List<int[]> avgConditionsStates;
        public List<int[]> avgBoonsStates;
        public List<int> dps1s;
        public uint hitboxHeight;
        public uint hitboxWidth;
        public Dictionary<string, List<JsonSkill>> rotation;
        public List<JsonMinions> minions;
        public Dictionary<string, JsonTargetBuffs> buffs;
        public Dictionary<string, JsonDamageDist>[] totalDamageDist;
        public Dictionary<string, JsonDamageDist>[] totalDamageTaken;
    }
}
