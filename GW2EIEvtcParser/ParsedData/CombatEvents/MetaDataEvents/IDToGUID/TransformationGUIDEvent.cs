namespace GW2EIEvtcParser.ParsedData;

public class TransformationGUIDEvent : IDToGUIDEvent
{
    internal static TransformationGUIDEvent DummyTransformationGUID = new();
    public uint TransformationID => (uint)ContentID;
    internal TransformationGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
    }

    private TransformationGUIDEvent() : base()
    {
    }
}

