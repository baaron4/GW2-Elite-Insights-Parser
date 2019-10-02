using System;
using LuckParser.EIData;

namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class BuffRemoveManualEvent : AbstractBuffRemoveEvent
    {
        public BuffRemoveManualEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
        }

        public BuffRemoveManualEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill) : base(by, to, time, removedDuration, buffSkill)
        {
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd)
        {
            return false; // don't consider manual remove events
        }

        public override void UpdateSimulator(BuffSimulator simulator)
        {
        }
        public override int CompareTo(AbstractBuffEvent abe)
        {
            throw new InvalidOperationException("Manual removes can't be sorted");
        }
    }
}
