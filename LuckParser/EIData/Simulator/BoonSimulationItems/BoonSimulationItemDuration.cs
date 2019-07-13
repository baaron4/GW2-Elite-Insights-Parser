using System;
using LuckParser.Parser;
using System.Collections.Generic;
using static LuckParser.EIData.BoonSimulator;
using LuckParser.Parser.ParsedData;

namespace LuckParser.EIData
{
    public class BoonSimulationItemDuration : BoonSimulationItem
    {
        private readonly AgentItem _src;
        private readonly AgentItem _seedSrc;
        private readonly bool _isExtension;

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.Start, other.BoonDuration)
        {
            _src = other.Src;
            _seedSrc = other.SeedSrc;
            _isExtension = other.IsExtension;
        }

        public override void SetEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0), Duration);
        }

        public override int GetStack()
        {
            return 1;
        }

        public override List<AgentItem> GetSources()
        {
            return new List<AgentItem>() { _src };
        }

        public override void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            long cDur = GetClampedDuration(start, end);
            AgentItem agent = _src;
            AgentItem seedAgent = _seedSrc;
            if (distrib.TryGetValue(agent, out BoonDistributionItem toModify))
            {
                toModify.Value += cDur;
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BoonDistributionItem(
                    cDur,
                    0, 0, 0, 0, 0));
            }
            if (_isExtension)
            {
                if (distrib.TryGetValue(agent, out toModify))
                {
                    toModify.Extension += cDur;
                    distrib[agent] = toModify;
                }
                else
                {
                    distrib.Add(agent, new BoonDistributionItem(
                        0,
                        0, 0, 0, cDur, 0));
                }
            }
            if (agent != seedAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.Extended += cDur;
                    distrib[seedAgent] = toModify;
                }
                else
                {
                    distrib.Add(seedAgent, new BoonDistributionItem(
                        0,
                        0, 0, 0, 0, cDur));
                }
            }
            if (agent == GeneralHelper.UnknownAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.UnknownExtension += cDur;
                    distrib[seedAgent] = toModify;
                }
                else
                {
                    distrib.Add(seedAgent, new BoonDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
    }
}
