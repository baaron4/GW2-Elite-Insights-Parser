using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationOverstackItem : BoonSimulationItemDuration
    {
        public BoonSimulationOverstackItem(BoonStackItem other) : base(other)
        {
            Duration = 1;
        }

        public override int GetStack(long end)
        {
            return 0;
        }

        public override long GetSrcDuration(ushort src, long start, long end)
        {
            return 0;
        }
        public override long GetItemDuration()
        {
            return Duration;
        }
        public override long GetClampedDuration(long start, long end)
        {
            return 0;
        }

        public override long GetOverstack(ushort src, long start, long end)
        {
            if (src != _src)
            {
                return 0;
            }
            if (end == 0 || (end > 0 && start <= Start && Start + Duration <= end ))
            {
                return _overstack;
            }
            return 0;
        }

        public override List<BoonsGraphModel.Segment> ToSegment()
        {
            return new List<BoonsGraphModel.Segment>();
        }
    }
}
