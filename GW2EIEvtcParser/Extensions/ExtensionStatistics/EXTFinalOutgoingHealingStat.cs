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
        public double Hps { get; internal set; }
        public int Healing { get; internal set; }
        public double HealingPowerHps { get; internal set; }
        public int HealingPowerHealing { get; internal set; }
        public double ConversionHps { get; internal set; }
        public int ConversionHealing { get; internal set; }

        internal EXTFinalOutgoingHealingStat(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            double phaseDuration = (end - start) / 1000.0;
            foreach (EXTAbstractHealingEvent healingEvent in actor.EXTHealing.GetOutgoingHealEvents(target, log, start, end))
            {
                Healing += healingEvent.HealingDone;
                switch (healingEvent.GetHealingType(log))
                {
                    case EXTHealingType.ConversionBased:
                        ConversionHealing += healingEvent.HealingDone;
                        break;
                    case EXTHealingType.HealingPower:
                        HealingPowerHealing += healingEvent.HealingDone;
                        break;
                    default:
                        break;
                }
            }
            if (phaseDuration > 0)
            {
                Hps = Healing / phaseDuration;
                HealingPowerHps = HealingPowerHealing / phaseDuration;
                ConversionHps = ConversionHealing / phaseDuration;
            }
        }

    }
    
}
