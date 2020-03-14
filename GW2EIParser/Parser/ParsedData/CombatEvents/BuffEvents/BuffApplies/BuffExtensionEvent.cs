using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffExtensionEvent : AbstractBuffApplyEvent
    {
        private readonly long _oldValue;
        private readonly long _durationChange;

        public BuffExtensionEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            if (InternalBy == GeneralHelper.UnknownAgent)
            {
                InternalBy = null;
            }
            _oldValue = evtcItem.OverstackValue - evtcItem.Value;
            _durationChange = evtcItem.Value;
        }

        public override void TryFindSrc(ParsedLog log)
        {
            if (InternalBy == null)
            {
                InternalBy = log.Buffs.TryFindSrc(To, Time, _durationChange, log, BuffID);
            }
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Extend(_durationChange, _oldValue, By, Time, BuffInstance);
        }

        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffExtensionEvent)
            {
                return 0;
            }
            if (abe is BuffApplyEvent)
            {
                return 1;
            }
            return -1;
        }
    }
}
