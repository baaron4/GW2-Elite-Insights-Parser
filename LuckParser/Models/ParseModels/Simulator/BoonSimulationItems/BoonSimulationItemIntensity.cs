using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemIntensity : BoonSimulationItem
    {
        private List<BoonSimulationItemDuration> _stacks = new List<BoonSimulationItemDuration>();

        public BoonSimulationItemIntensity(List<BoonStackItem> stacks) : base()
        {
            Start = stacks[0].Start;
            foreach (BoonStackItem stack in stacks)
            {
                _stacks.Add(new BoonSimulationItemDuration(stack));
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override void SetEnd(long end)
        {
            foreach (BoonSimulationItemDuration stack in _stacks)
            {
                stack.SetEnd(end);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override int GetStack(long end)
        {
            return _stacks.Count(x => x.End >= end);
        }

        public override List<BoonsGraphModel.Segment> ToSegment()
        {
            if (Duration == _stacks.Min(x => x.Duration))
            {
                return new List<BoonsGraphModel.Segment>
                {
                    new BoonsGraphModel.Segment(Start,End,_stacks.Count)
                };
            }
            long start = Start;
            List<BoonsGraphModel.Segment> res = new List<BoonsGraphModel.Segment>();
            foreach ( long end in _stacks.Select(x => x.Duration + Start).Distinct())
            {
                res.Add(new BoonsGraphModel.Segment(start,end,GetStack(end)));
                start = end;
            }
            return res;
        }

        public override void SetBoonDistributionItem(Dictionary<ushort, BoonDistributionItem> distrib, long start, long end)
        {
            foreach (BoonSimulationItemDuration item in _stacks)
            {
                item.SetBoonDistributionItem(distrib, start, end);
            }
        }
    }
}
