namespace LuckParser.Models.ParseModels
{
    public class BoonLog
    {
        // Fields
        private long time = 0;
        private long value = 0;
        private uint overstack = 0;
        private ushort src_instid = 0;

        //Constructor
        public BoonLog(long time, ushort src_instid, long value, uint overstack)
        {
            this.time = time;
            this.value = value;
            this.src_instid = src_instid;
            this.overstack = overstack;
        }

        // Getters
        public long getTime()
        {
            return time;
        }

        public long getValue()
        {
            return value;
        }
        public uint getOverstack() {
            return overstack;
        }

        public ushort getSrcInstid()
        {
            return src_instid;
        }

        // Add
        public void addTime(long time)
        {
            this.time += time;
        }

        public void addValue(long value)
        {
            this.value += value;
        }
        public void addOverstack(uint overstack)
        {
            this.overstack += overstack;
        }
    }
}