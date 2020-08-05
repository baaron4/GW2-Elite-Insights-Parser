namespace GW2EIEvtcParser.ParsedData
{
    public class PointOfViewEvent : AbstractMetaDataEvent
    {
        public AgentItem PoV { get; }

        internal PointOfViewEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            PoV = agentData.GetAgent(evtcItem.SrcAgent);
        }

    }
}
