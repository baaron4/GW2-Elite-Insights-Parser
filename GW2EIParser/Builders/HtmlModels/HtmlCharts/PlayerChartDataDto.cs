using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class PlayerChartDataDto
    {
        public List<List<int>> Targets { get; internal set; }
        public List<int> Total { get; internal set; }
        public List<object[]> HealthStates { get; internal set; }

        internal static List<PlayerChartDataDto> BuildPlayersGraphData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<PlayerChartDataDto>();
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];

            foreach (Player p in log.PlayerList)
            {
                var pChar = new PlayerChartDataDto()
                {
                    Total = p.Get1SDamageList(log, phaseIndex, phase, null),
                    Targets = new List<List<int>>(),
                    HealthStates = ChartDataDto.BuildHealthGraphStates(log, p, log.FightData.GetPhases(log)[phaseIndex], true)
                };
                foreach (NPC target in phase.Targets)
                {
                    pChar.Targets.Add(p.Get1SDamageList(log, phaseIndex, phase, target));
                }
                list.Add(pChar);
            }
            return list;
        }
    }
}
