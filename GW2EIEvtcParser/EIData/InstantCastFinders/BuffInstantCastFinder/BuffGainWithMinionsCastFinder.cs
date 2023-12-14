using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffGainWithMinionsCastFinder : BuffGainCastFinder
    {

        protected override AgentItem GetCasterAgent(AgentItem agent)
        {
            return agent.GetFinalMaster();
        }

        public BuffGainWithMinionsCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
        }
    }
}
