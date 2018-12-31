using LuckParser.Models.DataModels;
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

        public override void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AbstractActor, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            AbstractActor actor = GeneralHelper.GetActor(Src, Time, log);
            if (distrib.TryGetValue(actor, out var toModify))
            {
                toModify.Overstack += GetValue(start, end);
                distrib[actor] = toModify;
            }
            else
            {
                distrib.Add(actor, new BoonDistributionItem(
                    0,
                    GetValue(start, end), 0, 0, 0, 0));
            }
        }
    }
}
