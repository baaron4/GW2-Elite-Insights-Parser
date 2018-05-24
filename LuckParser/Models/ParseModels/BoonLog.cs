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
        private long overstack = 0;

        // Constructor
        public BoonLog(long time, long value)
        {
            this.time = time;
            this.value = value;
        }
        public BoonLog(long time, long value, long overstack)
        {
            this.time = time;
            this.value = value;
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
        public long getOverstack() {
            return overstack;
        }
    }
}