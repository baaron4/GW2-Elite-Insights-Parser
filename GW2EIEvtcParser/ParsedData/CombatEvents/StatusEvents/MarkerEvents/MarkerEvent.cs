namespace GW2EIEvtcParser.ParsedData;

public class MarkerEvent : AbstractStatusEvent
{
    /// <summary>
    /// ID of the marker. Match to stable GUID with <see cref="MarkerGUIDEvent"/>.
    /// </summary>
    public readonly int MarkerID;

    /// <summary>
    /// GUID event of the effect
    /// </summary>
    public MarkerGUIDEvent? GUIDEvent { get; private set; }

    /// <summary>
    /// Time at which marker has been removed.
    /// </summary>
    public long EndTime { get; protected set; } = int.MaxValue;

    internal bool IsEnd => MarkerID == 0;

    internal bool EndNotSet => EndTime == int.MaxValue;

    internal MarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        MarkerID = evtcItem.Value;
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

    internal void SetGUIDEvent(CombatData combatData)
    {
        GUIDEvent = combatData.GetMarkerGUIDEvent(MarkerID);
    }

}
