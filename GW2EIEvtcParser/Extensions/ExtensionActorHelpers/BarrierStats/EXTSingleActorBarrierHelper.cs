using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public class EXTSingleActorBarrierHelper : EXTActorBarrierHelper
{
    private readonly SingleActor _actor;
    private AgentItem _agentItem => _actor.AgentItem;
    private AgentItem _englobingAgentItem => _actor.EnglobingAgentItem;

    private CachingCollectionWithTarget<int[]>? _barrier1S;
    private CachingCollectionWithTarget<int[]>? _barrierReceived1S;

    private CachingCollectionWithTarget<EXTFinalOutgoingBarrierStat>? _outgoinBarrierStats;
    private CachingCollectionWithTarget<EXTFinalIncomingBarrierStat>? _incomingBarrierStats;

    internal EXTSingleActorBarrierHelper(SingleActor actor) : base()
    {
        _actor = actor;
    }
    protected override void InitBarrierEvents(ParsedEvtcLog log)
    {
        if (BarrierEventsByDst == null)
        {
            var barrierEvents = new List<EXTBarrierEvent>(log.CombatData.EXTBarrierCombatData.GetBarrierData(_agentItem).Where(x => x.ToFriendly));
            var minions = _actor.GetMinions(log); //TODO(Rennorb) @perf: Find average complexity for reserving elements in barrier events
            foreach (Minions minion in minions)
            {
                barrierEvents.AddRange(minion.EXTBarrier.GetOutgoingBarrierEvents(null, log));
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
            var barrierReceivedEvents = new List<EXTBarrierEvent>(log.CombatData.EXTBarrierCombatData.GetBarrierReceivedData(_agentItem).Where(x => x.ToFriendly));
            BarrierReceivedEventsBySrc = barrierReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            BarrierReceivedEventsBySrc[ParserHelper._nullAgent] = barrierReceivedEvents;
        }
    }

    private CachingCollectionWithTarget<List<EXTBarrierEvent>>? _justActorBarrierCache;
    public IReadOnlyList<EXTBarrierEvent> GetJustActorOutgoingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _justActorBarrierCache ??= new(log);
        if (!_justActorBarrierCache.TryGetValue(start, end, target, out var healEvents))
        {
            healEvents = GetOutgoingBarrierEvents(target, log, start, end).Where(x => x.From.Is(_agentItem)).ToList();
            _justActorBarrierCache.Set(start, end, target, healEvents);
        }
        return healEvents;
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
