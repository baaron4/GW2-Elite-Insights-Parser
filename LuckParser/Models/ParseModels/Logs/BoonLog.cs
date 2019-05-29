using LuckParser.Parser;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonLog
    {
        public long Time { get; }
        protected long Value;
        protected AgentItem Src;

        protected BoonLog(long time, AgentItem src, long value)
        {
            Time = time;
            Value = value;
            Src = src;
        }

        public abstract void UpdateSimulator(BoonSimulator simulator);
    }
}