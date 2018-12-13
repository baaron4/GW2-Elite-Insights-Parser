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

        public override void SetEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0),Duration);
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

        public override void SetBoonDistributionItem(Dictionary<ushort, BoonDistributionItem> distrib, long start, long end)
        {
            if (distrib.TryGetValue(_src, out var toModify))
            {
                toModify.Value += GetClampedDuration(start, end);
                distrib[_src] = toModify;
            }
            else
            {
                distrib.Add(_src, new BoonDistributionItem(
                    GetClampedDuration(start, end),
                    0, 0, 0));
            }
        }
    }
}
