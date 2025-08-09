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


#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitBarrierEvents(ParsedEvtcLog log)
    {
        if (BarrierEvents == null)
        {
            BarrierEvents = new List<EXTBarrierEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
            foreach (NPC minion in _minionList)
            {
                BarrierEvents.AddRange(minion.EXTBarrier.GetOutgoingBarrierEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            BarrierEvents.SortByTime();
            BarrierEventsByDst = BarrierEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting

    public override IEnumerable<EXTBarrierEvent> GetOutgoingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitBarrierEvents(log);

        if (target != null)
        {
            if (BarrierEventsByDst.TryGetValue(target.EnglobingAgentItem, out var barrierEvents))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return barrierEvents.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return BarrierEvents.Where(x => x.Time >= start && x.Time <= end);
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitIncomingBarrierEvents(ParsedEvtcLog log)
    {
        if (BarrierReceivedEvents == null)
        {
            BarrierReceivedEvents = new List<EXTBarrierEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
            foreach (NPC minion in _minionList)
            {
                BarrierReceivedEvents.AddRange(minion.EXTBarrier.GetIncomingBarrierEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            BarrierReceivedEvents.SortByTime();
            BarrierReceivedEventsBySrc = BarrierReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting

    public override IEnumerable<EXTBarrierEvent> GetIncomingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitIncomingBarrierEvents(log);

        if (target != null)
        {
            if (BarrierReceivedEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var barrierEvents))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return barrierEvents.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return BarrierReceivedEvents.Where(x => x.Time >= start && x.Time <= end);
    }
}
