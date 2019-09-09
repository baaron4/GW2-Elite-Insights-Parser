using LuckParser.EIData;

namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class BuffRemoveSingleEvent : AbstractBuffRemoveEvent
    {
        private readonly ParseEnum.IFF _iff;
        public BuffRemoveSingleEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            _iff = evtcItem.IFF;
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd)
        {
            return BuffID != ProfHelper.NoBuff &&
                !(_iff == ParseEnum.IFF.Unknown && By == GeneralHelper.UnknownAgent) && // weird single stack remove
                !(RemovedDuration <= 50 && RemovedDuration != 0) &&// low value single stack remove that can mess up with the simulator if server delay
                 Time <= fightEnd - 50; // don't take into account removal that are close to the end of the fight
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Remove(RemovedDuration, Time, ParseEnum.BuffRemove.Single);
        }
    }
}
