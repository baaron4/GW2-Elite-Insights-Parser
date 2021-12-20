using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.EXTHealing
{
    internal class HealingStatsExtension
    {
        public List<EXTHealingStatsPhaseDto> HealingPhases { get; }

        public List<EXTHealingStatsPlayerDetailsDto> PlayerHealingDetails { get; }

        public List<List<EXTHealingStatsPlayerChartDto>> PlayerHealingCharts { get; }

        public HealingStatsExtension(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            HealingPhases = new List<EXTHealingStatsPhaseDto>();
            PlayerHealingCharts = new List<List<EXTHealingStatsPlayerChartDto>>();
            PlayerHealingDetails = new List<EXTHealingStatsPlayerDetailsDto>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                HealingPhases.Add(new EXTHealingStatsPhaseDto(phase, log));
                PlayerHealingCharts.Add(EXTHealingStatsPlayerChartDto.BuildPlayersHealingGraphData(log, phase));
            }
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                PlayerHealingDetails.Add(EXTHealingStatsPlayerDetailsDto.BuildPlayerHealingData(log, actor, usedSkills, usedBuffs));
            }
        }
    }
}
