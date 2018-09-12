namespace LuckParser.Models.ParseModels
{
    public class BoonLog
    {
        public long Time { get; private set; }
        public long Value { get; private set; }
        public uint Overstack { get; private set; }
        public ushort SrcInstid { get; }

        public BoonLog(long time, ushort srcInstid, long value, uint overstack)
        {
            Time = time;
            Value = value;
            SrcInstid = srcInstid;
            Overstack = overstack;
        }

        public void AddTime(long time)
        {
            Time += time;
        }

        public void AddValue(long value)
        {
            Value += value;
        }

        public void AddOverstack(uint overstack)
        {
            Overstack += overstack;
        }
    }
}