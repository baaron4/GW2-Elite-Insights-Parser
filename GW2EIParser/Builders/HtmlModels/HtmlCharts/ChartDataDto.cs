using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class ChartDataDto
    {
        public List<PhaseChartDataDto> Phases { get; set; } = new List<PhaseChartDataDto>();
        public List<MechanicChartDataDto> Mechanics { get; set; } = new List<MechanicChartDataDto>();

        public static ChartDataDto BuildChartData(ParsedLog log)
        {
            var chartData = new ChartDataDto();
            var phaseChartData = new List<PhaseChartDataDto>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                var phaseData = new PhaseChartDataDto()
                {
                    Players = PlayerChartDataDto.BuildPlayersGraphData(log, i)
                };
                foreach (NPC target in phases[i].Targets)
                {
                    phaseData.Targets.Add(TargetChartDataDto.BuildTargetGraphData(log, i, target));
                }
                if (i == 0)
                {
                    phaseData.TargetsHealthForCR = new List<double[]>();
                    foreach (NPC target in log.FightData.Logic.Targets)
                    {
                        phaseData.TargetsHealthForCR.Add(target.Get1SHealthGraph(log)[0]);
                    }
                }

                phaseChartData.Add(phaseData);
            }
            chartData.Phases = phaseChartData;
            chartData.Mechanics = MechanicChartDataDto.BuildMechanicsChartData(log);
            return chartData;
        }
    }
}
