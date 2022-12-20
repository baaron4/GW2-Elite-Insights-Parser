using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EIData.BuffSimulators;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffApplyEvent : AbstractBuffApplyEvent
    {
        public bool Initial { get; }
        public int AppliedDuration { get; }

        public uint OverstackDuration { get; }
        private readonly bool _addedActive;

        internal BuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Initial = evtcItem.IsStateChange == ArcDPSEnums.StateChange.BuffInitial;
            AppliedDuration = evtcItem.Value;
            _addedActive = evtcItem.IsShields > 0;
            OverstackDuration = evtcItem.OverstackValue;
        }

        internal BuffApplyEvent(AgentItem by, AgentItem to, long time, int duration, SkillItem buffSkill, uint id, bool addedActive) : base(by, to, time, buffSkill, id)
        {
            AppliedDuration = duration;
            _addedActive = addedActive;
        }

        internal override void TryFindSrc(ParsedEvtcLog log)
        {
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Add(AppliedDuration, CreditedBy, Time, BuffInstance, _addedActive || simulator.Buff.StackType == ArcDPSEnums.BuffStackType.StackingConditionalLoss, OverstackDuration);
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
