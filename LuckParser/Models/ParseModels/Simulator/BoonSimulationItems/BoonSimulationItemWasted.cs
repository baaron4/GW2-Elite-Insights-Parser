using LuckParser.Models.DataModels;
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

        public override void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AbstractActor, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            AbstractActor actor = GeneralHelper.GetActor(Src, _applicationTime, log);
            if (distrib.TryGetValue(actor, out var toModify))
            {
                toModify.Waste += GetValue(start, end);
                distrib[actor] = toModify;
            }
            else
            {
                distrib.Add(actor, new BoonDistributionItem(
                    0,
                    0, GetValue(start, end), 0, 0, 0));
            }
        }
    }
}
