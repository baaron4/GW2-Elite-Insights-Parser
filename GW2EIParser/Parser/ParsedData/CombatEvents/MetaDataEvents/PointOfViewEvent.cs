namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class PointOfViewEvent : AbstractMetaDataEvent
    {
        public AgentItem PoV { get; }

        public PointOfViewEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            PoV = agentData.GetAgent(evtcItem.SrcAgent);
        }

    }
}
