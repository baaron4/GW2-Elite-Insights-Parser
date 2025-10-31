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

    protected override void InitHealEvents(ParsedEvtcLog log)
    {
        if (HealEventsByDst == null)
        {
            //TODO_PERF(Rennorb): find average complexity
            var healEvents = new List<EXTHealingEvent>(_minionList.Count * 10);
            foreach (NPC minion in _minionList)
            {
                healEvents.AddRange(minion.EXTHealing.GetOutgoingHealEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            healEvents.SortByTime();
            HealEventsByDst = healEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            HealEventsByDst[ParserHelper._nullAgent] = healEvents;
        }
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitIncomingHealEvents(ParsedEvtcLog log)
    {
        if (HealReceivedEventsBySrc == null)
        {
            //TODO_PERF(Rennorb): find average complexity
            var healReceivedEvents = new List<EXTHealingEvent>(_minionList.Count * 10);
            foreach (NPC minion in _minionList)
            {
                healReceivedEvents.AddRange(minion.EXTHealing.GetIncomingHealEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            healReceivedEvents.SortByTime();
            HealReceivedEventsBySrc = healReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            HealReceivedEventsBySrc[ParserHelper._nullAgent] = healReceivedEvents;
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting
}
