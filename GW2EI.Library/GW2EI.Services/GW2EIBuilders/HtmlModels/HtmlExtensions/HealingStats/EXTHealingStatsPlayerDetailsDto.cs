using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.EXTHealing;

internal class EXTHealingStatsPlayerDetailsDto
{
    public List<EXTHealingStatsHealingDistributionDto>? HealingDistributions { get; set; }
    public List<List<EXTHealingStatsHealingDistributionDto>>? HealingDistributionsTargets { get; set; }
    public List<EXTHealingStatsHealingDistributionDto>? IncomingHealingDistributions { get; set; }
    public List<EXTHealingStatsPlayerDetailsDto>? Minions { get; set; }

    // helpers

    public static EXTHealingStatsPlayerDetailsDto BuildPlayerHealingData(ParsedEvtcLog log, SingleActor actor, UsedData used)
    {
        var phases = log.LogData.GetPhases(log);
        var minions = actor.GetMinions(log);
        var dto = new EXTHealingStatsPlayerDetailsDto
        {
            HealingDistributions = new(phases.Count),
            HealingDistributionsTargets = new(phases.Count),
            IncomingHealingDistributions = new(phases.Count),
            Minions = new(minions.Count),
        };
        foreach (PhaseData phase in phases)
        {
            dto.HealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyHealingDistData(log, actor, null, phase, used));
            var dmgTargetsDto = new List<EXTHealingStatsHealingDistributionDto>(log.Friendlies.Count);
            foreach (SingleActor target in log.Friendlies)
            {
                dmgTargetsDto.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyHealingDistData(log, actor, target, phase, used));
            }
            dto.HealingDistributionsTargets.Add(dmgTargetsDto);
            dto.IncomingHealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildIncomingHealingDistData(log, actor, phase, used));
        }
        foreach (var minion in minions)
        {
            dto.Minions.Add(BuildFriendlyMinionsHealingData(log, actor, minion, used));
        }

        return dto;
    }

    private static EXTHealingStatsPlayerDetailsDto BuildFriendlyMinionsHealingData(ParsedEvtcLog log, SingleActor actor, Minions minion, UsedData used)
    {
        var phases = log.LogData.GetPhases(log);
        var dto = new EXTHealingStatsPlayerDetailsDto
        {
            HealingDistributions = new(phases.Count),
            HealingDistributionsTargets = new(phases.Count),
            IncomingHealingDistributions = new(phases.Count)
        };
        foreach (PhaseData phase in phases)
        {
            var dmgTargetsDto = new List<EXTHealingStatsHealingDistributionDto>(log.Friendlies.Count);
            foreach (SingleActor target in log.Friendlies)
            {
                dmgTargetsDto.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionHealingDistData(log, actor, minion, target, phase, used));
            }
            dto.HealingDistributionsTargets.Add(dmgTargetsDto);
            dto.HealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionHealingDistData(log, actor, minion, null, phase, used));
            dto.IncomingHealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionIncomingHealingDistData(log, minion, null, phase, used));
        }
        return dto;
    }
}
