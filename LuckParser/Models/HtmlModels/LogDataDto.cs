using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class LogDataDto
    {
        [DataMember(Order = 0)]
        public readonly List<PlayerDto> players = new List<PlayerDto>();
        [DataMember(Order = 1)]
        public readonly List<GraphDto> graphs = new List<GraphDto>();
        [DataMember(Order = 2)]
        public readonly List<PhaseDto> phases = new List<PhaseDto>();
        [DataMember]
        public List<BoonDto> boons;
        [DataMember]
        public List<BoonDto> offBuffs;
        [DataMember]
        public List<BoonDto> defBuffs;
        [DataMember]
        public List<MechanicDto> mechanics;
        [DataMember]
        public List<SkillDto> skills;
        [DataMember]
        public LogFlags flags = new LogFlags();
    }

    [DataContract]
    public class LogFlags
    {
        [DataMember]
        public bool simpleRotation;
        [DataMember]
        public bool dark;
    }
}
