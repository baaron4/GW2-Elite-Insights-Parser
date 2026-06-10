namespace GW2EIEvtcParser.ParsedData;

public class GadgetCaptureEvent : StatusEvent
{
    public long EndTime { get; private set; } = long.MinValue;
    public bool EndIsSet => EndTime != long.MinValue;

    public readonly byte OriginalOwner;

    public IReadOnlyList<(long Time, float Progress, byte From, byte By)> Progress => _progress;
    private readonly List<(long Time, float Progress, byte From, byte By)> _progress = [];
    internal GadgetCaptureEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        OriginalOwner = evtcItem.IsBuff;
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
        _progress.Add((evtcItem.Time, BitConverter.Int32BitsToSingle(evtcItem.Value), evtcItem.Result, evtcItem.IsBuff));
    }

}
