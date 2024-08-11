using GW2EIEvtcParser.EIData.BuffSimulators;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffStackResetEvent : AbstractBuffStackEvent
    {
        public int ResetToDuration { get; }
        internal BuffStackResetEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BuffInstance = evtcItem.Pad;
            ResetToDuration = evtcItem.Value;
        }

        internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            return useBuffInstanceSimulator && BuffInstance != 0 && base.IsBuffSimulatorCompliant(useBuffInstanceSimulator);
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator, bool forceStackType4ToBeActive)
        {
            simulator.Reset(BuffInstance, ResetToDuration);
        }
    }
}

