using System;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration: BoonSimulationItem
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
            Duration = Math.Min(Math.Max(end - Start, 0),Duration);
        }

        public override int GetStack()
        {
            return 1;
        }

        public override void SetBoonDistributionItem(Dictionary<ushort, BoonDistributionItem> distrib, long start, long end)
        {
            if (distrib.TryGetValue(Src, out var toModify))
            {
                toModify.Value += GetClampedDuration(start, end);
                distrib[Src] = toModify;
            }
            else
            {
                distrib.Add(Src, new BoonDistributionItem(
                    GetClampedDuration(start, end),
                    0, 0, 0));
            }
            if (Src == 0)
            {
                if (distrib.TryGetValue(OriginalSrc, out var toModify2))
                {
                    toModify2.Extension += GetClampedDuration(start, end);
                    distrib[OriginalSrc] = toModify2;
                }
                else
                {
                    distrib.Add(OriginalSrc, new BoonDistributionItem(
                        0,
                        0, 0, GetClampedDuration(start, end)));
                }
            }
        }
    }
}
