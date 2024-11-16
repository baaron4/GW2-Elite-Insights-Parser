using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.Extensions;

public class EXTFinalIncomingBarrierStat
{
    public readonly int BarrierReceived;
    public readonly int DownedBarrierReceived;

    internal EXTFinalIncomingBarrierStat(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? target)
    {
        foreach (EXTBarrierEvent barrierEvent in actor.EXTBarrier.GetIncomingBarrierEvents(target, log, start, end))
        {
            BarrierReceived += barrierEvent.BarrierGiven;
            if (barrierEvent.AgainstDowned)
            {
                DownedBarrierReceived += barrierEvent.BarrierGiven;
            }
        }
    }

}
