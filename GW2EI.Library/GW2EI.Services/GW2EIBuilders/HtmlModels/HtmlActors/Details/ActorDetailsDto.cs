using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class ActorDetailsDto
{
    public List<DamageDistributionDto>? DmgDistributions;
    public List<List<DamageDistributionDto>>? DmgDistributionsTargets;
    public List<DamageDistributionDto>? DmgDistributionsTaken;
    public List<List<SkillCastDto>>? Rotation;
    public List<List<BuffChartDataDto>>? BoonGraph;
    public List<List<List<BuffChartDataDto>>>? BoonGraphPerSource;
    public List<FoodDto>? Food;
    public List<DeathRecapDto>? DeathRecap;


    public static ActorDetailsDto BuildPlayerData(ParsedEvtcLog log, SingleActor actor, UsedHtmlData used)
    {
        var phases = log.LogData.GetPhases(log);
        var dto = new ActorDetailsDto
        {
            DmgDistributions = new(phases.Count),
            DmgDistributionsTargets = new(phases.Count),
            DmgDistributionsTaken = new(phases.Count),
            BoonGraph = new(phases.Count),
            Rotation = new(phases.Count),
            Food = FoodDto.BuildFoodData(log, actor, used),
            DeathRecap = DeathRecapDto.BuildDeathRecap(log, actor)
        };
        foreach (PhaseData phase in phases)
        {
            dto.Rotation.Add(SkillDto.BuildRotationData(log, actor, phase, used));
            dto.DmgDistributions.Add(DamageDistributionDto.BuildDamageDistributionData(log, actor, null, phase, used));
            var dmgTargetsDto = new List<DamageDistributionDto>(phase.Targets.Count);
            foreach (SingleActor target in phase.Targets.Keys)
            {
                dmgTargetsDto.Add(DamageDistributionDto.BuildDamageDistributionData(log, actor, target, phase, used));
            }
            dto.DmgDistributionsTargets.Add(dmgTargetsDto);
            dto.DmgDistributionsTaken.Add(DamageDistributionDto.BuildDamageTakenDistributionData(log, actor, phase, used));
            dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, actor, phase, used));
        }

        return dto;
    }

    static readonly List<List<BuffChartDataDto>> EmptyBuffChartList = [];
    static readonly List<SkillCastDto> EmptyRotationList = [];
    static readonly List<BuffChartDataDto> EmptyBoonGraphList = [];

    public static ActorDetailsDto BuildTargetData(ParsedEvtcLog log, SingleActor target, UsedHtmlData used, bool cr)
    {
        var phases = log.LogData.GetPhases(log);
        var dto = new ActorDetailsDto
        {
            DmgDistributions = new(phases.Count),
            DmgDistributionsTaken = new(phases.Count),
            BoonGraph = new(phases.Count),
            BoonGraphPerSource = new(phases.Count),
            Rotation = new(phases.Count),
        };

        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (phase.Targets.ContainsKey(target))
            {
                dto.DmgDistributions.Add(DamageDistributionDto.BuildDamageDistributionData(log, target, null, phase, used));
                dto.DmgDistributionsTaken.Add(DamageDistributionDto.BuildDamageTakenDistributionData(log, target, phase, used));
                dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, used));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, target, phase, used));

                var friendlies = log.Friendlies;
                var list = new List<List<BuffChartDataDto>>(friendlies.Count);
                foreach (var friendly in friendlies)
                {
                    list.Add(BuffChartDataDto.BuildBuffGraphData(log, target, friendly, phase, used));
                }
                dto.BoonGraphPerSource.Add(list);
            }
            // rotation + buff graph for CR
            else
            {
                if (cr && phase is PhaseDataWithMetaData && phase.IntersectsWindow(target.FirstAware, target.LastAware))
                {
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, used));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, target, phase, used));
                }
                else
                {
                    dto.Rotation.Add(EmptyRotationList);
                    dto.BoonGraph.Add(EmptyBoonGraphList);
                }

                dto.DmgDistributions.Add(DamageDistributionDto.EmptyInstance);
                dto.DmgDistributionsTaken.Add(DamageDistributionDto.EmptyInstance);
                dto.BoonGraphPerSource.Add(EmptyBuffChartList);
            }
        }

        return dto;
    }
}
