using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractBuffApplyEvent : AbstractBuffEvent
    {
        public uint BuffInstance { get; }

        public AbstractBuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            InternalBy = agentData.GetAgent(evtcItem.SrcAgent);
            To = agentData.GetAgent(evtcItem.DstAgent);
            BuffInstance = evtcItem.Pad;
        }

        public AbstractBuffApplyEvent(AgentItem by, AgentItem to, long time, SkillItem buffSkill, uint id) : base(buffSkill, time)
        {
            InternalBy = by;
            To = to;
            BuffInstance = id;
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff;
        }
    }
}
