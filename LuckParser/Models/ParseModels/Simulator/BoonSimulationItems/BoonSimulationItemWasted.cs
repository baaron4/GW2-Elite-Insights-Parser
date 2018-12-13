using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemWasted : AbstractBoonSimulationItemWasted
    {

        public BoonSimulationItemWasted(ushort src, long waste, long time) : base(src, waste, time)
        {
        }

        public override void SetBoonDistributionItem(Dictionary<ushort, BoonDistributionItem> distrib)
        {
            throw new NotImplementedException();
        }
    }
}
