using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class BuffsTracker
    {

        public abstract bool Has(Dictionary<long, BoonsGraphModel> bgms);

        public abstract int GetStack(Dictionary<long, BoonsGraphModel> bgms, long time);
    }
}
