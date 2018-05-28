using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BoonLog
    {
        // Fields
        private long time = 0;
        private long value = 0;
        private ushort overstack = 0;
        private ushort src_instid = 0;

        // Constructor
        public BoonLog(long time, ushort src_instid, long value)
        {
            this.time = time;
            this.value = value;
            this.src_instid = src_instid;
        }
        public BoonLog(long time, ushort src_instid, long value, ushort overstack)
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
        public ushort getOverstack() {
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
        public void addOverstack(ushort overstack)
        {
            this.overstack += overstack;
        }
    }
}