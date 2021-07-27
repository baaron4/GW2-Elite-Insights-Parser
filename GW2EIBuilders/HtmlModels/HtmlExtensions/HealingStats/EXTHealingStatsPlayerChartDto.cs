using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIBuilders.HtmlModels
{
    internal class EXTHealingStatsPlayerChartDto
    {
        public EXTHealingStatsPlayerHealingChartDto<int> Healing { get; }
        public EXTHealingStatsPlayerHealingChartDto<int> HealingPowerHealing { get; }
        public EXTHealingStatsPlayerHealingChartDto<int> ConversionBasedHealing { get; }
        public EXTHealingStatsPlayerHealingChartDto<int> HybridHealing { get; }

        private EXTHealingStatsPlayerChartDto(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor p)
        {
            Healing = new EXTHealingStatsPlayerHealingChartDto<int>()
            {
                Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.All),
                Friendlies = new List<IReadOnlyList<int>>()
            };
            HealingPowerHealing = new EXTHealingStatsPlayerHealingChartDto<int>()
            {
                Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.HealingPower),
                Friendlies = new List<IReadOnlyList<int>>()
            };
            ConversionBasedHealing = new EXTHealingStatsPlayerHealingChartDto<int>()
            {
                Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.ConversionBased),
                Friendlies = new List<IReadOnlyList<int>>()
            };
            HybridHealing = new EXTHealingStatsPlayerHealingChartDto<int>()
            {
                Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid),
                Friendlies = new List<IReadOnlyList<int>>()
            };
            foreach (AbstractSingleActor target in log.Friendlies)
            {
                Healing.Friendlies.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.All));
                HealingPowerHealing.Friendlies.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
                ConversionBasedHealing.Friendlies.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.ConversionBased));
                HybridHealing.Friendlies.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.Hybrid));
            }
        }

        public static List<EXTHealingStatsPlayerChartDto> BuildPlayersHealingGraphData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<EXTHealingStatsPlayerChartDto>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new EXTHealingStatsPlayerChartDto(log, phase, actor));
            }
            return list;
        }
    }
}
