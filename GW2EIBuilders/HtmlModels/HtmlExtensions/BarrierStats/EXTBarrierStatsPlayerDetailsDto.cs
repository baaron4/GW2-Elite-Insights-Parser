using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTBarrier
{
    internal class EXTBarrierStatsPlayerDetailsDto
    {
        public List<EXTBarrierStatsBarrierDistributionDto> BarrierDistributions { get; set; }
        public List<List<EXTBarrierStatsBarrierDistributionDto>> BarrierDistributionsTargets { get; set; }
        public List<EXTBarrierStatsBarrierDistributionDto> IncomingBarrierDistributions { get; set; }
        public List<EXTBarrierStatsPlayerDetailsDto> Minions { get; set; }

        // helpers

        public static EXTBarrierStatsPlayerDetailsDto BuildPlayerBarrierData(ParsedEvtcLog log, AbstractSingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTBarrierStatsPlayerDetailsDto
            {
                BarrierDistributions = new List<EXTBarrierStatsBarrierDistributionDto>(),
                BarrierDistributionsTargets = new List<List<EXTBarrierStatsBarrierDistributionDto>>(),
                IncomingBarrierDistributions = new List<EXTBarrierStatsBarrierDistributionDto>(),
                Minions = new List<EXTBarrierStatsPlayerDetailsDto>(),
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                dto.BarrierDistributions.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyBarrierDistData(log, actor, null, phase, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<EXTBarrierStatsBarrierDistributionDto>();
                foreach (AbstractSingleActor target in log.Friendlies)
                {
                    dmgTargetsDto.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyBarrierDistData(log, actor, target, phase, usedSkills, usedBuffs));
                }
                dto.BarrierDistributionsTargets.Add(dmgTargetsDto);
                dto.IncomingBarrierDistributions.Add(EXTBarrierStatsBarrierDistributionDto.BuildIncomingBarrierDistData(log, actor, phase, usedSkills, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in actor.GetMinions(log))
            {
                dto.Minions.Add(BuildFriendlyMinionsHealingData(log, actor, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static EXTBarrierStatsPlayerDetailsDto BuildFriendlyMinionsHealingData(ParsedEvtcLog log, AbstractSingleActor actor, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTBarrierStatsPlayerDetailsDto
            {
                BarrierDistributions = new List<EXTBarrierStatsBarrierDistributionDto>(),
                BarrierDistributionsTargets = new List<List<EXTBarrierStatsBarrierDistributionDto>>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var dmgTargetsDto = new List<EXTBarrierStatsBarrierDistributionDto>();
                foreach (AbstractSingleActor target in log.Friendlies)
                {
                    dmgTargetsDto.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyMinionBarrierDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
                }
                dto.BarrierDistributionsTargets.Add(dmgTargetsDto);
                dto.BarrierDistributions.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyMinionBarrierDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
            }
            return dto;
        }
    }
}
