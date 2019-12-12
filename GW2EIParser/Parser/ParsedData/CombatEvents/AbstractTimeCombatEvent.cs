namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractTimeCombatEvent
    {
        public long Time { get; protected set; }

        protected AbstractTimeCombatEvent(long time)
        {
            Time = time;
        }
    }
}
