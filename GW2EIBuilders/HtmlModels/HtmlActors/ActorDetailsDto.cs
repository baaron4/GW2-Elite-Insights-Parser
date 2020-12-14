using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class ActorDetailsDto
    {
        public List<DmgDistributionDto> DmgDistributions { get; set; }
        public List<List<DmgDistributionDto>> DmgDistributionsTargets { get; set; }
        public List<DmgDistributionDto> DmgDistributionsTaken { get; set; }
        public List<List<object[]>> Rotation { get; set; }
        public List<List<BuffChartDataDto>> BoonGraph { get; set; }
        public List<FoodDto> Food { get; set; }
        public List<ActorDetailsDto> Minions { get; set; }
        public List<DeathRecapDto> DeathRecap { get; set; }

        // helpers

        public static ActorDetailsDto BuildPlayerData(ParsedEvtcLog log, Player player, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Food = FoodDto.BuildPlayerFoodData(log, player, usedBuffs),
                Minions = new List<ActorDetailsDto>(),
                DeathRecap = DeathRecapDto.BuildDeathRecap(log, player)
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                dto.Rotation.Add(SkillDto.BuildRotationData(log, player, phase, usedSkills));
                dto.DmgDistributions.Add(DmgDistributionDto.BuildPlayerDMGDistData(log, player, null, phase, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (NPC target in phase.Targets)
                {
                    dmgTargetsDto.Add(DmgDistributionDto.BuildPlayerDMGDistData(log, player, target, phase, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributionsTaken.Add(DmgDistributionDto.BuildDMGTakenDistData(log, player, phase, usedSkills, usedBuffs));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, player, phase, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in player.GetMinions(log))
            {
                dto.Minions.Add(BuildPlayerMinionsData(log, player, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static ActorDetailsDto BuildPlayerMinionsData(ParsedEvtcLog log, Player player, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (NPC target in phase.Targets)
                {
                    dmgTargetsDto.Add(DmgDistributionDto.BuildPlayerMinionDMGDistData(log, player, minion, target, phase, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributions.Add(DmgDistributionDto.BuildPlayerMinionDMGDistData(log, player, minion, null, phase, usedSkills, usedBuffs));
            }
            return dto;
        }

        public static ActorDetailsDto BuildTargetData(ParsedEvtcLog log, NPC target, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, bool cr)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>()
            };
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (phase.Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(DmgDistributionDto.BuildTargetDMGDistData(log, target, phase, usedSkills, usedBuffs));
                    dto.DmgDistributionsTaken.Add(DmgDistributionDto.BuildDMGTakenDistData(log, target, phase, usedSkills, usedBuffs));
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, phase, usedBuffs));
                }
                // rotation + buff graph for CR
                else if (i == 0 && cr)
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, phase, usedBuffs));
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BuffChartDataDto>());
                }
            }

            dto.Minions = new List<ActorDetailsDto>();
            foreach (KeyValuePair<long, Minions> pair in target.GetMinions(log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(log, target, pair.Value, usedSkills, usedBuffs));
            }
            return dto;
        }

        private static ActorDetailsDto BuildTargetsMinionsData(ParsedEvtcLog log, NPC target, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                if (phase.Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(DmgDistributionDto.BuildTargetMinionDMGDistData(log, target, minion, phase, usedSkills, usedBuffs));
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                }
            }
            return dto;
        }
    }
}
