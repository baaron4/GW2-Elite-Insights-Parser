namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class TagEvent : AbstractStatusEvent
    {
        public int TagID { get; }

        public TagEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            TagID = evtcItem.Value;
            Src.SetCommanderTag(this);
        }

    }
}
