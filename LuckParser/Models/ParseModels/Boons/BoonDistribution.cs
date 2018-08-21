using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public struct OverAndValue
    {
        public long value;
        public long overstack;

        public OverAndValue(long value, long overstack)
        {
            this.value = value;
            this.overstack = overstack;
        }
    }

    public class BoonDistribution : Dictionary<long, Dictionary<ushort, OverAndValue>>
    {
        public BoonDistribution() : base()
        {
        }

        public long GetUptime(long boonid)
        {
            if (!ContainsKey(boonid))
            {
                return 0;
            }
            return this[boonid].Sum(x => x.Value.value);
        }

        public long GetGeneration(long boonid, ushort src)
        {
            if (!ContainsKey(boonid) || src == 0)
            {
                return 0;
            }
            return this[boonid].Where( x => src == x.Key).Sum(x => x.Value.value);
        }

        public long GetOverstack(long boonid, ushort src)
        {
            if (!ContainsKey(boonid) || src == 0)
            {
                return 0;
            }
            return this[boonid].Where(x => src == x.Key).Sum(x => x.Value.overstack);
        }
    }
}
