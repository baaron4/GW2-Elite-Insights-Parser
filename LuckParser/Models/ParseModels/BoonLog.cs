using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BoonLog
    {
        // Fields
        private int time = 0;
        private int value = 0;
        private int overstack = 0;

        // Constructor
        public BoonLog(int time, int value)
        {
            this.time = time;
            this.value = value;
        }
        public BoonLog(int time, int value,int overstack)
        {
            this.time = time;
            this.value = value;
            this.overstack = overstack;
        }

        // Getters
        public int getTime()
        {
            return time;
        }

        public int getValue()
        {
            return value;
        }
        public int getOverstack() {
            return overstack;
        }
    }
}