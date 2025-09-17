using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTHealing;

internal class EXTHealingStatsPlayerDetailsDto
{
    public List<EXTHealingStatsHealingDistributionDto>? HealingDistributions { get; set; }
    public List<List<EXTHealingStatsHealingDistributionDto>>? HealingDistributionsTargets { get; set; }
    public List<EXTHealingStatsHealingDistributionDto>? IncomingHealingDistributions { get; set; }
    public List<EXTHealingStatsPlayerDetailsDto>? Minions { get; set; }

    // helpers

    public static EXTHealingStatsPlayerDetailsDto BuildPlayerHealingData(ParsedEvtcLog log, SingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var phases = log.LogData.GetPhases(log);
        var minions = actor.GetMinions(log);
        var dto = new EXTHealingStatsPlayerDetailsDto
        {
            HealingDistributions = new (phases.Count),
            HealingDistributionsTargets = new (phases.Count),
            IncomingHealingDistributions = new (phases.Count),
            Minions = new (minions.Count),
        };
        foreach (PhaseData phase in phases)
        {
            dto.HealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyHealingDistData(log, actor, null, phase, usedSkills, usedBuffs));
            var dmgTargetsDto = new List<EXTHealingStatsHealingDistributionDto>(log.Friendlies.Count);
            foreach (SingleActor target in log.Friendlies)
            {
                dmgTargetsDto.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyHealingDistData(log, actor, target, phase, usedSkills, usedBuffs));
            }
            dto.HealingDistributionsTargets.Add(dmgTargetsDto);
            dto.IncomingHealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildIncomingHealingDistData(log, actor, phase, usedSkills, usedBuffs));
        }
        foreach (var minion in minions)
        {
            dto.Minions.Add(BuildFriendlyMinionsHealingData(log, actor, minion, usedSkills, usedBuffs));
        }

        return dto;
    }

    private static EXTHealingStatsPlayerDetailsDto BuildFriendlyMinionsHealingData(ParsedEvtcLog log, SingleActor actor, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var phases = log.LogData.GetPhases(log);
        var dto = new EXTHealingStatsPlayerDetailsDto
        {
            HealingDistributions = new (phases.Count),
            HealingDistributionsTargets = new(phases.Count),
            IncomingHealingDistributions = new(phases.Count)
        };
        foreach (PhaseData phase in phases)
        {
            var dmgTargetsDto = new List<EXTHealingStatsHealingDistributionDto>(log.Friendlies.Count);
            foreach (SingleActor target in log.Friendlies)
            {
                dmgTargetsDto.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionHealingDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
            }
            dto.HealingDistributionsTargets.Add(dmgTargetsDto);
            dto.HealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionHealingDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
            dto.IncomingHealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionIncomingHealingDistData(log, minion, null, phase, usedSkills, usedBuffs));
        }
        return dto;
    }
}
