using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulationItemDuration : BuffSimulationItem
    {
        private readonly AgentItem _src;
        private readonly AgentItem _seedSrc;
        private readonly bool _isExtension;

        public BuffSimulationItemDuration(BuffStackItem other) : base(other.Start, other.Duration)
        {
            _src = other.Src;
            _seedSrc = other.SeedSrc;
            _isExtension = other.IsExtension;
        }

        public override void OverrideEnd(long end)
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

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedEvtcLog log)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = GetDistrib(distribs, boonid);
            long cDur = GetClampedDuration(start, end);
            if (cDur == 0)
            {
                return;
            }
            AgentItem agent = _src;
            AgentItem seedAgent = _seedSrc;
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.Value += cDur;
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
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
                    distrib.Add(agent, new BuffDistributionItem(
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
                    distrib.Add(seedAgent, new BuffDistributionItem(
                        0,
                        0, 0, 0, 0, cDur));
                }
            }
            if (agent == ParserHelper._unknownAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.UnknownExtension += cDur;
                    distrib[seedAgent] = toModify;
                }
                else
                {
                    distrib.Add(seedAgent, new BuffDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
    }
}
