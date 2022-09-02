using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;

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


        public override IReadOnlyList<EXTAbstractHealingEvent> GetOutgoingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (HealEvents == null)
            {
                HealEvents = new List<EXTAbstractHealingEvent>();
                foreach (NPC minion in _minionList)
                {
                    HealEvents.AddRange(minion.EXTHealing.GetOutgoingHealEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                HealEvents = HealEvents.OrderBy(x => x.Time).ToList();
                HealEventsByDst = HealEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (HealEventsByDst.TryGetValue(target.AgentItem, out List<EXTAbstractHealingEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<EXTAbstractHealingEvent>();
                }
            }
            return HealEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<EXTAbstractHealingEvent> GetIncomingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (HealReceivedEvents == null)
            {
                HealReceivedEvents = new List<EXTAbstractHealingEvent>();
                foreach (NPC minion in _minionList)
                {
                    HealReceivedEvents.AddRange(minion.EXTHealing.GetIncomingHealEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                HealReceivedEvents = HealReceivedEvents.OrderBy(x => x.Time).ToList();
                HealReceivedEventsBySrc = HealReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (HealReceivedEventsBySrc.TryGetValue(target.AgentItem, out List<EXTAbstractHealingEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<EXTAbstractHealingEvent>();
                }
            }
            return HealReceivedEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }
    }
}
