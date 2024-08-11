namespace GW2EIEvtcParser.ParsedData
{
    public class LogNPCUpdateEvent : LogDateEvent
    {
        public int AgentID { get; }

        public long Time { get; }

        public AgentItem TriggerAgent { get; } = ParserHelper._unknownAgent;

        public bool TriggerIsGadget { get; }

        internal LogNPCUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            AgentID = (ushort)evtcItem.SrcAgent;
            if (evtcItem.DstAgent > 0)
            {
                TriggerIsGadget = evtcItem.IsFlanking > 0;
                TriggerAgent = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
            }
            Time = evtcItem.Time;
        }

    }
}
