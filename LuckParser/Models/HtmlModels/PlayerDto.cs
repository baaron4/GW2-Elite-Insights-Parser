using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{
    
    public class PlayerDto
    {
        public int group;
        public string name;
        public string acc;
        public string profession;
        public uint condi;
        public uint conc;
        public uint heal;
        public uint tough;
        public readonly List<MinionDto> minions = new List<MinionDto>();
        public List<string> firstSet;
        public List<string> secondSet;
        public string colTarget;
        public string colCleave;
        public string colTotal;
        public int isConjure;
    }
}
