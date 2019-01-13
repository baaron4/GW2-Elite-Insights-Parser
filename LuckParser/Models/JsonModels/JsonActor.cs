using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public abstract class JsonActor
    {

        public string Name;
        public uint Condition;
        public uint Concentration;
        public uint Healing;
        public uint Toughness;
        public uint HitboxHeight;
        public uint HitboxWidth;
        public List<JsonMinions> Minions;

        public Statistics.FinalDPS[] DpsAll;
        public Dictionary<string, JsonDamageDist>[] TotalDamageDist;
        public Dictionary<string, JsonDamageDist>[] TotalDamageTaken;
        public Dictionary<string, List<JsonSkill>> Rotation;
        public List<int>[] Damage1S;
        public List<int[]> AvgConditionsStates;
        public List<int[]> AvgBoonsStates;
    }
}
