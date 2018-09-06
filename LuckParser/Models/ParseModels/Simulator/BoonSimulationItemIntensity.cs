using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    class BoonSimulationItemIntensity : BoonSimulationItem
    {
        private List<BoonSimulationItemDuration> _stacks = new List<BoonSimulationItemDuration>();

        public BoonSimulationItemIntensity(List<BoonStackItem> stacks) : base()
        {
            Start = stacks[0].Start;
            foreach (BoonStackItem stack in stacks)
            {
                _stacks.Add(new BoonSimulationItemDuration(stack));
            }
            Duration = _stacks.Max(x => x.GetTotalDuration());
        }

        public override void SetEnd(long end)
        {
            foreach (BoonSimulationItemDuration stack in _stacks)
            {
                stack.SetEnd(end);
            }
            Duration = _stacks.Max(x => x.GetTotalDuration());
        }

        public override long GetSrcDuration(ushort src, long start, long end)
        {
            long total = 0;
            foreach (BoonSimulationItemDuration stack in _stacks.Where(x => x.GetSrc()[0] == src))
            {
                total += stack.GetSrcDuration(src, start, end);
            }
            return total;
        }
        public override long GetTotalDuration()
        {
            long total = 0;
            foreach (BoonSimulationItemDuration stack in _stacks)
            {
                total += stack.GetTotalDuration();
            }
            return total;
        }

        public override List<ushort> GetSrc()
        {
            return _stacks.Select(x => x.GetSrc()[0]).Distinct().ToList();
        }

        public override int GetStack(long end)
        {
            return _stacks.Count(x => x.End >= end);
        }

        public override List<BoonsGraphModel.Segment> ToSegment()
        {
            if (Duration == _stacks.Min(x => x.GetTotalDuration()))
            {
                return new List<BoonsGraphModel.Segment>
                {
                    new BoonsGraphModel.Segment(Start,End,_stacks.Count)
                };
            }
            long start = Start;
            List<BoonsGraphModel.Segment> res = new List<BoonsGraphModel.Segment>();
            foreach ( long end in _stacks.Select(x => x.GetTotalDuration() + Start).Distinct())
            {
                res.Add(new BoonsGraphModel.Segment(start,end,GetStack(end)));
                start = end;
            }
            return res;
        }
    }
}
