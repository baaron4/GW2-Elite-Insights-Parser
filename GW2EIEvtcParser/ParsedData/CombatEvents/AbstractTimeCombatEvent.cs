namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractTimeCombatEvent
    {
        public long Time { get; protected set; }

        protected AbstractTimeCombatEvent(long time)
        {
            Time = time;
        }

        public void OverrideTime(long time)
        {
            Time = time;
        }
    }
}
