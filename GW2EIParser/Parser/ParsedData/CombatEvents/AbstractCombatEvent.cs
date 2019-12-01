namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractCombatEvent
    {
        public long Time { get; protected set; }

        protected AbstractCombatEvent(long logTime, long offset)
        {
            Time = logTime - offset;
        }
    }
}
