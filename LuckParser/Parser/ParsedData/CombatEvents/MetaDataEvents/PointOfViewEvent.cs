namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class PointOfViewEvent : AbstractMetaDataEvent
    {
        public AgentItem PoV { get; }

        public PointOfViewEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, offset)
        {
            PoV = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.LogTime);
        }

    }
}
