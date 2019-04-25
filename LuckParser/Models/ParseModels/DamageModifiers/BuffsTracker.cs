using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffsTracker
    {
        private readonly HashSet<long> _ids;

        public BuffsTracker(long buffID)
        {
            _ids = new HashSet<long>()
            {
                buffID
            };
        }

        public BuffsTracker(List<long> buffsIds)
        {
            _ids = new HashSet<long>(buffsIds);
        }

        public int GetStack(Dictionary<long, BoonsGraphModel> bgms, long time)
        {
            int stack = 0;
            foreach (long key in bgms.Keys.Intersect(_ids))
            {
                stack = Math.Max(bgms[key].GetStackCount(time), stack);
            }
            return stack;
        }

        public bool Has(Dictionary<long, BoonsGraphModel> bgms)
        {
            return bgms.Keys.Intersect(_ids).Count() > 0;
        }
    }
}
