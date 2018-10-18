using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class LogDataDto
    {
        [DataMember]
        public List<TargetDto> targets = new List<TargetDto>();
        [DataMember(Order = 0)]
        public readonly List<PlayerDto> players = new List<PlayerDto>();
        [DataMember(Order = 1)]
        public readonly List<EnemyDto> enemies = new List<EnemyDto>();
        [DataMember(Order = 2)]
        public readonly List<GraphDto> graphs = new List<GraphDto>();
        [DataMember(Order = 3)]
        public readonly List<PhaseDto> phases = new List<PhaseDto>();
        [DataMember(Order = 4)]
        public List<long> boons;
        [DataMember(Order = 5)]
        public List<long> offBuffs;
        [DataMember(Order = 6)]
        public List<long> defBuffs;
        [DataMember(Order = 10)]
        public Dictionary<string, List<long>> persBuffs;
        [DataMember]
        public List<long> conditions;
        [DataMember(Order = 7)]
        public List<MechanicDto> mechanics;
        [DataMember(Order = 8)]
        public LogFlags flags = new LogFlags();
    }

    [DataContract]
    public class LogFlags
    {
        [DataMember]
        public bool simpleRotation;
        [DataMember]
        public bool dark;
        [DataMember]
        public bool combatReplay;
    }
}
