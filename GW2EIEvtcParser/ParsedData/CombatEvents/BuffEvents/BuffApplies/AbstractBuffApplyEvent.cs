using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffApplyEvent : AbstractBuffEvent
    {
        public uint BuffInstance { get; }

        internal AbstractBuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            By = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            To = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
            BuffInstance = evtcItem.Pad;
        }

        internal AbstractBuffApplyEvent(AgentItem by, AgentItem to, long time, SkillItem buffSkill, uint id) : base(buffSkill, time)
        {
            By = by;
            To = to;
            BuffInstance = id;
        }

        internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            return BuffID != SkillIDs.NoBuff;
        }
    }
}
