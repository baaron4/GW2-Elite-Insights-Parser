using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions;

public class EXTFinalIncomingHealingStat
{
    public readonly int Healed;
    public readonly int HealingPowerHealed;
    public readonly int ConversionHealed;
    public readonly int HybridHealed;
    public readonly int DownedHealed;

    internal EXTFinalIncomingHealingStat(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? target)
    {
        foreach (EXTHealingEvent healingEvent in actor.EXTHealing.GetIncomingHealEvents(target, log, start, end))
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
