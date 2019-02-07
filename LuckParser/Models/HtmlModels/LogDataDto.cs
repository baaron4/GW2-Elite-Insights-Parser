using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{   
    public class LogDataDto
    { 
        public List<TargetDto> Targets = new List<TargetDto>();
        public readonly List<PlayerDto> Players = new List<PlayerDto>();
        public readonly List<EnemyDto> Enemies = new List<EnemyDto>();
        public readonly List<PhaseDto> Phases = new List<PhaseDto>();
        public List<long> Boons;
        public List<long> OffBuffs;
        public List<long> DefBuffs;
        public List<long> DmgCommonModifiersBuffs;
        public Dictionary<string, List<long>> PersBuffs;
        
        public List<long> Conditions;
        public string EncounterDuration;
        public bool Success;
        public string FightName;
        public string FightIcon;
        public bool LightTheme;
        public bool NoMechanics;
        public bool SingleGroup;
    }
}
