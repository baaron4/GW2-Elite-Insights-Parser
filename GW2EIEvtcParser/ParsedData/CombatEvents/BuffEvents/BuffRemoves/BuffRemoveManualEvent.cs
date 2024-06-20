using GW2EIEvtcParser.EIData.BuffSimulators;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffRemoveManualEvent : AbstractBuffRemoveEvent
    {
        internal BuffRemoveManualEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

        internal BuffRemoveManualEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, IFF iff) : base(by, to, time, removedDuration, buffSkill, iff)
        {
        }

        internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            return false; // don't consider manual remove events
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator, bool forceStackType4ToBeActive)
        {
        }
        /*internal override int CompareTo(AbstractBuffEvent abe)
        {
            throw new InvalidOperationException("Manual removes can't be sorted");
        }*/
    }
}
