using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinderByDst : EffectCastFinder
    {
        protected override AgentItem GetKeyAgent(EffectEvent effectEvent)
        {
            return effectEvent.Dst;
        }

        public EffectCastFinderByDst(long skillID, string effectGUID) : base(skillID, effectGUID)
        {
        }
    }
}
