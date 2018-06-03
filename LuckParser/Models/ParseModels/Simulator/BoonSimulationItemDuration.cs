using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration: BoonSimulationItem
    {
        private ushort src;

        public BoonSimulationItemDuration(long start, long duration, ushort src) : base(start, duration)
        {
            this.src = src;
        }

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.start, other.boon_duration)
        {
            this.src = other.src;
        }

        public override long getDuration(ushort src)
        {
            return this.duration;
        }
        public override void setEnd(long end)
        {
            this.duration = Math.Min(Math.Max(end - this.start, 0),duration);
        }

        public override List<ushort> getSrc()
        {
            List<ushort> res = new List<ushort>
            {
                src
            };
            return res;
        }

        public override int getStack(long end)
        {
            return 1;
        }
    }
}
