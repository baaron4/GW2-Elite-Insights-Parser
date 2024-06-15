using System;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffGiveCastFinder : BuffCastFinder<BuffApplyEvent>
    {
        public override BuffCastFinder<BuffApplyEvent> WithMinions(bool minions)
        {
            throw new InvalidOperationException("BuffGiveCastFinder is always with minions");
        }
        public BuffGiveCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
            Minions = true;
            UsingChecker((bae, combatData, agentData, skillData) => !bae.Initial);
        }
        protected override AgentItem GetKeyAgent(BuffApplyEvent evt)
        {
            return evt.By;
        }
    }
}
