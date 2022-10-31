namespace GW2EIEvtcParser.ParsedData
{
    public class LogStartNPCUpdateEvent : LogDateEvent
    {
        public int SpeciesID { get; }

        internal LogStartNPCUpdateEvent(CombatItem evtcItem) : base(evtcItem)
        {
            SpeciesID = (ushort)evtcItem.SrcAgent;
        }

    }
}
