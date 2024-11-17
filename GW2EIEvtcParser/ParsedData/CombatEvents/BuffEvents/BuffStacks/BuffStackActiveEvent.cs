using GW2EIEvtcParser.EIData.BuffSimulators;

namespace GW2EIEvtcParser.ParsedData;

public class BuffStackActiveEvent : BuffStackEvent
{

    internal BuffStackActiveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        BuffInstance = (uint)evtcItem.DstAgent;
    }
    internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
    {
        return BuffInstance != 0 && base.IsBuffSimulatorCompliant(useBuffInstanceSimulator) && (useBuffInstanceSimulator || BuffID == SkillIDs.Regeneration);
    }

    internal override void UpdateSimulator(AbstractBuffSimulator simulator, bool forceStackType4ToBeActive)
    {
        simulator.Activate(BuffInstance);
    }
}

