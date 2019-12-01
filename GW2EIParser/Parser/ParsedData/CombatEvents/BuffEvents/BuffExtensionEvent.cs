using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffExtensionEvent : BuffApplyEvent
    {
        private readonly long _oldValue;

        public BuffExtensionEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            if (By == GeneralHelper.UnknownAgent)
            {
                By = null;
            }
            _oldValue = evtcItem.OverstackValue - evtcItem.Value;
        }

        public override void TryFindSrc(ParsedLog log)
        {
            if (By == null)
            {
                By = log.Buffs.TryFindSrc(To, Time, AppliedDuration, log, BuffID);
            }
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Extend(AppliedDuration, _oldValue, By, Time, BuffInstance);
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
