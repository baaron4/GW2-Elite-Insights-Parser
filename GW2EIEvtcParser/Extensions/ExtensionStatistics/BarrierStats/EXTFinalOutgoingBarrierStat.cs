using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTFinalOutgoingBarrierStat
    {
        public int Bps { get; }
        public int Barrier { get; }
        private int DownedBps { get; }
        private int DownedBarrier { get;}

        public int ActorHps { get; }
        public int ActorHealing { get; }
        private int ActorDownedHps { get; }
        private int ActorDownedHealing { get; }

        internal EXTFinalOutgoingBarrierStat(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            (Barrier, DownedBarrier) = ComputeHealingFrom(log, actor.EXTBarrier.GetOutgoingBarrierEvents(target, log, start, end));
            (ActorHealing, ActorDownedHealing) = ComputeHealingFrom(log, actor.EXTBarrier.GetJustActorOutgoingHealEvents(target, log, start, end));
            double phaseDuration = (end - start) / 1000.0;
            if (phaseDuration > 0)
            {
                Bps = (int)Math.Round(Barrier / phaseDuration);
                DownedBps = (int)Math.Round(DownedBarrier / phaseDuration);

                ActorHps = (int)Math.Round(ActorHealing / phaseDuration);
                ActorDownedHps = (int)Math.Round(ActorDownedHealing / phaseDuration);
            }
        }

        private static (int healing, int downedHealing) ComputeHealingFrom(ParsedEvtcLog log, IReadOnlyList<EXTAbstractBarrierEvent> barrierEvents)
        {
            int healing = 0;
            int downedHealing = 0;
            foreach (EXTAbstractBarrierEvent barrierEvent in barrierEvents)
            {
                healing += barrierEvent.BarrierGiven;
                if (barrierEvent.AgainstDowned)
                {
                    downedHealing += barrierEvent.BarrierGiven;
                }
            }
            return (healing, downedHealing);
        }

    }
    
}
