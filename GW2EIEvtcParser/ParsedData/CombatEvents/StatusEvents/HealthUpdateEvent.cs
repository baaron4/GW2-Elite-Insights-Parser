using System;
using GW2EIEvtcParser.Interfaces;

namespace GW2EIEvtcParser.ParsedData
{
    public class HealthUpdateEvent : AbstractStatusEvent, IStateable
    {
        public double HPPercent { get; }

        internal HealthUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            HPPercent = Math.Round(evtcItem.DstAgent / 100.0, 2);
            if (HPPercent > 100.0)
            {
                HPPercent = 100;
            }
        }

        public (long start, double value) ToState()
        {
            return (Time, HPPercent);
        }
    }
}
