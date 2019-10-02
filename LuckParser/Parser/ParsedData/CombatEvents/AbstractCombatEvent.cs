namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class AbstractCombatEvent
    {
        public long Time { get; protected set; }

        protected AbstractCombatEvent(long logTime, long offset)
        {
            Time = logTime - offset;
        }
    }
}
