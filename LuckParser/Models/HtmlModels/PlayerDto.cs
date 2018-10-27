using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class PlayerDto
    {
        [DataMember] public int group;
        [DataMember] public string name;
        [DataMember] public string acc;
        [DataMember] public string profession;
        [DataMember] public uint condi;
        [DataMember] public uint conc;
        [DataMember] public uint heal;
        [DataMember] public uint tough;
        [DataMember] public readonly List<MinionDto> minions = new List<MinionDto>();
        [DataMember] public List<string> firstSet;
        [DataMember] public List<string> secondSet;
        [DataMember] public string colTarget;
        [DataMember] public string colCleave;
        [DataMember] public string colTotal;
        [DataMember(EmitDefaultValue = false)] public bool isConjure;

        public PlayerDto() { }

        public PlayerDto(int group, string name, string acc, string profession)
        {
            this.group = group;
            this.name = name;
            this.acc = acc;
            this.profession = profession;
        }
    }
}
