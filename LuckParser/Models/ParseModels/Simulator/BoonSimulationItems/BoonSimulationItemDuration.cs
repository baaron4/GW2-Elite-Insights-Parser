using System;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration : BoonSimulationItem
    {
        protected readonly ushort Src;
        protected readonly ushort OriginalSrc;

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.Start, other.BoonDuration)
        {
            Src = other.Src;
            OriginalSrc = other.OriginalSrc;
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
            if (distrib.TryGetValue(Src, out toModify))
            {
                toModify.Value += GetClampedDuration(start, end);
                distrib[Src] = toModify;
            }
            else
            {
                distrib.Add(Src, new BoonDistributionItem(
                    GetClampedDuration(start, end),
                    0, 0, 0, 0));
            }
            if (Src != OriginalSrc)
            {
                if (distrib.TryGetValue(OriginalSrc, out toModify))
                {
                    toModify.Extension += GetClampedDuration(start, end);
                    distrib[OriginalSrc] = toModify;
                }
                else
                {
                    distrib.Add(OriginalSrc, new BoonDistributionItem(
                        0,
                        0, 0, 0, GetClampedDuration(start, end)));
                }
            }
            if (Src == 0)
            {
                if (distrib.TryGetValue(OriginalSrc, out toModify))
                {
                    toModify.UnknownExtension += GetClampedDuration(start, end);
                    distrib[OriginalSrc] = toModify;
                }
                else
                {
                    distrib.Add(OriginalSrc, new BoonDistributionItem(
                        0,
                        0, 0, GetClampedDuration(start, end), 0));
                }
            }
        }
    }
}
