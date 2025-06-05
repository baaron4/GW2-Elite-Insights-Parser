using GW2EIEvtcParser.Interfaces;

namespace GW2EIEvtcParser.ParsedData;

public class BreakbarPercentEvent : StatusEvent, IStateable
{
    public readonly double BreakbarPercent;

    internal BreakbarPercentEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        // 4 bytes
        BreakbarPercent = Math.Round(100.0 * BitConverter.Int32BitsToSingle(evtcItem.Value), 2);
        if (BreakbarPercent > 100.0)
        {
            BreakbarPercent = 100;
        }
    }

    public (long start, double value) ToState()
    {
        return (Time, BreakbarPercent);
    }
}
