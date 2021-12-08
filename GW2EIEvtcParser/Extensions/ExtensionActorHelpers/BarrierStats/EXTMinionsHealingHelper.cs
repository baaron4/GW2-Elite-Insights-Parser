using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTMinionsBarrierHelper : EXTActorBarrierHelper
    {
        private readonly Minions _minions;
        private IReadOnlyList<NPC> _minionList => _minions.MinionList;

        internal EXTMinionsBarrierHelper(Minions minions) : base()
        {
            _minions = minions;
        }


        public override IReadOnlyList<EXTAbstractBarrierEvent> GetOutgoingBarrierEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (HealEvents == null)
            {
                HealEvents = new List<EXTAbstractBarrierEvent>();
                foreach (NPC minion in _minionList)
                {
                    HealEvents.AddRange(minion.EXTBarrier.GetOutgoingBarrierEvents(null, log, 0, log.FightData.FightEnd));
                }
                HealEvents = HealEvents.OrderBy(x => x.Time).ToList();
                HealEventsByDst = HealEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (HealEventsByDst.TryGetValue(target.AgentItem, out List<EXTAbstractBarrierEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<EXTAbstractBarrierEvent>();
                }
            }
            return HealEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<EXTAbstractBarrierEvent> GetIncomingBarrierEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (HealReceivedEvents == null)
            {
                HealReceivedEvents = new List<EXTAbstractBarrierEvent>();
                foreach (NPC minion in _minionList)
                {
                    HealReceivedEvents.AddRange(minion.EXTBarrier.GetIncomingBarrierEvents(null, log, 0, log.FightData.FightEnd));
                }
                HealReceivedEvents = HealReceivedEvents.OrderBy(x => x.Time).ToList();
                HealReceivedEventsBySrc = HealReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (HealReceivedEventsBySrc.TryGetValue(target.AgentItem, out List<EXTAbstractBarrierEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<EXTAbstractBarrierEvent>();
                }
            }
            return HealReceivedEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }
    }
}
