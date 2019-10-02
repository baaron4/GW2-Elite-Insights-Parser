namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class MapIDEvent : AbstractMetaDataEvent
    {
        public int MapID { get; }

        public MapIDEvent(CombatItem evtcItem, long offset) : base(evtcItem, offset)
        {
            MapID = (int)evtcItem.SrcAgent;
        }

    }
}
