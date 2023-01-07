using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffStackEvent : AbstractBuffEvent
    {
        protected uint BuffInstance { get; set; }

        internal AbstractBuffStackEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            To = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            By = ParserHelper._unknownAgent;
        }

        internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            return useBuffInstanceSimulator && BuffInstance != 0;
        }

        internal override void TryFindSrc(ParsedEvtcLog log)
        {
        }

        /*internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is AbstractBuffApplyEvent)
            {
                return 1;
            }
            if (abe is AbstractBuffStackEvent)
            {
                return 0;
            }
            return -1;
        }*/
    }
}

