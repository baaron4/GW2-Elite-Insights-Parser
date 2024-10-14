using System.Collections.Generic;
using System.Linq;
using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLActors
{
    internal class ActorDetailsDto
    {
        public List<DmgDistributionDto>? DmgDistributions { get; set; }
        public List<List<DmgDistributionDto>>? DmgDistributionsTargets { get; set; }
        public List<DmgDistributionDto>? DmgDistributionsTaken { get; set; }
        public List<List<object[]>>? Rotation { get; set; }
        public List<List<BuffChartDataDto>>? BoonGraph { get; set; }
        public List<List<List<BuffChartDataDto>>>? BoonGraphPerSource { get; set; }
        public List<FoodDto>? Food { get; set; }
        public List<ActorDetailsDto>? Minions { get; set; }
        public List<DeathRecapDto>? DeathRecap { get; set; }

        // helpers

        //TODO(Rennorb) @perf: init capacity for usedBuffs
        public static ActorDetailsDto BuildPlayerData(ParsedEvtcLog log, AbstractSingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var phases = log.FightData.GetPhases(log);
            var minions = actor.GetMinions(log);
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(phases.Count),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>(phases.Count),
                DmgDistributionsTaken = new List<DmgDistributionDto>(phases.Count),
                BoonGraph = new List<List<BuffChartDataDto>>(phases.Count),
                Rotation = new List<List<object[]>>(phases.Count),
                Food = FoodDto.BuildFoodData(log, actor, usedBuffs),
                Minions = new List<ActorDetailsDto>(minions.Count),
                DeathRecap = DeathRecapDto.BuildDeathRecap(log, actor)
            };
            foreach (PhaseData phase in phases)
            {
                dto.Rotation.Add(SkillDto.BuildRotationData(log, actor, phase, usedSkills));
                dto.DmgDistributions.Add(DmgDistributionDto.BuildFriendlyDMGDistData(log, actor, null, phase, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<DmgDistributionDto>(phase.AllTargets.Count);
                foreach (AbstractSingleActor target in phase.AllTargets)
                {
                    dmgTargetsDto.Add(DmgDistributionDto.BuildFriendlyDMGDistData(log, actor, target, phase, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributionsTaken.Add(DmgDistributionDto.BuildDMGTakenDistData(log, actor, phase, usedSkills, usedBuffs));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, actor, phase, usedBuffs));
            }
            foreach (var minion in minions.Values)
            {
                dto.Minions.Add(BuildFriendlyMinionsData(log, actor, minion, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static ActorDetailsDto BuildFriendlyMinionsData(ParsedEvtcLog log, AbstractSingleActor actor, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var phases = log.FightData.GetPhases(log);
            var dto = new ActorDetailsDto
            {
                DmgDistributions        = new (phases.Count),
                DmgDistributionsTargets = new (phases.Count),
            };

            foreach (PhaseData phase in phases)
            {
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (AbstractSingleActor target in phase.AllTargets)
                {
                    dmgTargetsDto.Add(DmgDistributionDto.BuildFriendlyMinionDMGDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributions.Add(DmgDistributionDto.BuildFriendlyMinionDMGDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
            }
            return dto;
        }

        public static ActorDetailsDto BuildTargetData(ParsedEvtcLog log, AbstractSingleActor target, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, bool cr)
        {
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            var dto = new ActorDetailsDto
            {
                DmgDistributions      = new(phases.Count),
                DmgDistributionsTaken = new(phases.Count),
                BoonGraph             = new(phases.Count),
                BoonGraphPerSource    = new(phases.Count),
                Rotation              = new(phases.Count),
            };

            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (phase.AllTargets.Contains(target))
                {
                    dto.DmgDistributions.Add(DmgDistributionDto.BuildTargetDMGDistData(log, target, phase, usedSkills, usedBuffs));
                    dto.DmgDistributionsTaken.Add(DmgDistributionDto.BuildDMGTakenDistData(log, target, phase, usedSkills, usedBuffs));
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, target, phase, usedBuffs));
                    dto.BoonGraphPerSource.Add(log.Friendlies.Select(p => BuffChartDataDto.BuildBuffGraphData(log, target, p, phase, usedBuffs)).ToList());
                }
                // rotation + buff graph for CR
                else if (i == 0 && cr)
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, target, phase, usedBuffs));
                    dto.BoonGraphPerSource.Add(new List<List<BuffChartDataDto>>());
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BuffChartDataDto>());
                    dto.BoonGraphPerSource.Add(new List<List<BuffChartDataDto>>());
                }
            }

            dto.Minions = new List<ActorDetailsDto>();
            foreach (KeyValuePair<long, Minions> pair in target.GetMinions(log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(log, target, pair.Value, usedSkills, usedBuffs));
            }
            return dto;
        }

        private static ActorDetailsDto BuildTargetsMinionsData(ParsedEvtcLog log, AbstractSingleActor target, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                if (phase.AllTargets.Contains(target))
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
