using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemOverstack
    {
        public readonly ushort Src;
        private readonly long _overstack;
        private readonly long _time;

        public BoonSimulationItemOverstack(ushort src, long overstack, long time)
        {
            Src = src;
            _overstack = overstack;
            _time = time;
        }

        public long GetOverstack(long start, long end)
        {
            return (start <= _time && _time <= end) ? _overstack : 0;
        }
    }
}
