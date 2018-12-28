using System;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration : BoonSimulationItem
    {

        private readonly long _originalApplicationTime;
        private readonly long _originalSeedTime;
        private readonly ushort _src;
        private readonly ushort _seedSrc;

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.Start, other.BoonDuration)
        {
            _src = other.Src;
            _seedSrc = other.SeedSrc;
            _originalApplicationTime = other.ApplicationTime;
            _originalSeedTime = other.SeedTime;
        }

        public override void SetEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0), Duration);
        }

        public override int GetStack()
        {
            return 1;
        }

        public override void SetBoonDistributionItem(Dictionary<long, Dictionary<ushort, BoonDistributionItem>> distribs, long start, long end, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out var distrib))
            {
                distrib = new Dictionary<ushort, BoonDistributionItem>();
                distribs.Add(boonid, distrib);
            }
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
            if (_src != _seedSrc)
            {
                if (distrib.TryGetValue(_seedSrc, out toModify))
                {
                    toModify.Extension += GetClampedDuration(start, end);
                    distrib[_seedSrc] = toModify;
                }
                else
                {
                    distrib.Add(_seedSrc, new BoonDistributionItem(
                        0,
                        0, 0, 0, GetClampedDuration(start, end)));
                }
            }
            if (_src == 0)
            {
                if (distrib.TryGetValue(_seedSrc, out toModify))
                {
                    toModify.UnknownExtension += GetClampedDuration(start, end);
                    distrib[_seedSrc] = toModify;
                }
                else
                {
                    distrib.Add(_seedSrc, new BoonDistributionItem(
                        0,
                        0, 0, GetClampedDuration(start, end), 0));
                }
            }
        }
    }
}
