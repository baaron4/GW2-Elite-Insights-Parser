namespace GW2EIEvtcParser.ParsedData;

public class TransformationEvent : StatusEvent
{
    /// <summary>
    /// ID of the transformation. Match to stable GUID with <see cref="TransformationGUIDEvent"/>.
    /// </summary>
    public readonly uint TransformationID;

    /// <summary>
    /// GUID event of the transformation
    /// </summary>
    public TransformationGUIDEvent GUIDEvent { get; private set; } = TransformationGUIDEvent.DummyTransformationGUID;

    /// <summary>
    /// Time at which marker has been removed.
    /// </summary>
    public long EndTime { get; protected set; } = int.MaxValue;

    internal bool IsEnd => TransformationID == 0;

    internal bool EndNotSet => EndTime == int.MaxValue;

    internal TransformationEvent(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, TransformationGUIDEvent> transformationGUIDs) : base(evtcItem, agentData)
    {
        TransformationID = evtcItem.SkillID;
        if (transformationGUIDs.TryGetValue(TransformationID, out var markerGUID))
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
