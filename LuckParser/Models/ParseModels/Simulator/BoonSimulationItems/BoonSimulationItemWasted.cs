using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemWasted : AbstractBoonSimulationItemWasted
    {

        private readonly long _applicationTime;

        public BoonSimulationItemWasted(ushort src, long waste, long time, long applicationTime) : base(src, waste, time)
        {
            _applicationTime = applicationTime;
        }

        public override void SetBoonDistributionItem(Dictionary<long,Dictionary<ushort, BoonDistributionItem>> distribs, long start, long end, long boonid)
        {
            Dictionary<ushort, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            if (distrib.TryGetValue(Src, out var toModify))
            {
                toModify.Waste += GetValue(start, end);
                distrib[Src] = toModify;
            }
            else
            {
                distrib.Add(Src, new BoonDistributionItem(
                    0,
                    0, GetValue(start, end), 0, 0, 0));
            }
        }
    }
}
