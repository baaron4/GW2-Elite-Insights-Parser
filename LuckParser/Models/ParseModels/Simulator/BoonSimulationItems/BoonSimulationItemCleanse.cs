using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemCleanse
    {
        public readonly ushort ProvokedBy;
        private readonly long _duration;
        private readonly long _time;

        public BoonSimulationItemCleanse(ushort provokedBy, long duration, long time)
        {
            ProvokedBy = provokedBy;
            this._duration = duration;
            _time = time;
        }

        public long GetCleanseDuration(long start, long end)
        {
            return (start <= _time && _time <= end) ? _duration : 0;
        }
    }
}
