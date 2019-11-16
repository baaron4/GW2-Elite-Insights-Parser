using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{

    public enum BuffEnum { Self, Group, OffGroup, Squad };
    public class FinalPlayerBuffs
    {
        public double Uptime { get; set; }
        public double Generation { get; set; }
        public double Overstack { get; set; }
        public double Wasted { get; set; }
        public double UnknownExtended { get; set; }
        public double ByExtension { get; set; }
        public double Extended { get; set; }
        public double Presence { get; set; }
    }

}
