using System;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration : BoonSimulationItem
    {

        private readonly long _applicationTime;
        private readonly long _seedTime;
        private readonly ushort _src;
        private readonly ushort _seedSrc;

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.Start, other.BoonDuration)
        {
            _src = other.Src;
            _seedSrc = other.SeedSrc;
            _applicationTime = other.ApplicationTime;
            _seedTime = other.SeedTime;
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
            Dictionary<ushort, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            long cDur = GetClampedDuration(start, end);
            if (distrib.TryGetValue(_src, out BoonDistributionItem toModify))
            {
                toModify.Value += cDur;
                distrib[_src] = toModify;
            }
            else
            {
                distrib.Add(_src, new BoonDistributionItem(
                    cDur,
                    0, 0, 0, 0, 0));
            }
            if (_src != _seedSrc)
            {
                if (distrib.TryGetValue(_seedSrc, out toModify))
                {
                    toModify.Extension += cDur;
                    distrib[_seedSrc] = toModify;
                }
                else
                {
                    distrib.Add(_seedSrc, new BoonDistributionItem(
                        0,
                        0, 0, 0, cDur, 0));
                }
                if (distrib.TryGetValue(_src, out toModify))
                {
                    toModify.Extended += cDur;
                    distrib[_src] = toModify;
                }
                else
                {
                    distrib.Add(_src, new BoonDistributionItem(
                        0,
                        0, 0, 0, 0, cDur));
                }
            }
            if (_src == 0)
            {
                if (distrib.TryGetValue(_seedSrc, out toModify))
                {
                    toModify.UnknownExtension += cDur;
                    distrib[_seedSrc] = toModify;
                }
                else
                {
                    distrib.Add(_seedSrc, new BoonDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
    }
}
