using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class PlayerChartDataDto
    {
        public PlayerDamageChartDto<int> Damage { get;}
        public PlayerDamageChartDto<double> BreakbarDamage { get; }
        public List<object[]> HealthStates { get; }
        public List<object[]> BarrierStates { get; }

        private PlayerChartDataDto(ParsedEvtcLog log, PhaseData phase, Player p)
        {
            Damage = new PlayerDamageChartDto<int>()
            {
                Total = p.Get1SDamageList(log, phase.Start, phase.End, null),
                Targets = new List<IReadOnlyList<int>>()
            };
            BreakbarDamage = new PlayerDamageChartDto<double>()
            {
                Total = p.Get1SBreakbarDamageList(log, phase.Start, phase.End, null),
                Targets = new List<IReadOnlyList<double>>()
            };
            HealthStates = ChartDataDto.BuildHealthStates(log, p, phase, true);
            BarrierStates = ChartDataDto.BuildBarrierStates(log, p, phase);
            foreach (NPC target in phase.Targets)
            {
                Damage.Targets.Add(p.Get1SDamageList(log, phase.Start, phase.End, target));
                BreakbarDamage.Targets.Add(p.Get1SBreakbarDamageList(log, phase.Start, phase.End, target));
            }
        }

        public static List<PlayerChartDataDto> BuildPlayersGraphData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<PlayerChartDataDto>();

            foreach (Player p in log.PlayerList)
            {
                list.Add(new PlayerChartDataDto(log, phase, p));
            }
            return list;
        }
    }
}
