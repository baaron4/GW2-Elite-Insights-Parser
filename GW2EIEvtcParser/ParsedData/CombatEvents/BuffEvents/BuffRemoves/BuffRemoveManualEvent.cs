using System;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffRemoveManualEvent : AbstractBuffRemoveEvent
    {
        internal BuffRemoveManualEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

        internal BuffRemoveManualEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill) : base(by, to, time, removedDuration, buffSkill)
        {
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return false; // don't consider manual remove events
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
        }
        internal override int CompareTo(AbstractBuffEvent abe)
        {
            throw new InvalidOperationException("Manual removes can't be sorted");
        }
    }
}
