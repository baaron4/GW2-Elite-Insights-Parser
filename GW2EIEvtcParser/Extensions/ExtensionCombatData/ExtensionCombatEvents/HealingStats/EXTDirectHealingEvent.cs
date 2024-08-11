using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTDirectHealingEvent : EXTAbstractHealingEvent
    {

        internal EXTDirectHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            HealingDone = -evtcItem.Value;
            AgainstDowned = AgainstDowned || (evtcItem.IsOffcycle & ~(byte)HealingExtensionOffcycleBits.PeerMask) == 1;
        }
    }
}
