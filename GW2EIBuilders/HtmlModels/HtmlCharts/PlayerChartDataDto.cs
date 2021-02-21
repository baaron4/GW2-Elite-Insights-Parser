using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class PlayerChartDataDto : ActorChartDataDto
    {
        public PlayerDamageChartDto<int> Damage { get; }
        public PlayerDamageChartDto<int> PowerDamage { get; }
        public PlayerDamageChartDto<int> ConditionDamage { get; }
        public PlayerDamageChartDto<double> BreakbarDamage { get; }

        private PlayerChartDataDto(ParsedEvtcLog log, PhaseData phase, Player p) : base(log, phase, p, true)
        {
            Damage = new PlayerDamageChartDto<int>()
            {
                Total = p.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.All),
                Targets = new List<IReadOnlyList<int>>()
            };
            PowerDamage = new PlayerDamageChartDto<int>()
            {
                Total = p.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power),
                Targets = new List<IReadOnlyList<int>>()
            };
            ConditionDamage = new PlayerDamageChartDto<int>()
            {
                Total = p.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition),
                Targets = new List<IReadOnlyList<int>>()
            };
            BreakbarDamage = new PlayerDamageChartDto<double>()
            {
                Total = p.Get1SBreakbarDamageList(log, phase.Start, phase.End, null),
                Targets = new List<IReadOnlyList<double>>()
            };
            foreach (NPC target in phase.Targets)
            {
                Damage.Targets.Add(p.Get1SDamageList(log, phase.Start, phase.End, target, ParserHelper.DamageType.All));
                PowerDamage.Targets.Add(p.Get1SDamageList(log, phase.Start, phase.End, target, ParserHelper.DamageType.Power));
                ConditionDamage.Targets.Add(p.Get1SDamageList(log, phase.Start, phase.End, target, ParserHelper.DamageType.Condition));
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
