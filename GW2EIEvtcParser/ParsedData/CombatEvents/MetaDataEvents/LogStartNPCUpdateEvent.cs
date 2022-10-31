namespace GW2EIEvtcParser.ParsedData
{
    public class LogStartNPCUpdateEvent : LogDateEvent
    {
        public int AgentID { get; }

        internal LogStartNPCUpdateEvent(CombatItem evtcItem) : base(evtcItem)
        {
            AgentID = (ushort)evtcItem.SrcAgent;
        }

    }
}
