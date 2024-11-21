using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIBuilders.HtmlModels.EXTBarrier;


internal class EXTBarrierStatsPhaseDto
{

    public List<List<int>> OutgoingBarrierStats { get; set; }
    public List<List<List<int>>> OutgoingBarrierStatsTargets { get; set; }
    public List<List<int>> IncomingBarrierStats { get; set; }

    public EXTBarrierStatsPhaseDto(PhaseData phase, ParsedEvtcLog log)
    {
        OutgoingBarrierStats = BuildOutgoingBarrierStatData(log, phase);
        OutgoingBarrierStatsTargets = BuildOutgoingBarrierFriendlyStatData(log, phase);
        IncomingBarrierStats = BuildIncomingBarrierStatData(log, phase);
    }


    // helper methods

    private static List<int> GetOutgoingBarrierStatData(EXTFinalOutgoingBarrierStat outgoingBarrierStats)
    {
        var data = new List<int>
            {
                outgoingBarrierStats.Barrier,
            };
        return data;
    }

    private static List<int> GetIncomingBarrierStatData(EXTFinalIncomingBarrierStat incomingBarrierStats)
    {
        var data = new List<int>
            {
                incomingBarrierStats.BarrierReceived,
            };
        return data;
    }
    public static List<List<int>> BuildOutgoingBarrierStatData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<List<int>>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            EXTFinalOutgoingBarrierStat outgoingBarrierStats = actor.EXTBarrier.GetOutgoingBarrierStats(null, log, phase.Start, phase.End);
            list.Add(GetOutgoingBarrierStatData(outgoingBarrierStats));
        }
        return list;
    }

    public static List<List<List<int>>> BuildOutgoingBarrierFriendlyStatData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<List<List<int>>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            var playerData = new List<List<int>>();

            foreach (SingleActor target in log.Friendlies)
            {
                playerData.Add(GetOutgoingBarrierStatData(actor.EXTBarrier.GetOutgoingBarrierStats(target, log, phase.Start, phase.End)));
            }
            list.Add(playerData);
        }
        return list;
    }

    public static List<List<int>> BuildIncomingBarrierStatData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<List<int>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            EXTFinalIncomingBarrierStat incomingBarrierStats = actor.EXTBarrier.GetIncomingBarrierStats(null, log, phase.Start, phase.End);
            list.Add(GetIncomingBarrierStatData(incomingBarrierStats));
        }

        return list;
    }
}
