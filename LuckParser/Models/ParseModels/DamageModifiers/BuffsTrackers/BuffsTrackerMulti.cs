using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffsTrackerMulti : BuffsTracker
    {
        private readonly HashSet<long> _ids;

        public BuffsTrackerMulti(params Boon[] buffs)
        {
            _ids = new HashSet<long>();
            foreach (Boon buff in buffs)
            {
                _ids.Add(buff.ID);
            }
        }

        public override int GetStack(Dictionary<long, BoonsGraphModel> bgms, long time)
        {
            throw new NotImplementedException();
        }

        public override bool Has(Dictionary<long, BoonsGraphModel> bgms)
        {
            throw new NotImplementedException();
        }
    }
}
