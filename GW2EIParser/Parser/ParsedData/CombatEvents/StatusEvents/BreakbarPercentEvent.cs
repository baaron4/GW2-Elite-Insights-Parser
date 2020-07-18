using System;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BreakbarPercentEvent : AbstractStatusEvent
    {
        public float BreakbarPercent { get; }

        public BreakbarPercentEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            byte[] bytes = new byte[sizeof(float)];
            int offset = 0;
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Value))
            {
                bytes[offset++] = bt;
            }
            BreakbarPercent = BitConverter.ToSingle(bytes, 0);
        }

    }
}
