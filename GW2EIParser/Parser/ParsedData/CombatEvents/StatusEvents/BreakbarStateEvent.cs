using System;
using static GW2EIParser.Parser.ParseEnum;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BreakbarStateEvent : AbstractStatusEvent
    {     

        public BreakbarState State { get; }

        public BreakbarStateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            State = ParseEnum.GetBreakbarState(evtcItem.Value);
        }

    }
}
