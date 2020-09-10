using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffsTrackerMulti : BuffsTracker
    {
        private readonly HashSet<long> _ids;

        public BuffsTrackerMulti(List<long> buffsIds)
        {
            _ids = new HashSet<long>(buffsIds);
        }

        public override int GetStack(Dictionary<long, BuffsGraphModel> bgms, long time)
        {
            int stack = 0;
            foreach (long key in bgms.Keys.Intersect(_ids))
            {
                stack += bgms[key].GetStackCount(time) > 0 ? 1 : 0;
            }
            return stack;
        }

        public override bool Has(Dictionary<long, BuffsGraphModel> bgms)
        {
            return bgms.Keys.Intersect(_ids).Any();
        }
    }
}
