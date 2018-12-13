using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoonSimulationItemWasted : AbstractBoonSimulationItem
    {
        protected readonly ushort Src;
        private readonly long _waste;
        private readonly long _time;

        protected AbstractBoonSimulationItemWasted(ushort src, long waste, long time)
        {
            Src = src;
            _waste = waste;
            _time = time;
        }

        protected long GetValue(long start, long end)
        {
            return (start <= _time && _time <= end) ? _waste : 0;
        }
    }
}
