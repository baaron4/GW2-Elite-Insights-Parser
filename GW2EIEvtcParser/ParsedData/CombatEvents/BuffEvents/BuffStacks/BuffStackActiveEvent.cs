using GW2EIEvtcParser.EIData.BuffSimulators;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffStackActiveEvent : AbstractBuffStackEvent
    {

        internal BuffStackActiveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BuffInstance = (uint)evtcItem.DstAgent;
        }
        internal override bool IsBuffSimulatorCompliant(bool hasStackIDs)
        {
            return BuffInstance != 0 && base.IsBuffSimulatorCompliant(hasStackIDs);
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator, bool forceStackType4ToBeActive)
        {
            simulator.Activate(BuffInstance);
        }
    }
}

