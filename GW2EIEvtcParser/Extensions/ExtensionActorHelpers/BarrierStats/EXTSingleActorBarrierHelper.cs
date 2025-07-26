using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public class EXTSingleActorBarrierHelper : EXTActorBarrierHelper
{
    private readonly SingleActor _actor;
    private AgentItem _agentItem => _actor.AgentItem;

    private CachingCollectionWithTarget<int[]>? _barrier1S;
    private CachingCollectionWithTarget<int[]>? _barrierReceived1S;

    private CachingCollectionWithTarget<EXTFinalOutgoingBarrierStat>? _outgoinBarrierStats;
    private CachingCollectionWithTarget<EXTFinalIncomingBarrierStat>? _incomingBarrierStats;

    internal EXTSingleActorBarrierHelper(SingleActor actor) : base()
    {
        _actor = actor;
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitBarrierEvents(ParsedEvtcLog log)
    {
        if (BarrierEvents == null)
        {
            BarrierEvents = new List<EXTBarrierEvent>(log.CombatData.EXTBarrierCombatData.GetBarrierData(_agentItem).Where(x => x.ToFriendly));
            IReadOnlyDictionary<long, Minions> minions = _actor.GetMinions(log); //TODO(Rennorb) @perf: Find average complexity for reserving elements in barrier events
            foreach (Minions minion in minions.Values)
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
        if (!log.CombatData.HasEXTBarrier)
        {
            throw new InvalidOperationException("Healing Stats extension not present");
        }

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
            BarrierReceivedEvents = new List<EXTBarrierEvent>(log.CombatData.EXTBarrierCombatData.GetBarrierReceivedData(_agentItem).Where(x => x.ToFriendly));
            BarrierReceivedEventsBySrc = BarrierReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting

    public override IEnumerable<EXTBarrierEvent> GetIncomingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.CombatData.HasEXTBarrier)
        {
            throw new InvalidOperationException("Healing Stats extension not present");
        }

        InitIncomingBarrierEvents(log);
        if (target != null)
        {
            if (BarrierReceivedEventsBySrc.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [ ];
            }
        }

        return BarrierReceivedEvents.Where(x => x.Time >= start && x.Time <= end);
    }

    public IEnumerable<EXTBarrierEvent> GetJustActorOutgoingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        return GetOutgoingBarrierEvents(target, log, start, end).Where(x => x.From == _agentItem);
    }

    private static int[] ComputeBarrierGraph(IEnumerable<EXTBarrierEvent> dls, long start, long end)
    {
        int durationInMS = (int)(end - start);
        int durationInS = durationInMS / 1000;
        var graph = durationInS * 1000 != durationInMS ? new int[durationInS + 2] : new int[durationInS + 1];
        // fill the graph
        int previousTime = 0;
        foreach (EXTBarrierEvent dl in dls)
        {
            int time = (int)Math.Ceiling((dl.Time - start) / 1000.0);
            if (time != previousTime)
            {
                for (int i = previousTime + 1; i <= time; i++)
                {
                    graph[i] = graph[previousTime];
                }
            }
            previousTime = time;
            graph[time] += dl.BarrierGiven;
        }
        
        for (int i = previousTime + 1; i < graph.Length; i++)
        {
            graph[i] = graph[previousTime];
        }

        return graph;
    }

    public IReadOnlyList<int> Get1SBarrierList(ParsedEvtcLog log, long start, long end, SingleActor? target)
    {
        _barrier1S ??= new CachingCollectionWithTarget<int[]>(log);
        if (!_barrier1S.TryGetValue(start, end, target, out int[]? graph))
        {
            graph = ComputeBarrierGraph(GetOutgoingBarrierEvents(target, log, start, end), start, end);
            //
            _barrier1S.Set(start, end, target, graph);
        }
        return graph;
    }
    public IReadOnlyList<int> Get1SBarrierReceivedList(ParsedEvtcLog log, long start, long end, SingleActor? target)
    {
        _barrierReceived1S ??= new CachingCollectionWithTarget<int[]>(log);
        if (!_barrierReceived1S.TryGetValue(start, end, target, out int[]? graph))
        {
            graph = ComputeBarrierGraph(GetIncomingBarrierEvents(target, log, start, end), start, end);
            //
            _barrierReceived1S.Set(start, end, target, graph);
        }
        return graph;
    }

    public EXTFinalOutgoingBarrierStat GetOutgoingBarrierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _outgoinBarrierStats ??= new CachingCollectionWithTarget<EXTFinalOutgoingBarrierStat>(log);
        if (!_outgoinBarrierStats.TryGetValue(start, end, target, out EXTFinalOutgoingBarrierStat? value))
        {
            value = new EXTFinalOutgoingBarrierStat(log, start, end, _actor, target);
            _outgoinBarrierStats.Set(start, end, target, value);
        }
        return value;
    }

    public EXTFinalIncomingBarrierStat GetIncomingBarrierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _incomingBarrierStats ??= new CachingCollectionWithTarget<EXTFinalIncomingBarrierStat>(log);
        if (!_incomingBarrierStats.TryGetValue(start, end, target, out EXTFinalIncomingBarrierStat? value))
        {
            value = new EXTFinalIncomingBarrierStat(log, start, end, _actor, target);
            _incomingBarrierStats.Set(start, end, target, value);
        }
        return value;
    }
}
