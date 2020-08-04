using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffStackActiveEvent : AbstractBuffStackEvent
    {

        public BuffStackActiveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BuffInstance = (uint)evtcItem.DstAgent;
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Activate(BuffInstance);
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff && hasStackIDs && BuffInstance != 0;
        }
        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffStackActiveEvent)
            {
                return 0;
            }
            return 1;
        }
    }
}

