using System;
using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffRemoveManualEvent : AbstractBuffRemoveEvent
    {
        public BuffRemoveManualEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

        public BuffRemoveManualEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill) : base(by, to, time, removedDuration, buffSkill)
        {
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return false; // don't consider manual remove events
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
        }
        public override int CompareTo(AbstractBuffEvent abe)
        {
            throw new InvalidOperationException("Manual removes can't be sorted");
        }
    }
}
