using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions;

public class EXTSingleActorHealingHelper : EXTActorHealingHelper
{
    private readonly SingleActor _actor;
    private AgentItem _agentItem => _actor.AgentItem;

    private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTHealingEvent>>> _typedSelfHealEvents = [];

    private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<int[]>> _healing1S = [];
    private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<int[]>> _healingReceived1S = [];

    private CachingCollectionWithTarget<EXTFinalOutgoingHealingStat>? _outgoingHealStats;
    private CachingCollectionWithTarget<EXTFinalIncomingHealingStat>? _incomingHealStats;

    internal EXTSingleActorHealingHelper(SingleActor actor) : base()
    {
        _actor = actor;
    }


#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitHealEvents(ParsedEvtcLog log)
    {
        if (HealEvents == null)
        {
            HealEvents = new List<EXTHealingEvent>(log.CombatData.EXTHealingCombatData.GetHealData(_agentItem).Where(x => x.ToFriendly));
            foreach (var minion in _actor.GetMinions(log).Values)
            {
                HealEvents.AddRange(minion.EXTHealing.GetOutgoingHealEvents(null, log));
            }
            HealEvents.SortByTime();
            HealEventsByDst = HealEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting

    public override IEnumerable<EXTHealingEvent> GetOutgoingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.CombatData.HasEXTHealing)
        {
            throw new InvalidOperationException("Healing Stats extension not present");
        }

        InitHealEvents(log);

        if (target != null)
        {
            if (HealEventsByDst.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [ ];
            }
        }

        return HealEvents.Where(x => x.Time >= start && x.Time <= end);
    }

    public override IEnumerable<EXTHealingEvent> GetIncomingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.CombatData.HasEXTHealing)
        {
            throw new InvalidOperationException("Healing Stats extension not present");
        }

        InitIncomingHealEvents(log);

        if (target != null)
        {
            if (HealReceivedEventsBySrc.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [ ];
            }
        }

        return HealReceivedEvents.Where(x => x.Time >= start && x.Time <= end);
    }


#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitIncomingHealEvents(ParsedEvtcLog log)
    {
        if (HealReceivedEvents == null)
        {
            HealReceivedEvents = new List<EXTHealingEvent>(log.CombatData.EXTHealingCombatData.GetHealReceivedData(_agentItem).Where(x => x.ToFriendly));
            HealReceivedEventsBySrc = HealReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting


    public IEnumerable<EXTHealingEvent> GetJustActorOutgoingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        return GetOutgoingHealEvents(target, log, start, end).Where(x => x.From.Is(_agentItem));
    }

    internal IReadOnlyList<EXTHealingEvent> GetJustActorTypedOutgoingHealEvents(SingleActor target, ParsedEvtcLog log, long start, long end, EXTHealingType healingType)
    {
        if (!_typedSelfHealEvents.TryGetValue(healingType, out var healEventsPerPhasePerTarget))
        {
            healEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<EXTHealingEvent>>(log);
            _typedSelfHealEvents[healingType] = healEventsPerPhasePerTarget;
        }

        if (!healEventsPerPhasePerTarget.TryGetValue(start, end, target, out var dls))
        {
            dls = GetTypedOutgoingHealEvents(target, log, start, end, healingType).Where(x => x.From.Is(_agentItem)).ToList();
            healEventsPerPhasePerTarget.Set(start, end, target, dls);
        }

        return dls;
    }

    private static int[] ComputeHealingGraph(IReadOnlyList<EXTHealingEvent> dls, long start, long end)
    {
        int durationInMS = (int)(end - start);
        int durationInS = durationInMS / 1000;
        var graph = durationInS * 1000 != durationInMS ? new int[durationInS + 2] : new int[durationInS + 1];
        // fill the graph
        int previousTime = 0;
        foreach (EXTHealingEvent dl in dls)
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
            graph[time] += dl.HealingDone;
        }
        for (int i = previousTime + 1; i < graph.Length; i++)
        {
            graph[i] = graph[previousTime];
        }
        return graph;
    }

    public IReadOnlyList<int> Get1SHealingList(ParsedEvtcLog log, long start, long end, SingleActor? target, EXTHealingType healingType = EXTHealingType.All)
    {
        if (!_healing1S.TryGetValue(healingType, out var graphs))
        {
            graphs = new CachingCollectionWithTarget<int[]>(log);
            _healing1S[healingType] = graphs;
        }
        if (!graphs.TryGetValue(start, end, target, out var graph))
        {
            graph = ComputeHealingGraph(GetTypedOutgoingHealEvents(target, log, start, end, healingType), start, end);
            //
            graphs.Set(start, end, target, graph);
        }
        return graph;
    }

    public IReadOnlyList<int> Get1SHealingReceivedList(ParsedEvtcLog log, long start, long end, SingleActor? target, EXTHealingType healingType = EXTHealingType.All)
    {
        if (!_healingReceived1S.TryGetValue(healingType, out var graphs))
        {
            graphs = new CachingCollectionWithTarget<int[]>(log);
            _healingReceived1S[healingType] = graphs;
        }
        if (!graphs.TryGetValue(start, end, target, out var graph))
        {
            graph = ComputeHealingGraph(GetTypedIncomingHealEvents(target, log, start, end, healingType), start, end);
            //
            graphs.Set(start, end, target, graph);
        }
        return graph;
    }

    public EXTFinalOutgoingHealingStat GetOutgoingHealStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _outgoingHealStats ??= new CachingCollectionWithTarget<EXTFinalOutgoingHealingStat>(log);
        if (!_outgoingHealStats.TryGetValue(start, end, target, out var value))
        {
            value = new EXTFinalOutgoingHealingStat(log, start, end, _actor, target);
            _outgoingHealStats.Set(start, end, target, value);
        }
        return value;
    }

    public EXTFinalIncomingHealingStat GetIncomingHealStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _incomingHealStats ??= new CachingCollectionWithTarget<EXTFinalIncomingHealingStat>(log);
        if (!_incomingHealStats.TryGetValue(start, end, target, out var value))
        {
            value = new EXTFinalIncomingHealingStat(log, start, end, _actor, target);
            _incomingHealStats.Set(start, end, target, value);
        }
        return value;
    }
}
