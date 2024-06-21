using GW2EIEvtcParser.EIData.BuffSimulators;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffRemoveSingleEvent : AbstractBuffRemoveEvent
    {
        public uint BuffInstance { get; protected set; }

        private readonly bool _byShouldntBeUnknown;
        internal bool OverstackOrNaturalEnd => (IFF == IFF.Unknown && CreditedBy == ParserHelper._unknownAgent && !_byShouldntBeUnknown);

        internal BuffRemoveSingleEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            // Sometimes there is a dstAgent value but the agent itself is not in the pool, such cases should not trigger _overstackOrNaturalEnd
            _byShouldntBeUnknown = evtcItem.DstAgent != 0;
            BuffInstance = evtcItem.Pad;
        }

        internal BuffRemoveSingleEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, IFF iff, uint stackID) : base(by, to, time, removedDuration, buffSkill, iff)
        {
            BuffInstance = stackID;
            _byShouldntBeUnknown = true;
        }

        internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            if (!base.IsBuffSimulatorCompliant(useBuffInstanceSimulator))
            {
                return false;
            }
            if (useBuffInstanceSimulator)
            {
                return true;
            }
            // overstack or natural end removals
            return !OverstackOrNaturalEnd;
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator, bool forceStackType4ToBeActive)
        {
            simulator.Remove(CreditedBy, RemovedDuration, 1, Time, BuffRemove.Single, BuffInstance);
        }
        /*internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffRemoveSingleEvent)
            {
                return 0;
            }
            if (abe is BuffRemoveAllEvent)
            {
                return -1;
            }
            return 1;
        }*/
    }
}
