using System;
using GW2EIEvtcParser.Interfaces;

namespace GW2EIEvtcParser.ParsedData
{
    public class HealthUpdateEvent : AbstractStatusEvent, IStateable
    {
        public double HealthPercent { get; }

        internal HealthUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            HealthPercent = GetHealthPercent(evtcItem);
            if (HealthPercent > 100.0)
            {
                HealthPercent = 100;
            }
        }

        internal static double GetHealthPercent(CombatItem evtcItem)
        {
            return Math.Round(evtcItem.DstAgent / 100.0, 2);
        }

        public (long start, double value) ToState()
        {
            return (Time, HealthPercent);
        }
    }
}
