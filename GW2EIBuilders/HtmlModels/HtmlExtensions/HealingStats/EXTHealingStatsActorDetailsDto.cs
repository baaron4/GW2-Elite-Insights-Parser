using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class EXTHealingStatsActorDetailsDto
    {
        public List<EXTHealingStatsHealingDistributionDto> healingDistributions { get; set; }
        public List<List<EXTHealingStatsHealingDistributionDto>> healingDistributionsFriendlies { get; set; }
        public List<EXTHealingStatsHealingDistributionDto> IncomingHealingDistributions { get; set; }
        public List<EXTHealingStatsActorDetailsDto> Minions { get; set; }

        // helpers

        public static EXTHealingStatsActorDetailsDto BuildPlayerData(ParsedEvtcLog log, AbstractSingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTHealingStatsActorDetailsDto
            {
                healingDistributions = new List<EXTHealingStatsHealingDistributionDto>(),
                healingDistributionsFriendlies = new List<List<EXTHealingStatsHealingDistributionDto>>(),
                IncomingHealingDistributions = new List<EXTHealingStatsHealingDistributionDto>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                dto.healingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyHealingDistData(log, actor, null, phase, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<EXTHealingStatsHealingDistributionDto>();
                foreach (AbstractSingleActor target in log.Friendlies)
                {
                    dmgTargetsDto.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyHealingDistData(log, actor, target, phase, usedSkills, usedBuffs));
                }
                dto.healingDistributionsFriendlies.Add(dmgTargetsDto);
                dto.IncomingHealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildIncomingHealingDistData(log, actor, phase, usedSkills, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in actor.GetMinions(log))
            {
                dto.Minions.Add(BuildFriendlyMinionsData(log, actor, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static EXTHealingStatsActorDetailsDto BuildFriendlyMinionsData(ParsedEvtcLog log, AbstractSingleActor actor, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTHealingStatsActorDetailsDto
            {
                healingDistributions = new List<EXTHealingStatsHealingDistributionDto>(),
                healingDistributionsFriendlies = new List<List<EXTHealingStatsHealingDistributionDto>>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var dmgTargetsDto = new List<EXTHealingStatsHealingDistributionDto>();
                foreach (AbstractSingleActor target in log.Friendlies)
                {
                    dmgTargetsDto.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionHealingDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
                }
                dto.healingDistributionsFriendlies.Add(dmgTargetsDto);
                dto.healingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionHealingDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
            }
            return dto;
        }
    }
}
