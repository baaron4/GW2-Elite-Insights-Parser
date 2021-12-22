using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTBarrier
{
    internal class BarrierStatsExtension
    {
        public List<EXTBarrierStatsPhaseDto> BarrierPhases { get; }

        public List<EXTBarrierStatsPlayerDetailsDto> PlayerBarrierDetails { get; }

        public List<List<EXTBarrierStatsPlayerChartDto>> PlayerBarrierCharts { get; }

        public BarrierStatsExtension(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            BarrierPhases = new List<EXTBarrierStatsPhaseDto>();
            PlayerBarrierCharts = new List<List<EXTBarrierStatsPlayerChartDto>>();
            PlayerBarrierDetails = new List<EXTBarrierStatsPlayerDetailsDto>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                BarrierPhases.Add(new EXTBarrierStatsPhaseDto(phase, log));
                PlayerBarrierCharts.Add(EXTBarrierStatsPlayerChartDto.BuildPlayersBarrierGraphData(log, phase));
            }
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                PlayerBarrierDetails.Add(EXTBarrierStatsPlayerDetailsDto.BuildPlayerBarrierData(log, actor, usedSkills, usedBuffs));
            }
        }
    }
}
