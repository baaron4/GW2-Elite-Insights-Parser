namespace LuckParser.Models.ParseModels
{
    public class BoonLog
    {
        // Fields
        private long _time;
        private long _value;
        private uint _overstack;
        private readonly ushort _srcInstid;

        //Constructor
        public BoonLog(long time, ushort srcInstid, long value, uint overstack)
        {
            _time = time;
            _value = value;
            _srcInstid = srcInstid;
            _overstack = overstack;
        }

        // Getters
        public long GetTime()
        {
            return _time;
        }

        public long GetValue()
        {
            return _value;
        }
        public uint GetOverstack() {
            return _overstack;
        }

        public ushort GetSrcInstid()
        {
            return _srcInstid;
        }

        // Add
        public void AddTime(long time)
        {
            _time += time;
        }

        public void AddValue(long value)
        {
            _value += value;
        }
        public void AddOverstack(uint overstack)
        {
            _overstack += overstack;
        }
    }
}