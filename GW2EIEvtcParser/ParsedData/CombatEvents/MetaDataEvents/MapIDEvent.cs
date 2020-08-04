namespace GW2EIEvtcParser.ParsedData
{
    public class MapIDEvent : AbstractMetaDataEvent
    {
        public int MapID { get; }

        public MapIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
            MapID = (int)evtcItem.SrcAgent;
        }

    }
}
