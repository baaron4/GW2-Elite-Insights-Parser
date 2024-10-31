using System.Linq;

namespace GW2EIEvtcParser.ParsedData
{
    public class MarkerGUIDEvent : IDToGUIDEvent
    {
        internal static MarkerGUIDEvent DummyMarkerGUID = new MarkerGUIDEvent();

        private readonly bool _isCommanderTag = false;

        public bool IsCommanderTag => _isCommanderTag || MarkerGUIDs.CommanderTagMarkersHexGUIDs.Contains(HexContentGUID);
        internal MarkerGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
        {
            if (evtcVersion.Build >= ArcDPSEnums.ArcDPSBuilds.ExtraDataInGUIDEvents)
            {
                _isCommanderTag = evtcItem.SrcInstid == 1;
            }
        }
        internal MarkerGUIDEvent() : base()
        {
        }

    }
}
