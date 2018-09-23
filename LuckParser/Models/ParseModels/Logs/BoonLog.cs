using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonLog
    {
        public long Time { get; private set; }
        public long Value { get; private set; }
        public ushort SrcInstid { get; }

        protected BoonLog(long time, ushort srcInstid, long value)
        {
            Time = time;
            Value = value;
            SrcInstid = srcInstid;
        }

        public void AddTime(long time)
        {
            Time += time;
        }

        public void AddValue(long value)
        {
            Value += value;
        }

        public abstract ParseEnum.BuffRemove GetRemoveType();
    }
}