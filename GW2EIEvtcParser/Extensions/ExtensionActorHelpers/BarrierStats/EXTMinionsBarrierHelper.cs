using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public class EXTMinionsBarrierHelper : EXTActorBarrierHelper
{
    private readonly Minions _minions;
    private IReadOnlyList<NPC> _minionList => _minions.MinionList;

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
                BarrierEvents.AddRange(minion.EXTBarrier.GetOutgoingBarrierEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
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
            if (BarrierEventsByDst.TryGetValue(target.AgentItem, out var barrierEvents))
            {
                return barrierEvents.Where(x => x.Time >= start && x.Time <= end);
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
                BarrierReceivedEvents.AddRange(minion.EXTBarrier.GetIncomingBarrierEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
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
            if (BarrierReceivedEventsBySrc.TryGetValue(target.AgentItem, out var barrierEvents))
            {
                return barrierEvents.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [ ];
            }
        }

        return BarrierReceivedEvents.Where(x => x.Time >= start && x.Time <= end);
    }
}
