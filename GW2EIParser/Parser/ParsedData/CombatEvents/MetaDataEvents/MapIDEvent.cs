namespace GW2EIParser.Parser.ParsedData.CombatEvents
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
