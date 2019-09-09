using LuckParser.EIData;

namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class BuffRemoveAllEvent : AbstractBuffRemoveEvent
    {
        private readonly int _removedStacks;
        private readonly int _lastRemovedDuration;

        public BuffRemoveAllEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            _lastRemovedDuration = evtcItem.BuffDmg;
            _removedStacks = evtcItem.Result;
        }

        public BuffRemoveAllEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, int removedStacks, int lastRemovedDuration) : base(by, to, time, removedDuration, buffSkill)
        {
            _lastRemovedDuration = lastRemovedDuration;
            _removedStacks = removedStacks;
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd)
        {
            return BuffID != ProfHelper.NoBuff &&
                !(RemovedDuration <= 50 && RemovedDuration != 0 && _lastRemovedDuration <= 50 && _lastRemovedDuration != 0) && // low value all stack remove that can mess up with the simulator if server delay
                 Time <= fightEnd - 50; // don't take into account removal that are close to the end of the fight
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Remove(RemovedDuration, Time, ParseEnum.BuffRemove.All);
        }
    }
}
