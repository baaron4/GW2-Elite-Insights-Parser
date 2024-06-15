using System;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffExtendCastFinder : BuffCastFinder<BuffExtensionEvent>
    {

        public BuffExtendCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
        }

        protected override AgentItem GetKeyAgent(BuffExtensionEvent evt)
        {
            return evt.To;
        }

        internal BuffExtendCastFinder UsingDurationChecker(int duration, long epsilon = ServerDelayConstant)
        {
            UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.ExtendedDuration - duration) < epsilon);
            return this;
        }
    }
}
