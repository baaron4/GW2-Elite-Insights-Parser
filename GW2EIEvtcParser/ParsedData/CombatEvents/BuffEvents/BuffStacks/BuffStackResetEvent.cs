using GW2EIEvtcParser.EIData;
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

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Reset(BuffInstance, ResetToDuration);
        }
    }
}

