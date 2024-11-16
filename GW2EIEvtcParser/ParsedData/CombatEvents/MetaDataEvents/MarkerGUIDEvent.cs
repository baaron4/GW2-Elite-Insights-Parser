namespace GW2EIEvtcParser.ParsedData;

public class MarkerGUIDEvent : IDToGUIDEvent
{
    internal static MarkerGUIDEvent DummyMarkerGUID = new MarkerGUIDEvent();

    public readonly bool IsCommanderTag;
    internal MarkerGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
        IsCommanderTag = MarkerGUIDs.CommanderTagMarkersHexGUIDs.Contains(ContentGUID);

        if (evtcVersion.Build >= ArcDPSEnums.ArcDPSBuilds.ExtraDataInGUIDEvents)
        {
            IsCommanderTag |= evtcItem.SrcInstid == 1;
        }
    }

    internal MarkerGUIDEvent() : base()
    {
    }

}
