namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractTimeCombatEvent
    {
        public long Time { get; protected set; }

        internal AbstractTimeCombatEvent(long time)
        {
            Time = time;
        }

        internal void OverrideTime(long time)
        {
            Time = time;
        }
    }
}
