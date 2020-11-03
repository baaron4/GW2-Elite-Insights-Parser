﻿using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class PlayerChartDataDto
    {
        public List<List<int>> Targets { get; set; }
        public List<int> Total { get; set; }
        public List<object[]> HealthStates { get; set; }

        public static List<PlayerChartDataDto> BuildPlayersGraphData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<PlayerChartDataDto>();
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];

            foreach (Player p in log.PlayerList)
            {
                var pChar = new PlayerChartDataDto()
                {
                    Total = new List<int>(p.Get1SDamageList(log, phaseIndex, phase, null)),
                    Targets = new List<List<int>>(),
                    HealthStates = ChartDataDto.BuildHealthGraphStates(log, p, log.FightData.GetPhases(log)[phaseIndex], true)
                };
                foreach (NPC target in phase.Targets)
                {
                    pChar.Targets.Add(new List<int>(p.Get1SDamageList(log, phaseIndex, phase, target)));
                }
                list.Add(pChar);
            }
            return list;
        }
    }
}
