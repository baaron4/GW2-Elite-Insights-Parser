using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTBarrier;

internal class EXTBarrierStatsPlayerDetailsDto
{
    public List<EXTBarrierStatsBarrierDistributionDto>? BarrierDistributions { get; set; }
    public List<List<EXTBarrierStatsBarrierDistributionDto>>? BarrierDistributionsTargets { get; set; }
    public List<EXTBarrierStatsBarrierDistributionDto>? IncomingBarrierDistributions { get; set; }
    public List<EXTBarrierStatsPlayerDetailsDto>? Minions { get; set; }

    // helpers

    public static EXTBarrierStatsPlayerDetailsDto BuildPlayerBarrierData(ParsedEvtcLog log, SingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var phases = log.LogData.GetPhases(log);
        var minions = actor.GetMinions(log);
        var dto = new EXTBarrierStatsPlayerDetailsDto
        {
            BarrierDistributions = new (phases.Count),
            BarrierDistributionsTargets = new (phases.Count),
            IncomingBarrierDistributions = new (phases.Count),
            Minions = new (minions.Count),
        };
        foreach (PhaseData phase in phases)
        {
            dto.BarrierDistributions.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyBarrierDistData(log, actor, null, phase, usedSkills, usedBuffs));
            var dmgTargetsDto = new List<EXTBarrierStatsBarrierDistributionDto>(log.Friendlies.Count);
            foreach (SingleActor target in log.Friendlies)
            {
                dmgTargetsDto.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyBarrierDistData(log, actor, target, phase, usedSkills, usedBuffs));
            }
            dto.BarrierDistributionsTargets.Add(dmgTargetsDto);
            dto.IncomingBarrierDistributions.Add(EXTBarrierStatsBarrierDistributionDto.BuildIncomingBarrierDistData(log, actor, phase, usedSkills, usedBuffs));
        }
        foreach (var minion in minions)
        {
            dto.Minions.Add(BuildFriendlyMinionsHealingData(log, actor, minion, usedSkills, usedBuffs));
        }

        return dto;
    }

    private static EXTBarrierStatsPlayerDetailsDto BuildFriendlyMinionsHealingData(ParsedEvtcLog log, SingleActor actor, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var phases = log.LogData.GetPhases(log);
        var dto = new EXTBarrierStatsPlayerDetailsDto
        {
            BarrierDistributions = new (phases.Count),
            BarrierDistributionsTargets = new (phases.Count),
            IncomingBarrierDistributions = new(phases.Count)
        };
        foreach (PhaseData phase in phases)
        {
            var dmgTargetsDto = new List<EXTBarrierStatsBarrierDistributionDto>(log.Friendlies.Count);
            foreach (SingleActor target in log.Friendlies)
            {
                dmgTargetsDto.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyMinionBarrierDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
            }
            dto.BarrierDistributionsTargets.Add(dmgTargetsDto);
            dto.BarrierDistributions.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyMinionBarrierDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
            dto.IncomingBarrierDistributions.Add(EXTBarrierStatsBarrierDistributionDto.BuildFriendlyMinionIncomingBarrierDistData(log, minion, null, phase, usedSkills, usedBuffs));
        }
        return dto;
    }
}
