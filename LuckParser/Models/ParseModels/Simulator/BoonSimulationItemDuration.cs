using System;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration: BoonSimulationItem
    {
        protected readonly ushort _src;

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.Start, other.BoonDuration)
        {
            _src = other.Src;
        }

        public override long GetSrcDuration(ushort src, long start, long end)
        {
            if (src != _src)
            {
                return 0;
            }
            return GetClampedDuration(start, end);
        }
        public override long GetItemDuration()
        {
            return Duration;
        }
        public override void SetEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0),Duration);
        }

        public override List<ushort> GetSrc()
        {
            List<ushort> res = new List<ushort>
            {
                _src
            };
            return res;
        }

        public override int GetStack(long end)
        {
            return 1;
        }
        
        public override List<BoonsGraphModel.Segment> ToSegment()
        {
            return new List<BoonsGraphModel.Segment>
            {
                new BoonsGraphModel.Segment(Start,End,1)
            };
        }
    }
}
