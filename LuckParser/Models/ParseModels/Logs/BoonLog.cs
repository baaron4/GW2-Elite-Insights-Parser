using LuckParser.Parser;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonLog
    {
        public long Time { get; }
        public long Value { get; }
        public ushort SrcInstid { get; }

        protected BoonLog(long time, ushort srcInstid, long value)
        {
            Time = time;
            Value = value;
            SrcInstid = srcInstid;
        }

        public abstract void UpdateSimulator(BoonSimulator simulator);
    }
}