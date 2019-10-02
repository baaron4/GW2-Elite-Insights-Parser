using System.Collections.Generic;

namespace LuckParser.EIData
{
    public abstract class BuffsTracker
    {
        public abstract int GetStack(Dictionary<long, BuffsGraphModel> bgms, long time);
        public abstract bool Has(Dictionary<long, BuffsGraphModel> bgms);
    }
}
