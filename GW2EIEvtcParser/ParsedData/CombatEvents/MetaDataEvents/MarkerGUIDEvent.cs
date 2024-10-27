using System.Linq;

namespace GW2EIEvtcParser.ParsedData;

public class MarkerGUIDEvent : IDToGUIDEvent
{
    internal static MarkerGUIDEvent DummyMarkerGUID = new MarkerGUIDEvent();

    public bool IsCommanderTag { get; }
internal MarkerGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
        IsCommanderTag = MarkerGUIDs.CommanderTagMarkersHexGUIDs.Contains(HexContentGUID);
        if (evtcVersion.Build >= ArcDPSEnums.ArcDPSBuilds.ExtraDataInGUIDEvents)
        {
            IsCommanderTag |= evtcItem.SrcInstid == 1;
        }
    }

    internal MarkerGUIDEvent() : base()
    {
    }

}
