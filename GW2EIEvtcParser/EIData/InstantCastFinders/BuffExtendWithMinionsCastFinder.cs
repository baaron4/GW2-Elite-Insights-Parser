using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffExtendWithMinionsCastFinder : BuffExtendCastFinder
    {

        protected override AgentItem GetCasterAgent(AgentItem agent)
        {
            return agent.GetFinalMaster();
        }

        public BuffExtendWithMinionsCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
        }
    }
}
