using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffRemoveSingleEvent : AbstractBuffRemoveEvent
    {
        private readonly ArcDPSEnums.IFF _iff;
        public uint BuffInstance { get; protected set; }

        private readonly bool _removedActive;
        private bool _overstackOrNaturalEnd => (_iff == ArcDPSEnums.IFF.Unknown && CreditedBy == ParserHelper._unknownAgent);
        private bool _lowValueRemove => (RemovedDuration <= ParserHelper.BuffSimulatorDelayConstant && RemovedDuration != 0);

        internal BuffRemoveSingleEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            _iff = evtcItem.IFF;
            BuffInstance = evtcItem.Pad;
            _removedActive = evtcItem.IsShields > 0;
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            if (BuffID == Buff.NoBuff || Time > fightEnd - ParserHelper.BuffSimulatorDelayConstant)
            {
                // don't take into account removal that are close to the end of the fight
                return false;
            }
            if (hasStackIDs)
            {
                return true;
            }
            // overstack or natural end removals
            // low value single stack remove that can mess up with the simulator if server delay
            return !_overstackOrNaturalEnd && !_lowValueRemove;
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(CreditedBy, RemovedDuration, 1, Time, ArcDPSEnums.BuffRemove.Single, BuffInstance);
        }
        internal override int CompareTo(AbstractBuffEvent abe)
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
        }
    }
}
