using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EIData.BuffSimulators;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffExtensionEvent : AbstractBuffApplyEvent
    {
        public long OldDuration => NewDuration - ExtendedDuration;
        public long ExtendedDuration { get; }
        public long NewDuration { get; }
        private bool _sourceFinderRan = false;

        internal BuffExtensionEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            NewDuration = evtcItem.OverstackValue;
            ExtendedDuration = evtcItem.Value;
        }

        internal override void TryFindSrc(ParsedEvtcLog log)
        {
            if (!_sourceFinderRan && By == ParserHelper._unknownAgent)
            {
                _sourceFinderRan = true;
                By = log.Buffs.TryFindSrc(To, Time, ExtendedDuration, log, BuffID, BuffInstance);
            }
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Extend(ExtendedDuration, OldDuration, CreditedBy, Time, BuffInstance);
        }

        /*internal override int CompareTo(AbstractBuffEvent abe)
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
        }*/
    }
}
