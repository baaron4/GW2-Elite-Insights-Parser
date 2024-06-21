using GW2EIEvtcParser.EIData.BuffSimulators;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffApplyEvent : AbstractBuffApplyEvent
    {
        public bool Initial { get; }
        public int OriginalAppliedDuration { get; }
        public int AppliedDuration { get; }

        public uint OverridenDuration { get; }
        internal uint OverridenDurationInternal { get; set; }
        internal uint OverridenInstance { get; set; }
        private readonly bool _addedActive;

        internal BuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, EvtcVersionEvent evtcVersion) : base(evtcItem, agentData, skillData)
        {
            Initial = evtcItem.IsStateChange == StateChange.BuffInitial;
            AppliedDuration = evtcItem.Value;
            if (Initial && evtcVersion.Build >= ArcDPSBuilds.BuffExtensionOverstackValueChanged)
            {
                OriginalAppliedDuration = evtcItem.BuffDmg;
            }
            else
            {
                OriginalAppliedDuration = AppliedDuration;
            }
            _addedActive = evtcItem.IsShields > 0;
            OverridenDuration = evtcItem.OverstackValue;
        }

        internal BuffApplyEvent(AgentItem by, AgentItem to, long time, int duration, SkillItem buffSkill, IFF iff, uint id, bool addedActive) : base(by, to, time, buffSkill, iff, id)
        {
            AppliedDuration = duration;
            OriginalAppliedDuration = duration;
            _addedActive = addedActive;
        }

        internal override void TryFindSrc(ParsedEvtcLog log)
        {
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator, bool forceStackType4ToBeActive)
        {
            simulator.Add(AppliedDuration, CreditedBy, Time, BuffInstance, _addedActive || (forceStackType4ToBeActive && simulator.Buff.StackType == BuffStackType.StackingConditionalLoss), OverridenDurationInternal > 0 ? OverridenDurationInternal : OverridenDuration, OverridenInstance);
        }

        /*internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffApplyEvent && !(abe is BuffExtensionEvent))
            {
                return 0;
            }
            return -1;
        }*/
    }
}
