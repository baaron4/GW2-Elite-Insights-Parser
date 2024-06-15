using System;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffGainCastFinder : BuffCastFinder<BuffApplyEvent>
    {

        public BuffGainCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
            UsingChecker((bae, combatData, agentData, skillData) => !bae.Initial);
        }
        protected override AgentItem GetKeyAgent(BuffApplyEvent evt)
        {
            return evt.To;
        }

        internal BuffGainCastFinder UsingDurationChecker(int duration, long epsilon = ServerDelayConstant)
        {
            UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - duration) < epsilon);
            return this;
        }
    }
}
