using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionDetailsDto : ActorDetailsDto
{

    public static MinionDetailsDto BuildFriendlyMinionsData(ParsedEvtcLog log, Minions minions, UsedHtmlData used)
    {
        var phases = log.LogData.GetPhases(log);
        var dto = new MinionDetailsDto
        {
            DmgDistributions = new(phases.Count),
            DmgDistributionsTargets = new(phases.Count),
            DmgDistributionsTaken = new(phases.Count),
        };

        foreach (PhaseData phase in phases)
        {
            var allTargets = phase.Targets;
            var dmgTargetsDto = new List<DamageDistributionDto>(allTargets.Count);
            foreach (SingleActor target in allTargets.Keys)
            {
                dmgTargetsDto.Add(DamageDistributionDto.BuildMinionsDamageDistributionData(log, minions, target, phase, used));
            }
            dto.DmgDistributionsTargets.Add(dmgTargetsDto);
            dto.DmgDistributions.Add(DamageDistributionDto.BuildMinionsDamageDistributionData(log, minions, null, phase, used));
            dto.DmgDistributionsTaken.Add(DamageDistributionDto.BuildMinionsDamageTakenDistributionData(log, minions, phase, used));
        }
        return dto;
    }

    public static MinionDetailsDto BuildTargetMinionsData(ParsedEvtcLog log, Minions minions, UsedHtmlData used)
    {
        var master = minions.Master;
        var phases = log.LogData.GetPhases(log);
        var dmgDistributions = new List<DamageDistributionDto>(phases.Count);
        var dmgTakenDistributions = new List<DamageDistributionDto>(phases.Count);

        foreach (PhaseData phase in phases)
        {
            if (phase.Targets.ContainsKey(master))
            {
                dmgDistributions.Add(DamageDistributionDto.BuildMinionsDamageDistributionData(log, minions, null, phase, used));
                dmgTakenDistributions.Add(DamageDistributionDto.BuildMinionsDamageTakenDistributionData(log, minions, phase, used));
            }
            else
            {
                dmgDistributions.Add(DamageDistributionDto.EmptyInstance);
                dmgTakenDistributions.Add(DamageDistributionDto.EmptyInstance);
            }
        }

        var dto = new MinionDetailsDto()
        {
            DmgDistributions = dmgDistributions,
            DmgDistributionsTaken = dmgTakenDistributions
        };
        return dto;
    }

}
