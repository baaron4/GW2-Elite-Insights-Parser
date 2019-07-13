using System.Collections.Generic;

namespace LuckParser.EIData
{
    public abstract class BuffsTracker
    {
        public abstract int GetStack(Dictionary<long, BoonsGraphModel> bgms, long time);
        public abstract bool Has(Dictionary<long, BoonsGraphModel> bgms);
    }
}
