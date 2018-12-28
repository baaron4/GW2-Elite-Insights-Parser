using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemOverstack : AbstractBoonSimulationItemWasted
    {

        public BoonSimulationItemOverstack(ushort src, long overstack, long time) : base(src, overstack, time)
        {
        }

        public override void SetBoonDistributionItem(Dictionary<long,Dictionary<ushort, BoonDistributionItem>> distribs, long start, long end, long boonid)
        {
            Dictionary<ushort, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            if (distrib.TryGetValue(Src, out var toModify))
            {
                toModify.Overstack += GetValue(start, end);
                distrib[Src] = toModify;
            }
            else
            {
                distrib.Add(Src, new BoonDistributionItem(
                    0,
                    GetValue(start, end), 0, 0, 0));
            }
        }
    }
}
