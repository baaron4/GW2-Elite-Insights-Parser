using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffRemoveAllEvent : AbstractBuffRemoveEvent
    {
        public const int FullRemoval = int.MaxValue;

        public int RemovedStacks { get; }
        private readonly int _lastRemovedDuration;

        public BuffRemoveAllEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            _lastRemovedDuration = evtcItem.BuffDmg;
            RemovedStacks = evtcItem.Result;
        }

        public BuffRemoveAllEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, int removedStacks, int lastRemovedDuration) : base(by, to, time, removedDuration, buffSkill)
        {
            _lastRemovedDuration = lastRemovedDuration;
            RemovedStacks = removedStacks;
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd)
        {
            return BuffID != ProfHelper.NoBuff &&
                !(RemovedDuration <= 50 && RemovedDuration != 0 && _lastRemovedDuration <= 50 && _lastRemovedDuration != 0) && // low value all stack remove that can mess up with the simulator if server delay
                 Time <= fightEnd - 50; // don't take into account removal that are close to the end of the fight
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(By, RemovedDuration, RemovedStacks, Time, ParseEnum.BuffRemove.All, 0);
        }

        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffRemoveAllEvent)
            {
                return 0;
            }
            return 1;
        }
    }
}
