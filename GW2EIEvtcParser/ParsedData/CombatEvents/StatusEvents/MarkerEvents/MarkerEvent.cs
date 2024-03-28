namespace GW2EIEvtcParser.ParsedData
{
    public class MarkerEvent : AbstractMarkerEvent
    {
        public int MarkerID { get; }
        public long EndTime { get; protected set; } = long.MaxValue;

        internal MarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            MarkerID = evtcItem.Value;
        }

        internal void SetEndTime(long endTime)
        {
            // Sanity check
            if (EndTime != long.MaxValue)
            {
                return;
            }
            EndTime = endTime;
        }

    }
}
