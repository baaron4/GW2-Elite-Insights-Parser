using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinderByDst : EffectCastFinder
    {
        protected override Dictionary<AgentItem, List<EffectEvent>> GetEffectEventDict(EffectGUIDEvent effectGUIDEvent, CombatData combatData)
        {
            return combatData.GetEffectEventsByEffectID(effectGUIDEvent.ContentID).Where(x => x.IsAroundDst).GroupBy(x => x.Dst).ToDictionary(x => x.Key, x => x.ToList());
        }

        protected override AgentItem GetAgent(EffectEvent effectEvent)
        {
            return effectEvent.Dst;
        }

        public EffectCastFinderByDst(long skillID, string effectGUID) : base(skillID, effectGUID)
        {
        }
    }
}
