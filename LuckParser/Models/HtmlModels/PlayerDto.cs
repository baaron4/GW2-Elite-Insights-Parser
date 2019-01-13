using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{
    
    public class PlayerDto
    {
        public int Group;
        public int CombatReplayID;
        public string Name;
        public string Acc;
        public string Profession;
        public uint Condi;
        public uint Conc;
        public uint Heal;
        public uint Tough;
        public readonly List<MinionDto> Minions = new List<MinionDto>();
        public List<string> FirstSet;
        public List<string> SecondSet;
        public string ColTarget;
        public string ColCleave;
        public string ColTotal;
        public bool IsConjure;
    }
}
