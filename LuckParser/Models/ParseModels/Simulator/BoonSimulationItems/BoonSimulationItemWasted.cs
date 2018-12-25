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

        public override void SetBoonDistributionItem(Dictionary<ushort, BoonDistributionItem> distrib, long start, long end)
        {
            if (distrib.TryGetValue(Src, out var toModify))
            {
                toModify.Waste += GetValue(start, end);
                distrib[Src] = toModify;
            }
            else
            {
                distrib.Add(Src, new BoonDistributionItem(
                    0,
                    0, GetValue(start, end), 0, 0));
            }
        }
    }
}
