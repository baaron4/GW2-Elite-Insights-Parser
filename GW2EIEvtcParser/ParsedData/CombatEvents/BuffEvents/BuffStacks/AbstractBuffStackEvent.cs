using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffStackEvent : AbstractBuffEvent
    {
        protected uint BuffInstance { get; set; }

        internal AbstractBuffStackEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            To = agentData.GetAgent(evtcItem.SrcAgent);
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != Buff.NoBuff && hasStackIDs && BuffInstance != 0;
        }

        internal override void TryFindSrc(ParsedEvtcLog log)
        {
        }
    }
}

