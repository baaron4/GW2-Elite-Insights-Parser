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

        public override long GetDuration(ushort src, long start = 0, long end = 0)
        {
            return 0;
        }
        public override long GetSourcelessDuration()
        {
            return Duration;
        }
        public override long GetItemDuration(long start = 0, long end = 0)
        {
            return 0;
        }

        public override long GetOverstack(ushort src, long start = 0, long end = 0)
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
