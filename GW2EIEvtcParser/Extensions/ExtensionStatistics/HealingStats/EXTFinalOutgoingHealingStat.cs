using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions;

public class EXTFinalOutgoingHealingStat
{
    public readonly int Hps;
    public readonly int Healing;
    public readonly int HealingPowerHps;
    public readonly int HealingPowerHealing;
    public readonly int ConversionHps;
    public readonly int ConversionHealing;
    public readonly int HybridHps;
    public readonly int HybridHealing;
    public readonly int DownedHps;
    public readonly int DownedHealing;

    public readonly int ActorHps;
    public readonly int ActorHealing;
    public readonly int ActorHealingPowerHps;
    public readonly int ActorHealingPowerHealing;
    public readonly int ActorConversionHps;
    public readonly int ActorConversionHealing;
    public readonly int ActorHybridHps;
    public readonly int ActorHybridHealing;
    public readonly int ActorDownedHps;
    public readonly int ActorDownedHealing;

    internal EXTFinalOutgoingHealingStat(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? target)
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

    private static (int healing, int healingPowerHealing, int conversionHealing, int hybridHealing, int downedHealing) ComputeHealingFrom(ParsedEvtcLog log, IEnumerable<EXTHealingEvent> healingEvents)
    {
        int healing = 0;
        int healingPowerHealing = 0;
        int conversionhealing = 0;
        int hybridHealing = 0;
        int downedHealing = 0;
        foreach (EXTHealingEvent healingEvent in healingEvents)
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
            }

            if (healingEvent.AgainstDowned)
            {
                downedHealing += healingEvent.HealingDone;
            }
        }
        return (healing, healingPowerHealing, conversionhealing, hybridHealing, downedHealing);
    }

}
