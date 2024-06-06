namespace GW2EIEvtcParser.ParsedData
{
    public class MarkerEvent : AbstractStatusEvent
    {
        public int MarkerID { get; }
        public long EndTime { get; protected set; } = int.MaxValue;

        internal bool IsEnd => MarkerID == 0;

        internal bool EndNotSet => EndTime == int.MaxValue;

        internal MarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            MarkerID = evtcItem.Value;
        }

        internal void SetEndTime(long endTime)
        {
            // Sanity check
            if (!EndNotSet)
            {
                return;
            }
            EndTime = endTime;
        }

    }
}
