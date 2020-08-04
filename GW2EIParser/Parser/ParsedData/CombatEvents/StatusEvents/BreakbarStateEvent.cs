using System;
using GW2EIUtils;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BreakbarStateEvent : AbstractStatusEvent
    {     

        public ArcDPSEnums.BreakbarState State { get; }

        public BreakbarStateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            State = ArcDPSEnums.GetBreakbarState(evtcItem.Value);
        }

    }
}
