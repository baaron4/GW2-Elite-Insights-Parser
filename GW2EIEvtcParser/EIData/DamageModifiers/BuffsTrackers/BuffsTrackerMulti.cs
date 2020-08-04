using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class BuffsTrackerMulti : BuffsTracker
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
                stack = Math.Max(bgms[key].GetStackCount(time), stack);
            }
            return stack;
        }

        public override bool Has(Dictionary<long, BuffsGraphModel> bgms)
        {
            return bgms.Keys.Intersect(_ids).Any();
        }
    }
}
