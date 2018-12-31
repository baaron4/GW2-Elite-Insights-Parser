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
            Dictionary<AgentItem, BoonDistributionItem > distrib = GetDistrib(distribs, boonid);
            AgentItem agent = log.AgentData.GetAgentByInstID(Src, _applicationTime);
            if (distrib.TryGetValue(agent, out var toModify))
            {
                toModify.Waste += GetValue(start, end);
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BoonDistributionItem(
                    0,
                    0, GetValue(start, end), 0, 0, 0));
            }
        }
    }
}
