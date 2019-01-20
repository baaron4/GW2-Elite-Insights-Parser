using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonPlayer : JsonActor
    {      
        public string Account;
        public int Group;
        public string Profession;
        public string[] Weapons;
        public Statistics.FinalDPS[][] DpsTargets;
        public List<int>[][] TargetDamage1S;
        public Dictionary<string, JsonDamageDist>[][] TargetDamageDist;
        public Statistics.FinalStatsAll[] StatsAll;
        public Statistics.FinalStats[][] StatsTargets;
        public Statistics.FinalDefenses[] Defenses;
        public Statistics.FinalSupport[] Support;
        public Dictionary<string, List<AbstractMasterActor.ExtraBoonData>> DamageModifiers;
        public Dictionary<string, List<AbstractMasterActor.ExtraBoonData>>[] DamageModifiersTarget;
        public Dictionary<string, JsonBuffs> SelfBuffs;
        public Dictionary<string, JsonBuffs> GroupBuffs;
        public Dictionary<string, JsonBuffs> OffGroupBuffs;
        public Dictionary<string, JsonBuffs> SquadBuffs;
        public List<Player.DeathRecap> DeathRecap;
        public List<Player.Consumable> Consumables;
    }
}
