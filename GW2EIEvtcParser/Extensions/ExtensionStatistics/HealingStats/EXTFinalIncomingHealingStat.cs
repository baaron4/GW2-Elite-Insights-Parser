using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTFinalIncomingHealingStat
    {
        public int Healed { get; }
        public int HealingPowerHealed { get; }
        public int ConversionHealed { get; }
        public int HybridHealed { get; }
        public int DownedHealed { get; }

        internal EXTFinalIncomingHealingStat(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            foreach (EXTAbstractHealingEvent healingEvent in actor.EXTHealing.GetIncomingHealEvents(target, log, start, end))
            {
                Healed += healingEvent.HealingDone;
                switch (healingEvent.GetHealingType(log))
                {
                    case EXTHealingType.ConversionBased:
                        ConversionHealed += healingEvent.HealingDone;
                        break;
                    case EXTHealingType.Hybrid:
                        HybridHealed += healingEvent.HealingDone;
                        break;
                    case EXTHealingType.HealingPower:
                        HealingPowerHealed += healingEvent.HealingDone;
                        break;
                    default:
                        break;
                }
                if (healingEvent.AgainstDowned)
                {
                    DownedHealed += healingEvent.HealingDone;
                }
            }
        }

    }

}
