namespace GW2EIEvtcParser.ParsedData
{
    public class MapIDEvent : AbstractMetaDataEvent
    {
        public int MapID { get; }

        internal MapIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
            MapID = (int)evtcItem.SrcAgent;
        }

    }
}
