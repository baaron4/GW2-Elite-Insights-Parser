using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffStackActiveEvent : AbstractBuffStackEvent
    {

        internal BuffStackActiveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BuffInstance = (uint)evtcItem.DstAgent;
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Activate(BuffInstance);
        }
    }
}

