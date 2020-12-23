using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class PlayerChartDataDto
    {
        public PlayerDamageChartDto<int> Damage { get; set; }
        public PlayerDamageChartDto<double> BreakbarDamage { get; set; }
        public List<object[]> HealthStates { get; set; }
        public List<object[]> BarrierStates { get; set; }

        public static List<PlayerChartDataDto> BuildPlayersGraphData(ParsedEvtcLog log, int phaseIndex)
        {
            var list = new List<PlayerChartDataDto>();
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];

            foreach (Player p in log.PlayerList)
            {
                var pChar = new PlayerChartDataDto()
                {
                    Damage = new PlayerDamageChartDto<int>()
                    {
                        Total = p.Get1SDamageList(log, phaseIndex, phase, null),
                        Targets = new List<IReadOnlyList<int>>()
                    },
                    BreakbarDamage = new PlayerDamageChartDto<double>()
                    {
                        Total = p.Get1SBreakbarDamageList(log, phaseIndex, phase, null),
                        Targets = new List<IReadOnlyList<double>>()
                    },
                    HealthStates = ChartDataDto.BuildHealthStates(log, p, phase, true),
                    BarrierStates = ChartDataDto.BuildBarrierStates(log, p, phase)
                };
                foreach (NPC target in phase.Targets)
                {
                    pChar.Damage.Targets.Add(p.Get1SDamageList(log, phaseIndex, phase, target));
                    pChar.BreakbarDamage.Targets.Add(p.Get1SBreakbarDamageList(log, phaseIndex, phase, target));
                }
                list.Add(pChar);
            }
            return list;
        }
    }
}
