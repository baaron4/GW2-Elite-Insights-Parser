namespace GW2EIEvtcParser.ParsedData
{
    public class TagEvent : AbstractMetaDataEvent
    {
        public int TagID { get; }
        public AgentItem Src { get; }

        internal TagEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            TagID = evtcItem.Value;
            Src = agentData.GetAgent(evtcItem.SrcAgent);
            Src.SetCommanderTag(this);
        }

    }
}
