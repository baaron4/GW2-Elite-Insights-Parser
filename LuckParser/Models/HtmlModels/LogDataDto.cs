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
        [DataMember] public string encounterDuration;
        [DataMember] public bool success;
        [DataMember] public string fightName;
        [DataMember] public string fightIcon;
    }
}
