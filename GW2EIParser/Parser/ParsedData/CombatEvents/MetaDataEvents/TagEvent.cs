namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class TagEvent : AbstractMetaDataEvent
    {
        public int TagID { get; }
        public AgentItem Src { get; }

        public TagEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            TagID = evtcItem.Value;
            Src = agentData.GetAgent(evtcItem.SrcAgent);
            Src.SetCommanderTag(this);
        }

    }
}
