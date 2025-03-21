using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIBuilders.HtmlModels.EXTHealing;

internal class EXTHealingStatsPlayerChartDto
{
    public readonly PlayerDamageChartDto<int> Healing;
    public readonly PlayerDamageChartDto<int> HealingPowerHealing;
    public readonly PlayerDamageChartDto<int> ConversionBasedHealing;
    //public readonly PlayerDamageChartDto<int> HybridHealing;

    private EXTHealingStatsPlayerChartDto(ParsedEvtcLog log, PhaseData phase, SingleActor p)
    {
        Healing = new PlayerDamageChartDto<int>()
        {
            Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.All),
            Taken = p.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.All),
            Targets = new (log.Friendlies.Count)
        };
        //
        var hybridHealingPower = new List<int>(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
        IReadOnlyList<int> hybrid = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid);
        for (int i = 0; i < hybrid.Count; i++)
        {
            hybridHealingPower[i] += hybrid[i];
        }
        var hybridHealingPowerReceived = new List<int>(p.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
        IReadOnlyList<int> hybridReceived = p.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid);
        for (int i = 0; i < hybridReceived.Count; i++)
        {
            hybridHealingPowerReceived[i] += hybridReceived[i];
        }
        HealingPowerHealing = new PlayerDamageChartDto<int>()
        {
            Total = hybridHealingPower,
            Taken = hybridHealingPowerReceived,
            Targets = new (log.Friendlies.Count)
        };
        //
        ConversionBasedHealing = new PlayerDamageChartDto<int>()
        {
            Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.ConversionBased),
            Taken = p.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.ConversionBased),
            Targets = new (log.Friendlies.Count)
        };
        //
        /*HybridHealing = new PlayerDamageChartDto<int>()
        {
            Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid),
            Targets = new List<IReadOnlyList<int>>()
        };*/
        //
        foreach (SingleActor target in log.Friendlies)
        {
            Healing.Targets.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.All));
            //
            hybridHealingPower = new List<int>(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
            hybrid = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.Hybrid);
            for (int i = 0; i < hybrid.Count; i++)
            {
                hybridHealingPower[i] += hybrid[i];
            }
            HealingPowerHealing.Targets.Add(hybridHealingPower);
            //
            ConversionBasedHealing.Targets.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.ConversionBased));
            //
            //HybridHealing.Targets.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.Hybrid));
        }
    }

    public static List<EXTHealingStatsPlayerChartDto> BuildPlayersHealingGraphData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<EXTHealingStatsPlayerChartDto>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new EXTHealingStatsPlayerChartDto(log, phase, actor));
        }
        return list;
    }
}
