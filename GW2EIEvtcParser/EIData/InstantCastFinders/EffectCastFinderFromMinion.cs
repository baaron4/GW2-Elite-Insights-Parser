using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinderFromMinion : EffectCastFinder
    {

        protected override AgentItem GetAgent(EffectEvent effectEvent)
        {
            return effectEvent.Src.GetFinalMaster();
        }

        public EffectCastFinderFromMinion(long skillID, string effectGUID) : base(skillID, effectGUID)
        {
        }
    }
}
