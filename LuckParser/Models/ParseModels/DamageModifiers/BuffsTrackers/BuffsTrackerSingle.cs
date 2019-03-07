using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffsTrackerSingle : BuffsTracker
    {
        private readonly long _id;

        public BuffsTrackerSingle(Boon buff)
        {
            _id = buff.ID;
        }

        public override int GetStack(Dictionary<long, BoonsGraphModel> bgms, long time)
        {
            return bgms[_id].GetStackCount(time);
        }

        public override bool Has(Dictionary<long, BoonsGraphModel> bgms)
        {
            return bgms.ContainsKey(_id);
        }
    }
}
