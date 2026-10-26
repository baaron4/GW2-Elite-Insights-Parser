namespace GW2EIEvtcParser.ParsedData;

using static GW2EIEvtcParser.ArcDPSEnums;

public class MarkerGUIDEvent : IDToGUIDEvent
{
    internal static MarkerGUIDEvent DummyMarkerGUID = new();
    public long MarkerID => ContentID;

    public readonly bool IsCommanderTag;
    internal MarkerGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        IsCommanderTag = MarkerGUIDs.CommanderTagMarkersHexGUIDs.Contains(GUID);

        if (evtcItem.Build >= ArcDPSBuilds.ExtraDataInGUIDEvents)
        {
            IsCommanderTag |= evtcItem.SrcInstid == 1;
        }
    }

    private MarkerGUIDEvent() : base()
    {
    }

}
