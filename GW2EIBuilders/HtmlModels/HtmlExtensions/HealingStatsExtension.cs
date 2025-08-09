using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTHealing;

internal class HealingStatsExtension
{
    public readonly List<EXTHealingStatsPhaseDto> HealingPhases;
    public readonly List<EXTHealingStatsPlayerDetailsDto> PlayerHealingDetails;
    public readonly List<List<EXTHealingStatsPlayerChartDto>> PlayerHealingCharts;

    public HealingStatsExtension(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var phases = log.LogData.GetPhases(log);
        HealingPhases       = new(phases.Count);
        PlayerHealingCharts = new(phases.Count);
        foreach (PhaseData phase in phases)
        {
            HealingPhases.Add(new EXTHealingStatsPhaseDto(phase, log));
            PlayerHealingCharts.Add(EXTHealingStatsPlayerChartDto.BuildPlayersHealingGraphData(log, phase));
        }

        PlayerHealingDetails = new(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            PlayerHealingDetails.Add(EXTHealingStatsPlayerDetailsDto.BuildPlayerHealingData(log, actor, usedSkills, usedBuffs));
        }
    }
}
