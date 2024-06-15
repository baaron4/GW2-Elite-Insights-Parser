using System;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTFinalOutgoingHealingStat
    {
        public int Hps { get; }
        public int Healing { get; }
        public int HealingPowerHps { get; }
        public int HealingPowerHealing { get; }
        public int ConversionHps { get; }
        public int ConversionHealing { get; }
        public int HybridHps { get; }
        public int HybridHealing { get; }
        public int DownedHps { get; }
        public int DownedHealing { get; }

        public int ActorHps { get; }
        public int ActorHealing { get; }
        public int ActorHealingPowerHps { get; }
        public int ActorHealingPowerHealing { get; }
        public int ActorConversionHps { get; }
        public int ActorConversionHealing { get; }
        public int ActorHybridHps { get; }
        public int ActorHybridHealing { get; }
        public int ActorDownedHps { get; }
        public int ActorDownedHealing { get; }

        internal EXTFinalOutgoingHealingStat(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            (Healing, HealingPowerHealing, ConversionHealing, HybridHealing, DownedHealing) = ComputeHealingFrom(log, actor.EXTHealing.GetOutgoingHealEvents(target, log, start, end));
            (ActorHealing, ActorHealingPowerHealing, ActorConversionHealing, ActorHybridHealing, ActorDownedHealing) = ComputeHealingFrom(log, actor.EXTHealing.GetJustActorOutgoingHealEvents(target, log, start, end));
            double phaseDuration = (end - start) / 1000.0;
            if (phaseDuration > 0)
            {
                Hps = (int)Math.Round(Healing / phaseDuration);
                HealingPowerHps = (int)Math.Round(HealingPowerHealing / phaseDuration);
                ConversionHps = (int)Math.Round(ConversionHealing / phaseDuration);
                HybridHps = (int)Math.Round(HybridHealing / phaseDuration);
                DownedHps = (int)Math.Round(DownedHealing / phaseDuration);

                ActorHps = (int)Math.Round(ActorHealing / phaseDuration);
                ActorHealingPowerHps = (int)Math.Round(ActorHealingPowerHealing / phaseDuration);
                ActorConversionHps = (int)Math.Round(ActorConversionHealing / phaseDuration);
                ActorHybridHps = (int)Math.Round(ActorHybridHealing / phaseDuration);
                ActorDownedHps = (int)Math.Round(ActorDownedHealing / phaseDuration);
            }
        }

        private static (int healing, int healingPowerHealing, int conversionHealing, int hybridHealing, int downedHealing) ComputeHealingFrom(ParsedEvtcLog log, IReadOnlyList<EXTAbstractHealingEvent> healingEvents)
        {
            int healing = 0;
            int healingPowerHealing = 0;
            int conversionhealing = 0;
            int hybridHealing = 0;
            int downedHealing = 0;
            foreach (EXTAbstractHealingEvent healingEvent in healingEvents)
            {
                healing += healingEvent.HealingDone;
                switch (healingEvent.GetHealingType(log))
                {
                    case EXTHealingType.ConversionBased:
                        conversionhealing += healingEvent.HealingDone;
                        break;
                    case EXTHealingType.Hybrid:
                        hybridHealing += healingEvent.HealingDone;
                        break;
                    case EXTHealingType.HealingPower:
                        healingPowerHealing += healingEvent.HealingDone;
                        break;
                    default:
                        break;
                }
                if (healingEvent.AgainstDowned)
                {
                    downedHealing += healingEvent.HealingDone;
                }
            }
            return (healing, healingPowerHealing, conversionhealing, hybridHealing, downedHealing);
        }

    }

}
