using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    // to match non generic support stats
    public class FinalPlayerSupport
    {
        //public long allHeal;
        public long Resurrects { get; set; }
        public double ResurrectTime { get; set; }
        public long CondiCleanse { get; set; }
        public double CondiCleanseTime { get; set; }
        public long CondiCleanseSelf { get; set; }
        public double CondiCleanseTimeSelf { get; set; }
        public long BoonStrips { get; set; }
        public double BoonStripsTime { get; set; }
    }
}
