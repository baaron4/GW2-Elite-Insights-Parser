using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffStackResetEvent : AbstractBuffStackEvent
    {
        private readonly int _resetToDuration;
        internal BuffStackResetEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BuffInstance = evtcItem.Pad;
            _resetToDuration = evtcItem.Value;
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Reset(BuffInstance, _resetToDuration);
        }
    }
}

