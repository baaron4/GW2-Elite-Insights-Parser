using System;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BreakbarPercentEvent : AbstractStatusEvent
    {
        public float BreakbarPercent { get; }

        public BreakbarPercentEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            BreakbarPercent = Convert.ToSingle(evtcItem.Value);
        }

    }
}
