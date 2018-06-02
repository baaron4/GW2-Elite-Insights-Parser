using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonDistribution : Dictionary<int, Dictionary<ushort, long>>
    {
        public BoonDistribution() : base()
        {
        }

        public long getUptime(int boonid)
        {
            if (!ContainsKey(boonid))
            {
                return 0;
            }
            return this[boonid].Sum(x => x.Value);
        }

        public long getGeneration(int boonid, ushort src)
        {
            if (!ContainsKey(boonid) || src == 0)
            {
                return 0;
            }
            return this[boonid].Where( x => src == x.Key).Sum(x => x.Value);
        }
    }
}
