using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EIData.BuffSimulators;
using static GW2EIEvtcParser.ArcDPSEnums;

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

        internal BuffRemoveAllEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, IFF iff, int removedStacks, int lastRemovedDuration) : base(by, to, time, removedDuration, buffSkill, iff)
        {
            _lastRemovedDuration = lastRemovedDuration;
            RemovedStacks = removedStacks;
        }
        internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            return BuffID != SkillIDs.NoBuff;
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(CreditedBy, RemovedDuration, RemovedStacks, Time, BuffRemove.All, 0);
        }

        /*internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffRemoveAllEvent)
            {
                return 0;
            }
            return 1;
        }*/
    }
}
