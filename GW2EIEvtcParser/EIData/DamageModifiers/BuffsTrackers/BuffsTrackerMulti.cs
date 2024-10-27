using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData;

internal class BuffsTrackerMulti(HashSet<long> buffsIds) : BuffsTracker
{
    private readonly HashSet<long> _ids = buffsIds;

    public override int GetStack(IReadOnlyDictionary<long, BuffsGraphModel> bgms, long time)
    {
        int stack = 0;
        foreach (long key in bgms.Keys.Intersect(_ids))
        {
            stack += bgms[key].GetStackCount(time) > 0 ? 1 : 0;
        }
        return stack;
    }

    public override bool Has(IReadOnlyDictionary<long, BuffsGraphModel> bgms)
    {
        return bgms.Keys.Intersect(_ids).Any();
    }
}
