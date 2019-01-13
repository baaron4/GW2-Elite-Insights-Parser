using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonTarget : JsonActor
    {
        public ushort Id;
        public int TotalHealth;
        public double FinalHealth;
        public double HealthPercentBurned;
        public int FirstAware;
        public int LastAware;
        public double[] AvgBoons;
        public double[] AvgConditions;
        public Dictionary<string, JsonTargetBuffs> Buffs;
    }
}
