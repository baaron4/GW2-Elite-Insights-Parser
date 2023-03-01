using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class BuffsTracker
    {
        public abstract int GetStack(IReadOnlyDictionary<long, BuffsGraphModel> bgms, long time);
        public abstract bool Has(IReadOnlyDictionary<long, BuffsGraphModel> bgms);
    }
}
