using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemExtension : AbstractBoonSimulationItem
    {
        private readonly long _duration;
        private readonly long _time;
        private readonly ushort _src;

        public BoonSimulationItemExtension(long duration, long time, ushort src)
        {
            _duration = duration;
            _time = time;
            _src = src;
        }

        private long GetClampedExtension(long start, long end)
        {
            if (end > 0 && end - start > 0)
            {
                long startoffset = Math.Max(Math.Min(_duration, start - _time), 0);
                long itemEnd = _time + _duration;
                long endOffset = Math.Max(Math.Min(_duration, itemEnd - end), 0);
                return _duration - startoffset - endOffset;
            }
            return 0;
        }

        public override void SetBoonDistributionItem(Dictionary<ushort, BoonDistributionItem> distrib, long start, long end)
        {
            if (distrib.TryGetValue(_src, out var toModify))
            {
                toModify.UnknownExtension += GetClampedExtension(start, end);
                distrib[_src] = toModify;
            }
            else
            {
                distrib.Add(_src, new BoonDistributionItem(
                    0,
                    0, 0, GetClampedExtension(start, end)));
            }
        }
    }
}
