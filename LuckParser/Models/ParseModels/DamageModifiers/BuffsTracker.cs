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

        public BuffsTracker(Boon buff)
        {
            _ids = new HashSet<long>()
            {
                buff.ID
            };
        }

        public BuffsTracker(List<Boon> buffs)
        {
            _ids = new HashSet<long>(buffs.Select(x => x.ID));
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
