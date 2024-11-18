using GW2EIEvtcParser.Interfaces;

namespace GW2EIEvtcParser.ParsedData;

public class BarrierUpdateEvent : StatusEvent, IStateable
{
    public readonly double BarrierPercent;

    internal BarrierUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        BarrierPercent = GetBarrierPercent(evtcItem);
        if (BarrierPercent > 100.0)
        {
            BarrierPercent = 0;
        }
    }

    internal static double GetBarrierPercent(CombatItem evtcItem)
    {
        return Math.Round(evtcItem.DstAgent / 100.0, 2);
    }

    public (long start, double value) ToState()
    {
        return (Time, BarrierPercent);
    }
}
