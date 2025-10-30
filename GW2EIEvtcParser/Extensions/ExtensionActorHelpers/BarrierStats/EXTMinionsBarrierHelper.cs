using System.Diagnostics.Metrics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public class EXTMinionsBarrierHelper : EXTActorBarrierHelper
{
    private readonly Minions _minions;
    private IReadOnlyList<NPC> _minionList => _minions.MinionList;
    private SingleActor Master => _minions.Master;

    internal EXTMinionsBarrierHelper(Minions minions) : base()
    {
        _minions = minions;
    }


    protected override void InitBarrierEvents(ParsedEvtcLog log)
    {
        if (BarrierEventsByDst == null)
        {
            var barrierEvents = new List<EXTBarrierEvent>(_minionList.Count); //TODO_PERF(Rennorb): find average complexity
            foreach (NPC minion in _minionList)
            {
                barrierEvents.AddRange(minion.EXTBarrier.GetOutgoingBarrierEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            barrierEvents.SortByTime();
            BarrierEventsByDst = barrierEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            BarrierEventsByDst[ParserHelper._nullAgent] = barrierEvents;
        }
    }

    protected override void InitIncomingBarrierEvents(ParsedEvtcLog log)
    {
        if (BarrierReceivedEventsBySrc == null)
        {
            var barrierReceivedEvents = new List<EXTBarrierEvent>(_minionList.Count); //TODO_PERF(Rennorb): find average complexity
            foreach (NPC minion in _minionList)
            {
                barrierReceivedEvents.AddRange(minion.EXTBarrier.GetIncomingBarrierEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            barrierReceivedEvents.SortByTime();
            BarrierReceivedEventsBySrc = barrierReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            BarrierReceivedEventsBySrc[ParserHelper._nullAgent] = barrierReceivedEvents;
        }
    }
}
