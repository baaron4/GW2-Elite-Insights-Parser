using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemWasted : AbstractBoonSimulationItemWasted
    {

        private readonly long _originalTime;
        private readonly ushort _originalSrc;

        public BoonSimulationItemWasted(ushort src, long waste, long time, ushort originalSrc, long originalTime) : base(src, waste, time)
        {
            _originalSrc = originalSrc;
            _originalTime = originalTime;
        }

        public override void SetBoonDistributionItem(Dictionary<long,Dictionary<ushort, BoonDistributionItem>> distribs, long start, long end, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out var distrib))
            {
                distrib = new Dictionary<ushort, BoonDistributionItem>();
                distribs.Add(boonid, distrib);
            }
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
