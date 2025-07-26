using System.Diagnostics.Metrics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public class EXTMinionsHealingHelper : EXTActorHealingHelper
{
    private readonly Minions _minions;
    private IReadOnlyList<NPC> _minionList => _minions.MinionList;
    private SingleActor Master => _minions.Master;

    internal EXTMinionsHealingHelper(Minions minions) : base()
    {
        _minions = minions;
    }


    public override IEnumerable<EXTHealingEvent> GetOutgoingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitHealEvents(log);

        if (target != null)
        {
            if (HealEventsByDst.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [];
            }
        }

        return HealEvents.Where(x => x.Time >= start && x.Time <= end);
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitHealEvents(ParsedEvtcLog log)
    {
        if (HealEvents == null)
        {
            //TODO(Rennorb) @perf: find average complexity
            HealEvents = new List<EXTHealingEvent>(_minionList.Count * 10);
            foreach (NPC minion in _minionList)
            {
                HealEvents.AddRange(minion.EXTHealing.GetOutgoingHealEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            HealEvents.SortByTime();
            HealEventsByDst = HealEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting

    public override IEnumerable<EXTHealingEvent> GetIncomingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitIncomingHealEvents(log);

        if (target != null)
        {
            if (HealReceivedEventsBySrc.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end);
            }
            else
            {
                return [];
            }
        }

        return HealReceivedEvents.Where(x => x.Time >= start && x.Time <= end);
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitIncomingHealEvents(ParsedEvtcLog log)
    {
        if (HealReceivedEvents == null)
        {
            //TODO(Rennorb) @perf: find average complexity
            HealReceivedEvents = new List<EXTHealingEvent>(_minionList.Count * 10);
            foreach (NPC minion in _minionList)
            {
                HealReceivedEvents.AddRange(minion.EXTHealing.GetIncomingHealEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            HealReceivedEvents.SortByTime();
            HealReceivedEventsBySrc = HealReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting
}
