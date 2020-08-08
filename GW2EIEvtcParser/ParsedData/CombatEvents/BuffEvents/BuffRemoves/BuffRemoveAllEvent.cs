using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffRemoveAllEvent : AbstractBuffRemoveEvent
    {
        public const int FullRemoval = int.MaxValue;

        public int RemovedStacks { get; }
        private readonly int _lastRemovedDuration;

        internal BuffRemoveAllEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            _lastRemovedDuration = evtcItem.BuffDmg;
            RemovedStacks = evtcItem.Result;
        }

        internal BuffRemoveAllEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, int removedStacks, int lastRemovedDuration) : base(by, to, time, removedDuration, buffSkill)
        {
            _lastRemovedDuration = lastRemovedDuration;
            RemovedStacks = removedStacks;
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff &&
                    (hasStackIDs ||
                        (!(RemovedDuration <= 50 && RemovedDuration != 0 && _lastRemovedDuration <= 50 && _lastRemovedDuration != 0) && // low value all stack remove that can mess up with the simulator if server delay));
                        Time <= fightEnd - 50)); // don't take into account removal that are close to the end of the fight
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(By, RemovedDuration, RemovedStacks, Time, ArcDPSEnums.BuffRemove.All, 0);
        }

        internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is AbstractBuffStackEvent)
            {
                return -1;
            }
            if (abe is BuffRemoveAllEvent)
            {
                return 0;
            }
            return 1;
        }
    }
}
