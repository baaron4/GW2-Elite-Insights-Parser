using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinderByDstFromMinion : EffectCastFinderByDst
    {

        protected override AgentItem GetAgent(EffectEvent effectEvent)
        {
            return effectEvent.Dst.GetFinalMaster();
        }

        public EffectCastFinderByDstFromMinion(long skillID, string effectGUID) : base(skillID, effectGUID)
        {
        }
    }
}
