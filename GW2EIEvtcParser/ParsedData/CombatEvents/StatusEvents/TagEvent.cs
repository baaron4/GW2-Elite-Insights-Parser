namespace GW2EIEvtcParser.ParsedData
{
    public class TagEvent : AbstractStatusEvent
    {
        public int TagID { get; }

        internal TagEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            TagID = evtcItem.Value;
        }

    }
}
