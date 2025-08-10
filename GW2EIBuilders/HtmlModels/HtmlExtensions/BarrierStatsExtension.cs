using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTBarrier;

internal class BarrierStatsExtension
{
    public readonly List<EXTBarrierStatsPhaseDto> BarrierPhases;

    public readonly List<EXTBarrierStatsPlayerDetailsDto> PlayerBarrierDetails;

    public readonly List<List<EXTBarrierStatsPlayerChartDto>> PlayerBarrierCharts;

    public BarrierStatsExtension(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var phases = log.LogData.GetPhases(log);
        BarrierPhases       = new(phases.Count);
        PlayerBarrierCharts = new(phases.Count);
        foreach (PhaseData phase in phases)
        {
            BarrierPhases.Add(new EXTBarrierStatsPhaseDto(phase, log));
            PlayerBarrierCharts.Add(EXTBarrierStatsPlayerChartDto.BuildPlayersBarrierGraphData(log, phase));
        }

        PlayerBarrierDetails = new(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            PlayerBarrierDetails.Add(EXTBarrierStatsPlayerDetailsDto.BuildPlayerBarrierData(log, actor, usedSkills, usedBuffs));
        }
    }
}
