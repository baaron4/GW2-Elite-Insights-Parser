using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoonSimulationItemWasted : AbstractBoonSimulationItem
    {
        protected readonly AgentItem Src;
        private readonly long _waste;
        protected readonly long Time;

        protected AbstractBoonSimulationItemWasted(AgentItem src, long waste, long time)
        {
            Src = src;
            _waste = waste;
            Time = time;
        }

        protected long GetValue(long start, long end)
        {
            return (start <= Time && Time <= end) ? _waste : 0;
        }
    }
}
