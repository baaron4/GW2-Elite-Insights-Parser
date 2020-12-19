using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffApplyEvent : AbstractBuffApplyEvent
    {
        public bool Initial { get; }
        public int AppliedDuration { get; }

        private readonly uint _overstackDuration;
        private readonly bool _addedActive;

        internal BuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Initial = evtcItem.IsStateChange == ArcDPSEnums.StateChange.BuffInitial;
            AppliedDuration = evtcItem.Value;
            _addedActive = evtcItem.IsShields > 0;
            _overstackDuration = evtcItem.OverstackValue;
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
            simulator.Add(AppliedDuration, By, Time, BuffInstance, _addedActive, _overstackDuration);
        }

        internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffApplyEvent && !(abe is BuffExtensionEvent))
            {
                return 0;
            }
            return -1;
        }
    }
}
