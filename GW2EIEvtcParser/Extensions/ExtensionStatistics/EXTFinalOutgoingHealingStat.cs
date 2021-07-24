using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTFinalOutgoingHealingStat
    {
        public int Hps { get; internal set; }
        public int Healing { get; internal set; }
        public int HealingPowerHps { get; internal set; }
        public int HealingPowerHealing { get; internal set; }
        public int ConversionHps { get; internal set; }
        public int ConversionHealing { get; internal set; }

        public int ActorHps { get; internal set; }
        public int ActorHealing { get; internal set; }
        public int ActorHealingPowerHps { get; internal set; }
        public int ActorHealingPowerHealing { get; internal set; }
        public int ActorConversionHps { get; internal set; }
        public int ActorConversionHealing { get; internal set; }

        internal EXTFinalOutgoingHealingStat(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            (Healing, HealingPowerHealing, ConversionHealing) = ComputeHealingFrom(log, actor.EXTHealing.GetOutgoingHealEvents(target, log, start, end));
            (ActorHealing, ActorHealingPowerHealing, ActorConversionHealing) = ComputeHealingFrom(log, actor.EXTHealing.GetJustActorOutgoingHealEvents(target, log, start, end));
            double phaseDuration = (end - start) / 1000.0;
            if (phaseDuration > 0)
            {
                Hps = (int)Math.Round(Healing / phaseDuration);
                HealingPowerHps = (int)Math.Round(HealingPowerHealing / phaseDuration);
                ConversionHps = (int)Math.Round(ConversionHealing / phaseDuration);
                ActorHps = (int)Math.Round(ActorHealing / phaseDuration);
                ActorHealingPowerHps = (int)Math.Round(ActorHealingPowerHealing / phaseDuration);
                ActorConversionHps = (int)Math.Round(ActorConversionHealing / phaseDuration);
            }
        }

        private static (int healing, int healingPowerHealing, int conversionhealing) ComputeHealingFrom(ParsedEvtcLog log, IReadOnlyList<EXTAbstractHealingEvent> damageEvents)
        {
            int healing = 0;
            int healingPowerHealing = 0;
            int conversionhealing = 0;
            foreach (EXTAbstractHealingEvent healingEvent in damageEvents)
            {
                healing += healingEvent.HealingDone;
                switch (healingEvent.GetHealingType(log))
                {
                    case EXTHealingType.ConversionBased:
                        conversionhealing += healingEvent.HealingDone;
                        break;
                    case EXTHealingType.HealingPower:
                        healingPowerHealing += healingEvent.HealingDone;
                        break;
                    default:
                        break;
                }
            }
            return (healing, healingPowerHealing, conversionhealing);
        }

    }
    
}
