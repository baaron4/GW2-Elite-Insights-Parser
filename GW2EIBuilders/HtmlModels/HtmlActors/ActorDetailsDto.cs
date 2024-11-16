using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class ActorDetailsDto
{
    public List<DmgDistributionDto>?           DmgDistributions;
    public List<List<DmgDistributionDto>>?     DmgDistributionsTargets;
    public List<DmgDistributionDto>?           DmgDistributionsTaken;
    public List<List<SkillCastDto>>?           Rotation;
    public List<List<BuffChartDataDto>>?       BoonGraph;
    public List<List<List<BuffChartDataDto>>>? BoonGraphPerSource;
    public List<FoodDto>?                      Food;
    public List<ActorDetailsDto>?              Minions;
    public List<DeathRecapDto>?                DeathRecap;


    public static ActorDetailsDto BuildPlayerData(ParsedEvtcLog log, AbstractSingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var phases = log.FightData.GetPhases(log);
        var minions = actor.GetMinions(log);
        var dto = new ActorDetailsDto
        {
            DmgDistributions        = new(phases.Count),
            DmgDistributionsTargets = new(phases.Count),
            DmgDistributionsTaken   = new(phases.Count),
            BoonGraph               = new(phases.Count),
            Rotation                = new(phases.Count),
            Food                    = FoodDto.BuildFoodData(log, actor, usedBuffs),
            Minions                 = new(minions.Count),
            DeathRecap              = DeathRecapDto.BuildDeathRecap(log, actor)
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
            DmgDistributions        = new(phases.Count),
            DmgDistributionsTargets = new(phases.Count),
        };

        foreach (PhaseData phase in phases)
        {
            var allTargets = phase.AllTargets;
            var dmgTargetsDto = new List<DmgDistributionDto>(allTargets.Count);
            foreach (AbstractSingleActor target in allTargets)
            {
                dmgTargetsDto.Add(DmgDistributionDto.BuildFriendlyMinionDMGDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
            }
            dto.DmgDistributionsTargets.Add(dmgTargetsDto);
            dto.DmgDistributions.Add(DmgDistributionDto.BuildFriendlyMinionDMGDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
        }
        return dto;
    }

    static readonly List<List<BuffChartDataDto>> EmptyBuffChartList = [];
    static readonly List<SkillCastDto>           EmptyRotationList  = [];
    static readonly List<BuffChartDataDto>       EmptyBoonGraphList = [];

    public static ActorDetailsDto BuildTargetData(ParsedEvtcLog log, AbstractSingleActor target, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, bool cr)
    {
        var phases = log.FightData.GetPhases(log);
        var minions = target.GetMinions(log);
        var dto = new ActorDetailsDto
        {
            DmgDistributions      = new(phases.Count),
            DmgDistributionsTaken = new(phases.Count),
            BoonGraph             = new(phases.Count),
            BoonGraphPerSource    = new(phases.Count),
            Rotation              = new(phases.Count),
            Minions               = new(minions.Count),
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

                var friendlies = log.Friendlies;
                var list = new List<List<BuffChartDataDto>>(friendlies.Count);
                foreach(var friendly in friendlies)
                {
                    list.Add(BuffChartDataDto.BuildBuffGraphData(log, target, friendly, phase, usedBuffs));
                }
                dto.BoonGraphPerSource.Add(list);
            }
            // rotation + buff graph for CR
            else
            {
                if (i == 0 && cr)
                {
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, target, phase, usedBuffs));
                }
                else
                {
                    dto.Rotation.Add(EmptyRotationList);
                    dto.BoonGraph.Add(EmptyBoonGraphList);
                }

                dto.DmgDistributions.Add(DmgDistributionDto.EmptyInstance);
                dto.DmgDistributionsTaken.Add(DmgDistributionDto.EmptyInstance);
                dto.BoonGraphPerSource.Add(EmptyBuffChartList);
            }
        }

        foreach (var minion in minions.Values)
        {
            var dmgDistributions = new List<DmgDistributionDto>(phases.Count);

            foreach (PhaseData phase in phases)
            {
                if (phase.AllTargets.Contains(target))
                {
                    dmgDistributions.Add(DmgDistributionDto.BuildTargetMinionDMGDistData(log, target, minion, phase, usedSkills, usedBuffs));
                }
                else
                {
                    dmgDistributions.Add(DmgDistributionDto.EmptyInstance);
                }
            }

            dto.Minions.Add(new()
            {
                DmgDistributions = dmgDistributions
            });
        }


        return dto;
    }
}
