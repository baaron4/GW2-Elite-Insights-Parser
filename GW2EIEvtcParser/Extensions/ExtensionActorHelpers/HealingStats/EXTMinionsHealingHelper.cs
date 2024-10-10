using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTMinionsHealingHelper : EXTActorHealingHelper
    {
        private readonly Minions _minions;
        private IReadOnlyList<NPC> _minionList => _minions.MinionList;

        internal EXTMinionsHealingHelper(Minions minions) : base()
        {
            _minions = minions;
        }


        public override List<EXTAbstractHealingEvent> GetOutgoingHealEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
        {
            if (HealEvents == null)
            {
               InitHealEvents(log);
            }
            if (target != null)
            {
                if (HealEventsByDst.TryGetValue(target.AgentItem, out var list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return [ ];
                }
            }
            return HealEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        /// <param name="healEventsList">Append to this list</param>
        public void AppendOutgoingHealEvents(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end, List<EXTAbstractHealingEvent> healEventsList)
        {
            if (HealEvents == null)
            {
                InitHealEvents(log);
            }

            if (target != null)
            {
                if (HealEventsByDst.TryGetValue(target.AgentItem, out var list))
                {
                    healEventsList.AddRange(list.Where(x => x.Time >= start && x.Time <= end));
                }

                return;
            }

            healEventsList.AddRange(HealEvents.Where(x => x.Time >= start && x.Time <= end));

            return;
        }

        [MemberNotNull(nameof(HealEvents))]
        [MemberNotNull(nameof(HealEventsByDst))]
        void InitHealEvents(ParsedEvtcLog log)
        {
            //TODO(Rennorb) @perf: find average complexity
            HealEvents = new List<EXTAbstractHealingEvent>(_minionList.Count * 10);
            foreach (NPC minion in _minionList)
            {
                minion.EXTHealing.AppendOutgoingHealEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd, HealEvents);
            }
            HealEvents.SortByTime();
            HealEventsByDst = HealEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }

        public override List<EXTAbstractHealingEvent> GetIncomingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (HealReceivedEvents == null)
            {
                InitIncomingHealEvents(log);
            }
            if (target != null)
            {
                if (HealReceivedEventsBySrc.TryGetValue(target.AgentItem, out var list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return [ ];
                }
            }
            return HealReceivedEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        /// <param name="healEventsList">Append to this list</param>
        /// <exception cref="InvalidOperationException">Heal Stats ext missing</exception>
        public void AppendIncomingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end, List<EXTAbstractHealingEvent> healEventsList)
        {
            if (HealReceivedEvents == null)
            {
                InitIncomingHealEvents(log);
            }
            if (target != null)
            {
                if (HealReceivedEventsBySrc.TryGetValue(target.AgentItem, out var list))
                {
                    healEventsList.AddRange(list.Where(x => x.Time >= start && x.Time <= end));
                }

                return;
            }

            healEventsList.AddRange(HealReceivedEvents.Where(x => x.Time >= start && x.Time <= end));

            return;
        }

        [MemberNotNull(nameof(HealReceivedEvents))]
        [MemberNotNull(nameof(HealReceivedEventsBySrc))]
        void InitIncomingHealEvents(ParsedEvtcLog log)
        {
            //TODO(Rennorb) @perf: find average complexity
            HealReceivedEvents = new List<EXTAbstractHealingEvent>(_minionList.Count * 10);
            foreach (NPC minion in _minionList)
            {
                minion.EXTHealing.AppendIncomingHealEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd, HealReceivedEvents);
            }
            HealReceivedEvents.SortByTime();
            HealReceivedEventsBySrc = HealReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
}
