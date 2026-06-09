namespace GW2EIEvtcParser.ParsedData;

public class GadgetCaptureEvent : StatusEvent
{
    public long EndTime { get; private set; } = long.MinValue;
    public bool EndIsSet => EndTime != long.MinValue;

    public IReadOnlyList<(long Time, double progress)> Progress => _progress;
    private readonly List<(long Time, double progress)> _progress = [];
    internal GadgetCaptureEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        EndTime = evtcItem.Time;
    }

    internal void SetEnd(CombatItem evtcItem)
    {
        if (EndIsSet)
        {
            return;
        }
        EndTime = evtcItem.Time;
    }

    internal void AddProgress(CombatItem evtcItem)
    {
        if (EndIsSet)
        {
            return;
        }
    }

}
