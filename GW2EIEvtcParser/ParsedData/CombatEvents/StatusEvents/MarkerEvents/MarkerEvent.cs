namespace GW2EIEvtcParser.ParsedData;

public class MarkerEvent : StatusEvent
{
    /// <summary>
    /// ID of the marker. Match to stable GUID with <see cref="MarkerGUIDEvent"/>.
    /// </summary>
    public readonly int MarkerID;

    /// <summary>
    /// GUID event of the effect
    /// </summary>
    public MarkerGUIDEvent GUIDEvent { get; private set; } = MarkerGUIDEvent.DummyMarkerGUID;

    /// <summary>
    /// Time at which marker has been removed.
    /// </summary>
    public long EndTime { get; protected set; } = int.MaxValue;

    internal bool IsEnd => MarkerID == 0;

    internal bool EndNotSet => EndTime == int.MaxValue;

    internal MarkerEvent(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, MarkerGUIDEvent> markerGUIDs) : base(evtcItem, agentData)
    {
        MarkerID = evtcItem.Value;
        if (markerGUIDs.TryGetValue(MarkerID, out var markerGUID))
        {
            GUIDEvent = markerGUID;
        }
    }

    internal void SetEndTime(long endTime)
    {
        // Sanity check
        if (!EndNotSet)
        {
            return;
        }
        EndTime = endTime;
    }

}
