using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.Extensions;

public class EXTFinalOutgoingBarrierStat
{
    public readonly int Bps;
    public readonly int Barrier;
    private readonly int DownedBps;
    private readonly int DownedBarrier;

    public readonly int ActorBps;
    public readonly int ActorBarrier;
    private readonly int ActorDownedBps;
    private readonly int ActorDownedBarrier;

    internal EXTFinalOutgoingBarrierStat(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? target)
    {
        (Barrier, DownedBarrier) = ComputeBarrierFrom(actor.EXTBarrier.GetOutgoingBarrierEvents(target, log, start, end));
        (ActorBarrier, ActorDownedBarrier) = ComputeBarrierFrom(actor.EXTBarrier.GetJustActorOutgoingBarrierEvents(target, log, start, end));
        double phaseDuration = (end - start) / 1000.0;
        if (phaseDuration > 0)
        {
            Bps = (int)Math.Round(Barrier / phaseDuration);
            DownedBps = (int)Math.Round(DownedBarrier / phaseDuration);

            ActorBps = (int)Math.Round(ActorBarrier / phaseDuration);
            ActorDownedBps = (int)Math.Round(ActorDownedBarrier / phaseDuration);
        }
    }

    private static (int barrier, int downedBarrier) ComputeBarrierFrom(IEnumerable<EXTBarrierEvent> barrierEvents)
    {
        int barrier = 0;
        int downedBarrier = 0;
        foreach (EXTBarrierEvent barrierEvent in barrierEvents)
        {
            barrier += barrierEvent.BarrierGiven;
            if (barrierEvent.AgainstDowned)
            {
                downedBarrier += barrierEvent.BarrierGiven;
            }
        }
        return (barrier, downedBarrier);
    }

}
