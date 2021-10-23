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

        public BuffGainWithMinionsCastFinder(long skillID, long buffID, long icd, BuffGainCastChecker checker = null) : base(skillID, buffID, icd, checker)
        {
        }

        public BuffGainWithMinionsCastFinder(long skillID, long buffID, long icd, ulong minBuild, ulong maxBuild, BuffGainCastChecker checker = null) : base(skillID, buffID, icd, minBuild, maxBuild, checker)
        {
        }
    }
}
