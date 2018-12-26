using System;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration : BoonSimulationItem
    {
        private readonly ushort _src;
        private readonly ushort _originalSrc;

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.Start, other.BoonDuration)
        {
            _src = other.Src;
            _originalSrc = other.OriginalSrc;
        }

        public override void SetEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0), Duration);
        }

        public override int GetStack()
        {
            return 1;
        }

        public override void SetBoonDistributionItem(Dictionary<ushort, BoonDistributionItem> distrib, long start, long end)
        {
            BoonDistributionItem toModify;
            if (distrib.TryGetValue(_src, out toModify))
            {
                toModify.Value += GetClampedDuration(start, end);
                distrib[_src] = toModify;
            }
            else
            {
                distrib.Add(_src, new BoonDistributionItem(
                    GetClampedDuration(start, end),
                    0, 0, 0, 0));
            }
            if (_src != _originalSrc)
            {
                if (distrib.TryGetValue(_originalSrc, out toModify))
                {
                    toModify.Extension += GetClampedDuration(start, end);
                    distrib[_originalSrc] = toModify;
                }
                else
                {
                    distrib.Add(_originalSrc, new BoonDistributionItem(
                        0,
                        0, 0, 0, GetClampedDuration(start, end)));
                }
            }
            if (_src == 0)
            {
                if (distrib.TryGetValue(_originalSrc, out toModify))
                {
                    toModify.UnknownExtension += GetClampedDuration(start, end);
                    distrib[_originalSrc] = toModify;
                }
                else
                {
                    distrib.Add(_originalSrc, new BoonDistributionItem(
                        0,
                        0, 0, GetClampedDuration(start, end), 0));
                }
            }
        }
    }
}
