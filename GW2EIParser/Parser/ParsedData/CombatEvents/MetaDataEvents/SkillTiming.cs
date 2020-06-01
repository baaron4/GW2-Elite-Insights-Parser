using System;
using System.Collections.Generic;
using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class SkillTiming
    {

        public ulong Action { get; }

        public ulong AtMillisecond { get; }

        public SkillTiming(CombatItem evtcItem)
        {
            Action = evtcItem.SrcAgent;
            AtMillisecond = evtcItem.DstAgent;
        }

    }
}
