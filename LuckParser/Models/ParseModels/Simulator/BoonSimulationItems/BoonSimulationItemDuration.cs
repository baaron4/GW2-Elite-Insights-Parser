using System;
using LuckParser.Models.DataModels;
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

        public override void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AbstractActor, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            long cDur = GetClampedDuration(start, end);
            AbstractActor actor = GeneralHelper.GetActor(_src, _applicationTime, log);
            AbstractActor seedActor = GeneralHelper.GetActor(_seedSrc, _seedTime, log);
            if (distrib.TryGetValue(actor, out BoonDistributionItem toModify))
            {
                toModify.Value += cDur;
                distrib[actor] = toModify;
            }
            else
            {
                distrib.Add(actor, new BoonDistributionItem(
                    cDur,
                    0, 0, 0, 0, 0));
            }
            if (actor != seedActor)
            {
                if (distrib.TryGetValue(seedActor, out toModify))
                {
                    toModify.Extension += cDur;
                    distrib[seedActor] = toModify;
                }
                else
                {
                    distrib.Add(seedActor, new BoonDistributionItem(
                        0,
                        0, 0, 0, cDur, 0));
                }
                if (distrib.TryGetValue(actor, out toModify))
                {
                    toModify.Extended += cDur;
                    distrib[actor] = toModify;
                }
                else
                {
                    distrib.Add(actor, new BoonDistributionItem(
                        0,
                        0, 0, 0, 0, cDur));
                }
            }
            if (_src == 0)
            {
                if (distrib.TryGetValue(seedActor, out toModify))
                {
                    toModify.UnknownExtension += cDur;
                    distrib[seedActor] = toModify;
                }
                else
                {
                    distrib.Add(seedActor, new BoonDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
    }
}
