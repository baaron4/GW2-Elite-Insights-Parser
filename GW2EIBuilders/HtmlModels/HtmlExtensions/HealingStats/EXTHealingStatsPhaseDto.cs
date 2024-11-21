using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIBuilders.HtmlModels.EXTHealing;

using HealingStatItem = List<int>;

internal class EXTHealingStatsPhaseDto
{

    public List<HealingStatItem> OutgoingHealingStats { get; set; }
    public List<List<HealingStatItem>> OutgoingHealingStatsTargets { get; set; }
    public List<HealingStatItem> IncomingHealingStats { get; set; }

    public EXTHealingStatsPhaseDto(PhaseData phase, ParsedEvtcLog log)
    {
        OutgoingHealingStats = BuildOutgoingHealingStatData(log, phase);
        OutgoingHealingStatsTargets = BuildOutgoingHealingFriendlyStatData(log, phase);
        IncomingHealingStats = BuildIncomingHealingStatData(log, phase);
    }


    // helper methods

    private static HealingStatItem GetOutgoingHealingStatData(EXTFinalOutgoingHealingStat outgoingHealingStats)
    {
        return [
                outgoingHealingStats.Healing,
                outgoingHealingStats.HealingPowerHealing + outgoingHealingStats.HybridHealing,
                outgoingHealingStats.ConversionHealing,
                //outgoingHealingStats.HybridHealing,
                outgoingHealingStats.DownedHealing
       ];
    }

    private static HealingStatItem GetIncomingHealingStatData(EXTFinalIncomingHealingStat incomingHealintStats)
    {
        return [
                incomingHealintStats.Healed,
                incomingHealintStats.HealingPowerHealed + incomingHealintStats.HybridHealed,
                incomingHealintStats.ConversionHealed,
                //incomingHealintStats.HybridHealed,
                incomingHealintStats.DownedHealed
            ];
    }
    public static List<HealingStatItem> BuildOutgoingHealingStatData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<HealingStatItem>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            EXTFinalOutgoingHealingStat outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(null, log, phase.Start, phase.End);
            list.Add(GetOutgoingHealingStatData(outgoingHealingStats));
        }
        return list;
    }

    public static List<List<HealingStatItem>> BuildOutgoingHealingFriendlyStatData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<List<HealingStatItem>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            var playerData = new List<HealingStatItem>(log.Friendlies.Count);

            foreach (SingleActor target in log.Friendlies)
            {
                playerData.Add(GetOutgoingHealingStatData(actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End)));
            }
            list.Add(playerData);
        }
        return list;
    }

    public static List<HealingStatItem> BuildIncomingHealingStatData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<HealingStatItem>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            EXTFinalIncomingHealingStat incomingHealintStats = actor.EXTHealing.GetIncomingHealStats(null, log, phase.Start, phase.End);
            list.Add(GetIncomingHealingStatData(incomingHealintStats));
        }

        return list;
    }
}
