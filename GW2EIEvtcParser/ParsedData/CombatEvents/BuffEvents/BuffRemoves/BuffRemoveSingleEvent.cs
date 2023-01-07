using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EIData.BuffSimulators;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffRemoveSingleEvent : AbstractBuffRemoveEvent
    {
        private readonly ArcDPSEnums.IFF _iff;
        public uint BuffInstance { get; protected set; }

        private readonly bool _removedActive;
        private readonly bool _byShouldntBeUnknown;
        private bool _overstackOrNaturalEnd => (_iff == ArcDPSEnums.IFF.Unknown && CreditedBy == ParserHelper._unknownAgent && !_byShouldntBeUnknown);

        internal BuffRemoveSingleEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            _iff = evtcItem.IFF;
            // Sometimes there is a dstAgent value but the agent itself is not in the pool, such cases should not trigger _overstackOrNaturalEnd
            _byShouldntBeUnknown = evtcItem.DstAgent != 0;
            BuffInstance = evtcItem.Pad;
            _removedActive = evtcItem.IsShields > 0;
        }

        internal BuffRemoveSingleEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, bool removedActive, uint stackID) : base(by, to, time, removedDuration, buffSkill)
        {
            BuffInstance = stackID;
            _removedActive = removedActive;
        }

        internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            if (BuffID == SkillIDs.NoBuff)
            {
                return false;
            }
            if (useBuffInstanceSimulator)
            {
                return true;
            }
            // overstack or natural end removals
            return !_overstackOrNaturalEnd;
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(CreditedBy, RemovedDuration, 1, Time, ArcDPSEnums.BuffRemove.Single, BuffInstance);
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
